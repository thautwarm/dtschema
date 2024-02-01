using System;
using System.Globalization;
using System.IO;
using System.Text;
using Antlr4.Runtime;

namespace DTSchema;

public sealed class ParseException : Exception
{
    public Pos Pos { get; init; }
    public ParseException(string message, Pos pos) : base(message)
    {
        Pos = pos;
    }
}

public sealed class CompileException : Exception
{
    public Pos Pos { get; init; }
    public CompileException(string message, Pos pos) : base(message)
    {
        Pos = pos;
    }
}

internal sealed class lexer_error_listener : IAntlrErrorListener<int>
{
    private static readonly lexer_error_listener instance = new();
    public static lexer_error_listener Instance => instance;

    public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        StringBuilder builder = new();
        builder.Append($"Error on line {line} at position {charPositionInLine + 1}:");
        builder.AppendLine(msg);

        var pos = new Pos(line, charPositionInLine + 1, recognizer.InputStream.SourceName);

        throw new ParseException(builder.ToString(), pos);
    }
}

internal sealed class parser_error_listener : BaseErrorListener
{
    private static readonly parser_error_listener instance = new();
    public static parser_error_listener Instance => instance;

    public override void SyntaxError(System.IO.TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
    {
        StringBuilder builder = new();

        // the human readable message
        var sourceName = recognizer.InputStream.SourceName;
        builder.AppendFormat(CultureInfo.InvariantCulture, "Error on line {0} at position {1}:\n", line, charPositionInLine + 1);
        // the actual error message
        builder.AppendLine(msg);
#if MY_DEBUG
            builder.AppendLine($"Debug: Offending symbol type: {recognizer.Vocabulary.GetSymbolicName(offendingSymbol.Type)}");
#endif

        var pos = new Pos(line, charPositionInLine + 1, sourceName);

        if (offendingSymbol.TokenSource != null)
        {
            // the line with the error on it
            string input = offendingSymbol.TokenSource.InputStream.ToString() ?? "";
            string[] lines = input.Split('\n');
            string errorLine = lines[line - 1];
            builder.AppendLine(errorLine);

            // adding indicator symbols pointing out where the error is on the line
            int start = offendingSymbol.StartIndex;
            int stop = offendingSymbol.StopIndex;
            if (start >= 0 && stop >= 0)
            {
                // the end point of the error in "line space"
                int end = (stop - start) + charPositionInLine + 1;
                for (int i = 0; i < end; i++)
                {
                    // move over until we are at the point we need to be
                    if (i >= charPositionInLine && i < end)
                    {
                        builder.Append('^');
                    }
                    else
                    {
                        builder.Append(' ');
                    }
                }
            }
        }

        throw new ParseException(builder.ToString(), pos);
    }
}

