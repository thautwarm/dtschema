using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;

namespace DTSchema.Compiler.Backends;

// dart is a host backend, which
// means it supports deserialization
// but not serialization
public sealed class DartHost : Backend
{
    Printer P = new();
    int unionTagIndex = 0;
    string unionSortName = "";

    readonly HashSet<string> AllGeneratedTypes = [];

    readonly string OutputModule;

#pragma warning disable
    public DartHost(TypeStore store, string outputModule) : base(store)
    {
        OutputModule = outputModule;
        foreach (var (k, v) in store.DeclaredTypes)
        {
            AllGeneratedTypes.Add(k);
            if (v is TypeDefVariant variant)
            {
                foreach (var tag in variant.Cases)
                {
                    AllGeneratedTypes.Add(tag.Tag);
                }
            }
        }
    }
#pragma warning restore

    public Defer SetTag(int tagIndex)
    {
        var old = unionTagIndex;
        unionTagIndex = tagIndex;
        return new Defer(() =>
        {
            unionTagIndex = old;
        });
    }

    public Defer SetSort(string sortName)
    {
        var old = unionSortName;
        unionSortName = sortName;
        return new Defer(() =>
        {
            unionSortName = old;
        });
    }

    public override string CodeGen(TypeStore store)
    {
        Predef();
        foreach (var each in store.DeclaredTypes.Values)
        {
            PrintTypeDef(each);
        }

        return P.Builder.ToString();
    }

    void Predef()
    {
        _ = P << $"part of '{OutputModule}.dart';";
        _ = P << "";
    }

    void PrintTypeDef(TypeDef each)
    {
        if (each is TypeDefVariant variant)
        {
            PrintVariant(variant);
        }
        else if (each is TypeDefStruct @struct)
        {
            PrintStuct(@struct, false);
        }
        else
        {
            return;
        }
    }

    void PrintVariant(TypeDefVariant variant)
    {
        _ = P << "abstract class " + variant.Name + " {";
        ++P;
        _ = P << "int get __Tag;";

        // print the serialization impl
        {
            _ = P << $"static dynamic toJson(SerdeCtx ctx, {variant.Name} self) {{";
            {
                ++P;
                int tag = 0;
                _ = P << "switch (self.__Tag) {";
                {
                    ++P;
                    foreach (var (Tag, fields) in variant.Cases)
                    {
                        _ = P << "case " + tag + ":";
                        ++P;
                        _ = P << "return " + "Serde.tagged(" + tag + ", " +  ShowSerde(new TyNamed(Tag), "self as " + Tag, false) + ")" + ";";
                        --P;
                        tag++;
                    }
                    _ = P << "default: throw Exception(\"" + variant.Name + ":unknown tag\");";
                    --P;
                }
                _ = P << "}";
                --P;
            }
            _ = P << "}";
        }

        // print the deserialization impl
        {
            _ = P << "static " + variant.Name + " fromJson(SerdeCtx ctx, dynamic json) {";
            ++P;
            // check if dict
            _ = P << "if (json is! Map<String, dynamic>) throw Exception(\""
                     + variant.Name + ":" + "expected a Map<String, dynamic> but got ${json.runtimeType}\");";

            _ = P << "var tag = json[\"__Tag\"];";
            _ = P << "if (tag is! num) throw Exception(\""
                     + variant.Name + ":" + "expected a int but got ${tag.runtimeType}\");";

            _ = P << "switch (tag.toInt()) {";
            {
                ++P;
                int tag = 0;
                foreach (var (Tag, _) in variant.Cases)
                {
                    _ = P << "case " + tag + ":";
                    ++P;
                    _ = P << "return " + ShowDeserdeNotNull(new TyNamed(Tag), "json") + ";";
                    --P;
                    tag++;
                }
                _ = P << "default: throw Exception(\"" + variant.Name + ":unknown tag\");";
                --P;
            }
            _ = P << "}";
            --P;
            _ = P << "}";
        }
        --P;
        _ = P << "}";

        // print the constructors/cases
        {
            int tag = 0;
            unionSortName = variant.Name;
            foreach (var (Tag, fields) in variant.Cases)
            {
                unionTagIndex = tag;
                PrintStuct(new TypeDefStruct(Tag, fields), true);
                tag++;
            }
        }
    }

