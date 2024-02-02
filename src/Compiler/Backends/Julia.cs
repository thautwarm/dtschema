using System.Collections.Generic;

namespace DTSchema.Compiler.Backends
{
    // TODO: what is a good encoding of ADTs in Julia without intensive compilation lag?
    public sealed class Julia : Backend
    {
        Printer P = new();

        readonly string Required;

        public Julia(TypeStore store, string required) : base(store)
        {
            Required = required;
        }

        public override string CodeGen(TypeStore store)
        {
            Predef();
            foreach (var each in store.DeclaredTypes.Values)
            {
                PrePrintTypeDef(each);
            }

            foreach (var each in store.DeclaredTypes.Values)
            {
                PrintTypeDef(each);
            }

            return P.Builder.ToString();
        }

        void Predef()
        {
            // Julia-specific predefinitions
            // e.g., imports, initial configurations
            _ = P << "include (\"" + Required + "\")";
            _ = P << "";
        }

        void PrePrintTypeDef(TypeDef each)
        {
            if (each is TypeDefVariant variant)
            {
                _ = P << "struct " + variant.Name;
                _ = P << "  _data::Dict{String,Any}";
                _ = P << "end";
            }
        }

        void PrintTypeDef(TypeDef each)
        {
            // Handle different types of definitions
            if (each is TypeDefVariant variant)
            {
                PrintVariant(variant);
            }
            else if (each is TypeDefStruct @struct)
            {
                // PrintStruct(@struct);
            }
        }

        void PrintVariant(TypeDefVariant variant)
        {
            // Print the variant definition
            _ = P << "abstract type " + variant.Name + " end";
            _ = P << "";

        }
    }
}