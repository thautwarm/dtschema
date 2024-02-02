using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DTSchema.Compiler;

public abstract record class TypeDef(string Name);
public record class TypeDefStruct(string Name, List<Field> Fields) : TypeDef(Name);
public record class TypeDefVariant(string Name, List<(string Tag, List<Field> fields)> Cases) : TypeDef(Name);
public record class TypeDefExtern(string Name) : TypeDef(Name);
public record class TypeDefEnum(string Name, List<string> Cases) : TypeDef(Name);

abstract record class TKey;

public class TypeStore
{
    public readonly Dictionary<string, TypeDef> DeclaredTypes = new();
    public readonly Dictionary<string, int> TypeIds = new();

    public int GetTypeId(Ty t)
    {
        var name = GetTypeNameUnique(t);
        if (TypeIds.TryGetValue(name, out var id))
        {
            return id;
        }
        else
        {
            id = TypeIds.Count;
            TypeIds.Add(name, id);
            return id;
        }
    }

    string GetTypeNameUnique(Ty t)
    {
        switch (t)
        {
            case TyJSON _:
                return ":json";
            case TyEnum @enum:
                return "enum:" + @enum.name;
            case TyNamed named:
                return "@" + named.name;
            case TyMap map:
                return "{" + GetTypeNameUnique(map.key) + ":" + GetTypeNameUnique(map.value) + "}";
            case TyList list:
                return "[" + GetTypeNameUnique(list.elt) + "]";
            case TyTuple tuple:
                return "(" + string.Join(",", tuple.elts.Select(GetTypeNameUnique)) + ")";
            case TyFn fn:
                if (fn.ret is HasRet hasRet)
                {
                    return "fn(" + string.Join(",", fn.args.Select(x => GetTypeNameUnique(x.typ))) + ")" + ":" + GetTypeNameUnique(hasRet.ret);
                }
                return "fn(" + string.Join(",", fn.args.Select(x => GetTypeNameUnique(x.typ))) + ")";
            default:
                throw new UnreachableException();
        }
    }

    public void PreDefine(Def definition)
    {
        switch (definition)
        {
            case DefEnum defEnum:
                {
                    if (DeclaredTypes.TryGetValue(defEnum.name, out var _)) return;
                    DeclaredTypes.Add(defEnum.name, new TypeDefEnum(defEnum.name, defEnum.cases));
                    break;
                }
            case DefSort defSort:
                {
                    if (DeclaredTypes.TryGetValue(defSort.sort.name, out var existing)) return;
                    DeclaredTypes.Add(defSort.sort.name, new TypeDefVariant(defSort.sort.name, new()));
                    break;
                }
            case DefExtern defExtern:
                {
                    if (DeclaredTypes.TryGetValue(defExtern.name, out var existing)) return;
                    DeclaredTypes.Add(defExtern.name, new TypeDefExtern(defExtern.name));
                    break;
                }
            case DefRecord defRecord:
                {
                    if (DeclaredTypes.TryGetValue(defRecord.ctor.name, out var existing)) return;
                    DeclaredTypes.Add(defRecord.ctor.name, new TypeDefStruct(defRecord.ctor.name, new()));
                    break;
                }
            default:
                throw new UnreachableException();
        }
    }

    public void Load(IEnumerable<Def> definition)
    {
        var defs = definition.ToList();
        foreach (var def in defs)
        {
            PreDefine(def);
        }
        foreach (var def in defs)
        {
            Define(def);
        }
    }

    public void Define(Def definition)
    {
        switch (definition)
        {
            case DefSort defSort:
                {
                    if (DeclaredTypes.TryGetValue(defSort.sort.name, out var existing))
                    {
                        if (existing is TypeDefVariant variant)
                        {
                            for (var i = 0; i < defSort.sort.variants.Count; i++)
                            {
                                var variantDef = defSort.sort.variants[i];
                                variant.Cases.Add((variantDef.name, variantDef.fields));
                            }
                        }
                        else
                        {
                            throw new Exception($"Type {defSort.sort.name} is already defined as {existing}");
                        }
                    }
                    else
                    {
                        throw new Exception($"Type {defSort.sort.name} is not defined");
                    }
                    break;
                }
            case DefRecord defRecord:
                {
                    if (DeclaredTypes.TryGetValue(defRecord.ctor.name, out var existing))
                    {
                        if (existing is TypeDefStruct typeDefStruct)
                        {
                            if (typeDefStruct.Fields.Count == 0)
                            {
                                typeDefStruct.Fields.AddRange(defRecord.ctor.fields);
                            }
                            else
                            {
                                throw new Exception($"Type {defRecord.ctor.name} is already defined as {existing}");
                            }
                        }
                        else
                        {
                            throw new Exception($"Type {defRecord.ctor.name} is already defined as {existing}");
                        }
                    }
                    else
                    {
                        throw new Exception($"Type {defRecord.ctor.name} is not defined");
                    }
                    break;
                }
            case DefEnum _:
            case DefExtern _:
                break;
        }
    }
}