    void PrintStuct(TypeDefStruct @struct, bool hasSort)
    {
        if (hasSort)
        {
            _ = P << "class " + @struct.Name + " extends " + unionSortName + " {";
        }
        else
        {
            _ = P << "class " + @struct.Name + " {";
        }
        {
            ++P;

            if (hasSort)
            {
                // print the tag getter
                _ = P << "int get __Tag => " + unionTagIndex + ";";
            }

            foreach (var field in @struct.fields)
            {
                _ = P << ShowType(field.typ) + " " + field.name + ";";
            }

            // print the dart constructor
            _ = P << @struct.Name + "(";
            {
                ++P;
                foreach (var field in @struct.fields)
                {
                    _ = P << "this." + field.name + ",";
                }
                _ = P << ");";
                --P;
            }


            // print the serialization impl
            _ = P << $"static dynamic toJson(SerdeCtx ctx, {@struct.Name} self) {{";
            {
                ++P;
                _ = P << "return <String, dynamic>{";
                {
                    ++P;
                    foreach (var field in @struct.fields)
                    {
                        _ = P << "\"" + field.name + "\": " + ShowSerde(field.typ, "self." + field.name, field.nullable) + ",";
                    }
                    --P;
                }
                _ = P << "};";
                --P;
            }
            _ = P << "}";


            // print the deserialization impl
            _ = P << "static " + @struct.Name + " fromJson(SerdeCtx ctx, dynamic json) {";
            {
                ++P;
                // check if dict
                _ = P << "if (json is! Map<String, dynamic>) throw Exception(\""
                         + @struct.Name + ":"
                         + "expected a Map<String, dynamic> but got ${json.runtimeType}\");";

                _ = P << "return " + @struct.Name + "(";
                {
                    ++P;
                    foreach (var field in @struct.fields)
                    {
                        _ = P << ShowDeserde(field.typ, "json[\"" + field.name + "\"]", field.nullable) + ",";
                    }
                    --P;
                }
                _ = P << ");";
                --P;
            }
            _ = P << "}";
        }
        --P;
        _ = P << "}";
    }

    string ShowType(Ty t)
    {
        switch (t)
        {
            case TyList list:
                return "List<" + ShowType(list.elt) + ">";
            case TyMap map:
                return "Map<" + ShowType(map.key) + "," + ShowType(map.value) + ">";
            case TyNamed named:
                return TypeMap(named.name);
            case TyTuple tuple:
                return "(" + string.Join(",", tuple.elts.Select(ShowType)) + ")";
            case TyFn fn:
                string asyncRet;
                if (fn.ret is HasRet hasRet)
                {
                    if (hasRet.nullable)
                    {
                        asyncRet = "Future<" + ShowType(hasRet.ret) + "?>";
                    }
                    else
                    {
                        asyncRet = "Future<" + ShowType(hasRet.ret) + ">";
                    }
                }
                else
                {
                    asyncRet = "Future<void>";
                }
                var asyncArgs = string.Join(",", fn.args.Select(x => ShowType(x.typ)));
                return asyncRet + " Function(" + asyncArgs + ")";
            default:
                throw new UnreachableException();
        }
    }

    string ShowSerde(Ty t, string expr, bool nullable)
    {
        expr = "(" + expr + ")";
        if (nullable)
        {
            return string.Format("Serde.nullable({0}, {1})", expr, "(x) => " + ShowSerdeNotNull(t, "x"));
        }
        return ShowSerdeNotNull(t, expr);
    }

