using System;
using System.Text;
using DTSchema.Compiler;
using SystemX.CommandLine;

namespace DTSchema;

public static class EntryPoint
{

    enum Backend
    {
        dart, python, julia, csharp, typescript
    }

    class Options
    {
        public string File = "main.dts";
        public string? Out = null;
        public Backend Backend = Backend.dart;
        public string? Required;

    }

    static void PrintHelp()
    {

        Console.WriteLine("Usage: dts [options] <File> <Output>");
        Console.WriteLine("Options:");
        Console.WriteLine("  --help: Show this help message");
        Console.WriteLine("  --backend: The backend to use");
        Console.WriteLine("             Possible values: dart, typescript");
        Console.WriteLine("  --require: The name of the required module");
        Console.WriteLine("  <File>: The input .dts file");
        Console.WriteLine("  <Output>: The output file");
        Environment.Exit(0);
    }

    public static void Main(string[] argv)
    {
        CmdParser<Options> parser = new();
        parser.Flag("help", (Options o) => PrintHelp());
        parser.Positional("File", (Options o, string v) => o.File = v);
        parser.Positional("Output", (Options o, string v) => o.Out = v);
        parser.Named("require", (Options o, string v) => o.Required = v);

        parser.Named("backend", (Options o, string v) =>
        {
            o.Backend = v.ToLowerInvariant() switch
            {
                "dart" => Backend.dart,
                "python" => Backend.python,
                "julia" => Backend.julia,
                "csharp" => Backend.csharp,
                "typescript" or "ts" => Backend.typescript,
                _ => throw new Exception($"Unknown backend {v}"),
            };
        });

        var options = parser.Parse(argv);

        var fileContent = System.IO.File.ReadAllText(options.File, Encoding.UTF8);
        var defs = DTSchema.Parser.DTSchemaParser.ParseAll(fileContent, sourceName: options.File);

        TypeStore typeStore = new();
        typeStore.Load(defs);

        var outputFile = options.Out ?? "out";
        var require = options.Required ?? System.IO.Path.GetFileNameWithoutExtension(options.File) + "_require";
        switch (options.Backend)
        {
            case Backend.dart:
                var dartBe = new Compiler.Backends.Dart(typeStore, require);
                System.IO.File.WriteAllText(
                    outputFile,
                    dartBe.CodeGen(typeStore),
                    Encoding.UTF8
                );
                break;
            case Backend.typescript:
                var tsBe = new Compiler.Backends.TypeScript(typeStore, require);
                System.IO.File.WriteAllText(
                    outputFile,
                    tsBe.CodeGen(typeStore),
                    Encoding.UTF8
                );
                break;
            default:
                throw new Exception($"Unknown backend {options.Backend}");
        }
    }
}
