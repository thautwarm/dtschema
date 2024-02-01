#pragma warning disable IDE1006, CS3021

using System;
using System.Collections.Generic;
using Antlr4.Runtime;

namespace DTSchema.Parser;

internal sealed class SourceStream: AntlrInputStream
{
    readonly string _me_SourceName;
    public SourceStream(string code, string sourceName): base(code)
    {
        _me_SourceName = sourceName;
    }
    public override string SourceName => _me_SourceName;
}

public partial class DTSchemaParser
{
    static string lexeme(IToken token) => token.Text;

    static List<T> appendList<T>(List<T> list, T item)
    {
        list.Add(item);
        return list;
    }

    static bool notSingular<T>(List<T> list) => list.Count != 1;

    static T select<T>(bool cond, T a, T b) => cond ? a : b;

    static T firstElt<T>(List<T> list) => list[0];

    static Pos getPos(IToken token) => new(token.Line, token.Column, token.TokenSource.SourceName);

    public static List<Def> ParseAll(string code, string sourceName = "unknown")
    {
        var inputStream = new SourceStream(code, sourceName);
        var lexer = new DTSchemaLexer(inputStream);
        lexer.RemoveErrorListeners();
        lexer.AddErrorListener(lexer_error_listener.Instance);
        var tokenStream = new CommonTokenStream(lexer);
        var parser = new DTSchemaParser(tokenStream)
        {
            BuildParseTree = false
        };
        parser.RemoveErrorListeners();
        parser.AddErrorListener(parser_error_listener.Instance);
        return parser.start().result;
    }
}