    string ShowSerdeNotNull(Ty t, string expr)
    {
        switch (t)
        {
            case TyList list:
                return string.Format("Serde.list({0}, {1})", expr, "(_x) => " + ShowSerdeNotNull(list.elt, "_x"));
            case TyTuple tuple:
                var r = string.Join(",", tuple.elts.Select((t, i) => ShowSerdeNotNull(t, expr + ".$" + (i + 1))));
                return "(" + r + ")";
            case TyMap map:
                return string.Format(
                    "Serde.map({0}, {1}, {2})",
                    expr,
                    "(_k) => " + ShowSerdeNotNull(map.key, "_k"),
                    "(_v) => " + ShowSerdeNotNull(map.value, "_v")
                );
            case TyNamed named:
                if (AllGeneratedTypes.Contains(named.name))
                    return string.Format("{0}.toJson(ctx, {1})", ShowType(named), expr);
                return string.Format("Serde.{0}(ctx, {1})", named.name, expr);
            case TyFn fn:
                var tStr = ShowType(fn);
                return string.Format("Serde.unsupported<{0}>(\"cannot serialize a function at HOST side\")", tStr);
            default:
                throw new UnreachableException();
        }
    }

    string ShowDeserde(Ty t, string expr, bool nullable)
    {
        expr = "(" + expr + ")";
        if (nullable)
        {
            var tStr = ShowType(t);
            return string.Format(
                "Deserde.nullable<{0}>({1}, {2})",
                tStr,
                expr,
                "(x) => " + ShowDeserdeNotNull(t, "x")
            );
        }
        return ShowDeserdeNotNull(t, expr);
    }

    string ShowDeserdeNotNull(Ty t, string expr)
    {
        switch (t)
        {
            case TyList list:
                return string.Format(
                    "Deserde.list<{0}>({1}, {2})",
                    ShowType(list.elt),
                    expr,
                    "(_x) => " + ShowDeserdeNotNull(list.elt, "_x")
                );
            case TyTuple tuple:
                var r = string.Join(",", tuple.elts.Select((t, i) => ShowDeserdeNotNull(t, expr + ".$" + (i + 1))));
                return "(" + r + ")";
            case TyMap map:
                return string.Format(
                    "Deserde.map<{0},{1}>({2}, {3}, {4})",
                    ShowType(map.key),
                    ShowType(map.value),
                    expr,
                    "(_k) => " + ShowDeserdeNotNull(map.key, "_k"),
                    "(_v) => " + ShowDeserdeNotNull(map.value, "_v")
                );
            case TyNamed named:
                if (AllGeneratedTypes.Contains(named.name))
                    return string.Format("{0}.fromJson(ctx, {1})", ShowType(named), expr);
                return string.Format("Deserde.{0}(ctx, {1})", named.name, expr);
            case TyFn fn:

                var argSig = string.Join(",", fn.args.Select(x => ShowType(x.typ) + " " + x.name));
                string fnBody;
                var serializedData = "[" + string.Join(",", fn.args.Select(x => ShowSerde(x.typ, x.name, x.nullable))) + "]";
                if (fn.ret is HasRet hasRet)
                {
                    var deserialized = ShowDeserde(hasRet.ret, "response", hasRet.nullable);
                    fnBody = string.Format(
                        "({0}) async {{ final response = await ctx.request(slot, {1}); return {2}; }}",
                        argSig,
                        serializedData,
                        deserialized
                    );
                    return string.Format("Deserde.signal({0}, (slot) => {1})", expr, fnBody);
                }
                else
                {
                    fnBody = string.Format("({0}) async {{ await ctx.request(slot, {1}); }}", argSig, serializedData);
                    return string.Format("Deserde.signal({0}, (slot) => {1})", expr, fnBody);
                }
            default:
                throw new UnreachableException();
        }
    }

    static string TypeMap(string typeName)
    {
        return typeName switch
        {
            "bool" => "bool",
            "int" => "int",
            "float" => "double",
            "str" => "String",
            "bytes" => "Uint8List",
            _ => typeName,
        };
    }
}