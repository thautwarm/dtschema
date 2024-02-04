using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DTSchema.Compiler.Backends
{

    /**
        The Julia backend is now pretty restricted:
        1. support only construction of data types
        2. do not support data manipulation such as getters and setters

        The reason is to counter the following drawbacks of Julia:
        1. Type definitions and related method specialization in Julia cause notable compilation lags
        2. Julia does not support forward declaration of types (we actually simulate this)
    */
    public sealed class Julia(TypeStore store, string required) : Backend(store)
    {
        Printer P = new();

        readonly string Required = required;

        public Dictionary<string, HashSet<string>> Dependencies = [];

        public override string CodeGen(TypeStore store)
        {
            _ = P << "# This file is generated by DTSchema compiler, do not edit";
            Predef();

            // handle declarations and resolve dependencies
            foreach (var each in store.DeclaredTypes.Values)
            {
                PrePrintTypeDef(each);
            }

            // generate type definitions for record types
            var orders = Utils.TopoSort(
                Dependencies.Keys.ToList(),
                ignoreCyclic: true,
                getDeps: (subject) =>
                {
                    return Dependencies.TryGetValue(subject, out var deps) ? deps : [];
                }
            );

            foreach (var each in orders)
            {
                if (store.DeclaredTypes.TryGetValue(each, out var def))
                {
                    PrintStruct((TypeDefStruct)def, null);
                }
            }

            // generate ADT cases and
            // type information for exhaustive matching stuffs
            var caseMap = new Dictionary<string, List<string>>();
            foreach (var each in store.DeclaredTypes.Values)
            {
                if (each is TypeDefVariant variant)
                {
                    var ctors = caseMap[each.Name] = [];
                    for (int i = 0; i < variant.Cases.Count; i++)
                    {
                        var (tag, fields) = variant.Cases[i];
                        PrintStruct(new TypeDefStruct(tag, fields), (i, tag, variant.Name));
                        ctors.Add(tag);
                    }
                }
            }


            _ = P << "const __taginfo__ = (";
            P++;
            foreach (var (sortName, ctors) in caseMap)
            {
                _ = P << "(" + sortName + ",";
                {
                    P++;
                    _ = P << "(";
                    for (int i = 0; i < ctors.Count; i++)
                    {
                        _ = P << ctors[i] + ",";
                    }
                    _ = P << ")";
                    P--;
                }
                _ = P << "),";
            }
            P--;
            _ = P << ")";

            return P.Builder.ToString();
        }

        void PrintStruct(TypeDefStruct @struct, (int TagIndex, string TagName, string SortName)? tagInfo)
        {
            _ = P << "export " + @struct.Name;
            if (tagInfo != null)
            {
                _ = P << "Base.@kwdef struct " + @struct.Name + " <: " + tagInfo.Value.SortName;
            }
            else
            {
                _ = P << "Base.@kwdef struct " + @struct.Name;
            }

            {
                P++;
                if (tagInfo != null)
                {
                    _ = P << "__tag::Int = " + tagInfo.Value.TagIndex;
                }
                foreach (var field in @struct.Fields)
                {
                    if (field.nullable)
                    {
                        _ = P << field.name + "::Union{" + ShowType(field.typ) + ", Nothing} = nothing";
                    }
                    else
                    {
                        _ = P << field.name + "::" + ShowType(field.typ);
                    }
                }
                P--;
            }
            _ = P << "end";
        }

        void VisitDeps(string recordTypeName, Ty t)
        {
            switch (t)
            {
                case TyJSON _:
                case TyEnum _:
                    break;
                case TyList list:
                    VisitDeps(recordTypeName, list.elt);
                    break;
                case TyMap map:
                    VisitDeps(recordTypeName, map.key);
                    VisitDeps(recordTypeName, map.value);
                    break;
                case TyFn fn:
                    for (int i = 0; i < fn.args.Count; i++)
                    {
                        VisitDeps(recordTypeName, fn.args[i].typ);
                    }
                    if (fn.ret is HasRet hasRet)
                    {
                        VisitDeps(recordTypeName, hasRet.ret);
                    }
                    break;
                case TyNamed named:
                    if (Store.DeclaredTypes.TryGetValue(named.name, out var def) && def is TypeDefStruct)
                    {
                        if (!Dependencies.TryGetValue(recordTypeName, out var deps))
                        {
                            deps = [];
                            Dependencies[recordTypeName] = deps;
                        }
                        deps.Add(named.name);
                    }
                    break;
                case TyTuple tuple:
                    foreach (var each in tuple.elts)
                    {
                        VisitDeps(recordTypeName, each);
                    }
                    break;
                default:
                    throw new UnreachableException();
            }
        }

        void Predef()
        {
            // Julia-specific predefinitions
            // e.g., imports, initial configurations
            _ = P << "include(\"" + Required + "\")";
            _ = P << "";
        }

        void PrePrintTypeDef(TypeDef each)
        {
            if (each is TypeDefVariant variant)
            {
                _ = P << "export " + variant.Name;
                _ = P << "abstract type " + variant.Name + " <: TaggedUnion end";
            }
            else if (each is TypeDefStruct @struct)
            {
                Dependencies[@struct.Name] = [];
                for (int i = 0; i < @struct.Fields.Count; i++)
                {
                    var field = @struct.Fields[i];
                    VisitDeps(@struct.Name, field.typ);
                }
            }
            else if (each is TypeDefEnum @enum)
            {
                _ = P << "export " + @enum.Name;
                _ = P << "baremodule " + @enum.Name;
                {
                    P++;
                    _ = P << "using Base: @enum";
                    _ = P << "export T";
                    foreach (var eachCase in @enum.Cases)
                    {
                        _ = P << "export " + Rename(eachCase);
                    }
                    _ = P << string.Join(" ", @enum.Cases.Select(Rename).Prepend("T").Prepend("@enum"));
                    P--;
                }
                _ = P << "end";
            }
        }


        string ShowType(Ty t)
        {
            switch (t)
            {
                case TyJSON _:
                    return "RawJson";
                case TyEnum @enum:
                    return @enum.name + ".T";
                case TyFn fn:
                    // this is pretty annoying
                    // but we can't really do anything about it
                    var argType = "Tuple{" + string.Join(", ", fn.args.Select((x) => ShowType(x.typ))) + "}";
                    if (fn.ret is HasRet ret)
                    {
                        return "Signal{" + argType + ", " + ShowType(ret.ret) + "}";
                    }
                    else
                    {
                        return "VoidSignal{" + argType + "}";
                    }
                case TyList list:
                    return "Vector{" + ShowType(list.elt) + "}";
                case TyMap map:
                    return "Dict{" + ShowType(map.key) + "," + ShowType(map.value) + "}";
                case TyNamed named:
                    return TypeMap(named.name);
                case TyTuple tuple:
                    return "Tuple{" + string.Join(", ", tuple.elts.Select(ShowType)) + "}";
                default:
                    throw new UnreachableException();
            }
        }

        static string Rename(string varName)
        {
            if (varName == "end")
                return "end_";
            return varName;
        }

        static string TypeMap(string typeName)
        {
            return typeName switch
            {
                "bool" => "Bool",
                "int" => "Int",
                "float" => "Cdouble",
                "str" => "String",
                _ => typeName,
            };
        }
    }
}