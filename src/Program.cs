using System;
using System.Text;
using DTSchema.Compiler;
using SystemX.CommandLine;

namespace DTSchema;

public static class EntryPoint
{

    enum Backend
    {
        dart, python, julia, csharp
    }

    class Options
    {
        public string File = "main.dts";
        public string? Out = null;
        public Backend Backend = Backend.dart;
        public string OutputModule = "module";

    }

    public static void Main(string[] argv)
    {
        CmdParser<Options> parser = new();
        parser.Positional("File", (Options o, string v) => o.File = v);
        parser.Positional("Output", (Options o, string v) => o.Out = v);
        parser.Named("module", (Options o, string v) => o.OutputModule = v);

        parser.Named("backend", (Options o, string v) => {
            switch (v.ToLowerInvariant())
            {
                case "dart":
                    o.Backend = Backend.dart;
                    break;
                case "python":
                    o.Backend = Backend.python;
                    break;
                case "julia":
                    o.Backend = Backend.julia;
                    break;
                case "csharp":
                    o.Backend = Backend.csharp;
                    break;
                default:
                    throw new Exception($"Unknown backend {v}");
            }
        });

        var options = parser.Parse(argv);

        var fileContent = System.IO.File.ReadAllText(options.File, Encoding.UTF8);
        var defs = DTSchema.Parser.DTSchemaParser.ParseAll(fileContent, sourceName: options.File);

        TypeStore typeStore = new();
        typeStore.Load(defs);

        var outputFile = options.Out ?? "out";
        switch (options.Backend)
        {
            case Backend.dart:
                var dartBe = new DTSchema.Compiler.Backends.DartHost(typeStore, options.OutputModule);
                System.IO.File.WriteAllText(
                    outputFile,
                    dartBe.CodeGen(typeStore),
                    Encoding.UTF8
                );
                break;
            default:
                throw new Exception($"Unknown backend {options.Backend}");
        }
    }
}
