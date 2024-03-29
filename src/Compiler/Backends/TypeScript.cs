using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DTSchema.Compiler.Backends
{
    /**
        The encoding of ADTs in TypeScript is straghtforward
        with only replacing the functions with signal slots.
    */
    public sealed class TypeScript : Backend
    {
        Printer P = new();
        readonly string Required;

        // Trim .ts extension
        string RequiredModule => System.IO.Path.GetFileNameWithoutExtension(Required);

        string unionSortName = "";
        int unionTagIndex = 0;
        public TypeScript(TypeStore store, string required) : base(store)
        {
            Required = required;
            foreach (var (k, _) in store.DeclaredTypes)
            {
                AllGeneratedTypes.Add(k);
            }
        }

        public override string CodeGen(TypeStore store)
        {
            _ = P << "// This file is generated by DTSchema compiler, do not edit";
            _ = P << $"import * as {RequiredModule} from \"./{Required}\";";
            _ = P << $"export * from \"./{Required}\"";
            _ = P << "";

            foreach (var each in store.DeclaredTypes.Values)
            {
                PrintTypeDef(each);
                _ = P << "";
            }

            return P.Builder.ToString();
        }

        void PrintTypeDef(TypeDef typeDef)
        {
            if (typeDef is TypeDefVariant variant)
            {
                PrintVariant(variant);
            }
            else if (typeDef is TypeDefStruct @struct)
            {
                PrintStruct(@struct, false);
            }
            else if (typeDef is TypeDefEnum @enum)
            {
                _ = P << $"export enum {@enum.Name} {{";
                {
                    P++;
                    foreach (var each in @enum.Cases)
                    {
                        _ = P << $"{each},";
                    }
                    P--;
                }
                _ = P << "}";
            }
        }

        void PrintVariant(TypeDefVariant variant)
        {
            _ = P << $"export interface {variant.Name} {{";
            {
                P++;

                // brand
                _ = P << $"  __brand: \"{variant.Name}\";";
                _ = P << $"  __tag: number";
                P--;
            }

            _ = P << "}";

            int tag = 0;
            unionSortName = variant.Name;
            foreach (var (Tag, fields) in variant.Cases)
            {
                unionTagIndex = tag;
                PrintStruct(new TypeDefStruct(Tag, fields), true);
                tag++;
            }
        }

        void PrintStruct(TypeDefStruct @struct, bool hasTag)
        {
            if (!hasTag)
            {
                _ = P << $"export interface {@struct.Name} {{";

                // fields
                {
                    P++;
                    foreach (var each in @struct.Fields)
                    {
                        if (each.nullable)
                        {
                            _ = P << $"{each.name}?: {ShowType(each.typ, fnToSignal: true)};";
                        }
                        else
                        {
                            _ = P << $"{each.name}: {ShowType(each.typ, fnToSignal: true)};";
                        }
                    }
                    P--;
                }

                _ = P << "}";
            }

            string retType = hasTag ? unionSortName : @struct.Name;

            // generate constructor
            var argSig = string.Join(
                ", ",
                @struct.Fields.Select(x =>
                {
                    if (x.nullable)
                    {
                        return x.name + "?: " + ShowType(x.typ, fnToSignal: false);
                    }
                    return x.name + ": " + ShowType(x.typ, fnToSignal: false);
                })
            );
            _ = P << "export function " + @struct.Name + "(opts: {" + argSig + "})" + "{";
            {
                P++;
                _ = P << $"return <{retType}><unknown>" + "{";
                {
                    P++;
                    if (hasTag)
                    {
                        _ = P << $"__tag: {unionTagIndex},";
                    }
                    foreach (var each in @struct.Fields)
                    {
                        _ = P << $"{each.name}: " + ShowSerde(each.typ, "opts." + each.name, each.nullable) + ",";
                    }
                    P--;
                }
                _ = P << "};";
                P--;
            }
            _ = P << "}";
        }

        string TypeMap(string typeName)
        {
            return typeName switch
            {
                "bool" => "boolean",
                "int" => "number",
                "float" => "number",
                "str" => "string",
                _ => typeName,
            };
        }

        bool HasFunction(Ty t)
        {
            return t switch
            {
                TyFn _ => true,
                TyList list => HasFunction(list.elt),
                TyTuple tuple => tuple.elts.Any(HasFunction),
                TyMap map => HasFunction(map.key) || HasFunction(map.value),
                TyNamed named => false,
                TyEnum @enum => false,
                TyJSON _ => false,
                _ => throw new UnreachableException(),
            };
        }

        string ShowSerde(Ty t, string expr, bool nullable)
        {
            if (HasFunction(t))
            {
                return ShowSerdeHasFunc(t, expr, nullable);
            }
            else
            {
                return ShowSerdeNoFunc(t, expr);
            }
        }

        static string ShowSerdeNoFunc(Ty _, string expr) => expr;

        string ShowSerdeHasFunc(Ty t, string expr, bool nullable)
        {
            switch (t)
            {
                case TyFn fn:
                    if (nullable)
                        return string.Format("((_x) => (_x === null || _x === undefined) ? null : {0}.signal(_x))({1})", RequiredModule, expr);
                    else
                        return string.Format("{0}.signal({1})", RequiredModule, expr);
                case TyList list:
                    if (nullable)
                        return string.Format("{0}?.map((x) => {1})", expr, ShowSerde(list.elt, "x", false));
                    else
                        return string.Format("{0}.map((x) => {1})", expr, ShowSerde(list.elt, "x", false));
                case TyTuple tuple:
                    if (nullable)
                    {
                        return string.Format("({0})", string.Join(",", tuple.elts.Select((x, i) => ShowSerde(x, expr + $"?.[{i}]", nullable))));
                    }
                    return string.Format("({0})", string.Join(",", tuple.elts.Select((x, i) => ShowSerde(x, expr + $"[{i}]", nullable))));
                case TyMap map:
                    if (nullable)
                    {
                        return string.Format(
                            "((_x) => (_x === null || _x === undefined) ? null : Object.fromEntries(Object.entries(_x).map(([k,v]) => [{1},{2}])))({0})",
                            expr,
                            ShowSerde(map.key, "k", false),
                            ShowSerde(map.value, "v", false)
                        );
                    }
                    return string.Format(
                        "Object.fromEntries(Object.entries({0}).map(([k,v]) => [{1},{2}]))",
                        expr,
                        ShowSerde(map.key, "k", false),
                        ShowSerde(map.value, "v", false)
                    );
                case TyNamed named:
                    return expr;
                case TyEnum @enum:
                    return expr;
                case TyJSON _:
                    return expr;
                default:
                    throw new UnreachableException();
            }
        }

        string ShowType(Ty t, bool fnToSignal)
        {
            switch (t)
            {
                case TyJSON _:
                    return "any";
                case TyEnum @enum:
                    return @enum.name;
                case TyList list:
                    return "Array<" + ShowType(list.elt, fnToSignal) + ">";
                case TyMap map:
                    return "Record<" + ShowType(map.key, fnToSignal) + "," + ShowType(map.value, fnToSignal) + ">";
                case TyNamed named:
                    return TypeMap(named.name);
                case TyTuple tuple:
                    return "[" + string.Join(",", tuple.elts.Select((x) => ShowType(x, fnToSignal))) + "]";
                case TyFn fn:
                    string funcRet;
                    if (fn.ret is HasRet hasRet)
                    {
                        if (hasRet.nullable)
                        {
                            funcRet = "Promise<" + ShowType(hasRet.ret, fnToSignal) + " | null | undefined" + ">";
                        }
                        else
                        {
                            funcRet = "Promise<" + ShowType(hasRet.ret, fnToSignal) + ">";
                        }
                    }
                    else
                    {
                        funcRet = "Promise<void>";
                    }
                    var argTypes = string.Join(
                        ",",
                        fn.args.Select(x =>
                        {
                            if (x.nullable)
                            {
                                return x.name + "?:" + ShowType(x.typ, fnToSignal);
                            }
                            return x.name + ":" + ShowType(x.typ, fnToSignal);
                        })
                    );
                    if (fnToSignal)
                    {
                        return $"{RequiredModule}.Signal" + "<" + "(" + argTypes + ")" + "=>" + funcRet + ">";
                    }
                    return "(" + "(" + argTypes + ")" + "=>" + funcRet + ")";
                default:
                    throw new UnreachableException();
            }
        }
    }
}