grammar DTSchema;
options { language = CSharp; }
@parser::members {
public static Sort Sort (string name,System.Collections.Generic.List<Ctor> variants)
{
    return (Sort) new Sort(name,variants);
}
public static Ctor Ctor (string name,System.Collections.Generic.List<Field> fields)
{
    return (Ctor) new Ctor(name,fields);
}
public static Field Field (string name,Ty typ,bool nullable)
{
    return (Field) new Field(name,typ,nullable);
}
public static Ty TyTuple (System.Collections.Generic.List<Ty> elts)
{
    return (Ty) new TyTuple(elts);
}
public static Ty TyNamed (string name)
{
    return (Ty) new TyNamed(name);
}
public static Ty TyMap (Ty key,Ty value)
{
    return (Ty) new TyMap(key,value);
}
public static Ty TyList (Ty elt)
{
    return (Ty) new TyList(elt);
}
public static Ty TyJSON ()
{
    return (Ty) new TyJSON();
}
public static Ty TyFn (System.Collections.Generic.List<Field> args,Ret ret)
{
    return (Ty) new TyFn(args,ret);
}
public static Ty TyEnum (string name)
{
    return (Ty) new TyEnum(name);
}
public static Ret NoRet ()
{
    return (Ret) new NoRet();
}
public static Ret HasRet (Ty ret,bool nullable)
{
    return (Ret) new HasRet(ret,nullable);
}
public static Def DefSort (Pos pos,Sort sort)
{
    return (Def) new DefSort(pos,sort);
}
public static Def DefRecord (Pos pos,Ctor ctor)
{
    return (Def) new DefRecord(pos,ctor);
}
public static Def DefExtern (Pos pos,string name)
{
    return (Def) new DefExtern(pos,name);
}
public static Def DefEnum (Pos pos,string name,System.Collections.Generic.List<string> cases)
{
    return (Def) new DefEnum(pos,name,cases);
}
}
start returns [System.Collections.Generic.List<Def> result]: v=start__y_ EOF { $result = _localctx.v.result; };
list_o_definition_p_ returns [System.Collections.Generic.List<Def> result]
: list_o_definition_p__0__1=list_o_definition_p_ list_o_definition_p__0__2=definition { $result = (System.Collections.Generic.List<Def>) appendList<Def>((System.Collections.Generic.List<Def>) _localctx.list_o_definition_p__0__1.result, (Def) _localctx.list_o_definition_p__0__2.result);
            }
| list_o_definition_p__1__1=definition { $result = new System.Collections.Generic.List<Def> { _localctx.list_o_definition_p__1__1.result };
            }
;
start__y_ returns [System.Collections.Generic.List<Def> result]
: start__y__0__1=list_o_definition_p_ { $result = _localctx.start__y__0__1.result;
            }
;
definition returns [Def result]
: definition_0__1=def_t_record { $result = _localctx.definition_0__1.result;
            }
| definition_1__1=def_t_sort { $result = _localctx.definition_1__1.result;
            }
| definition_2__1=def_t_extern { $result = _localctx.definition_2__1.result;
            }
| definition_3__1=def_t_enum { $result = _localctx.definition_3__1.result;
            }
;
enum_t_case returns [string result]
: '|' enum_t_case_0__2=IDENTIFIER { $result = (string) lexeme((IToken) _localctx.enum_t_case_0__2);
            }
;
list_o_enum_t_case_p_ returns [System.Collections.Generic.List<string> result]
: list_o_enum_t_case_p__0__1=list_o_enum_t_case_p_ list_o_enum_t_case_p__0__2=enum_t_case { $result = (System.Collections.Generic.List<string>) appendList<string>((System.Collections.Generic.List<string>) _localctx.list_o_enum_t_case_p__0__1.result, (string) _localctx.list_o_enum_t_case_p__0__2.result);
            }
| list_o_enum_t_case_p__1__1=enum_t_case { $result = new System.Collections.Generic.List<string> { _localctx.list_o_enum_t_case_p__1__1.result };
            }
;
def_t_enum returns [Def result]
: def_t_enum_0__1='enum' def_t_enum_0__2=IDENTIFIER '=' def_t_enum_0__4=list_o_enum_t_case_p_ { $result = (Def) DefEnum((Pos) (Pos) getPos((IToken) _localctx.def_t_enum_0__1), (string) (string) lexeme((IToken) _localctx.def_t_enum_0__2), (System.Collections.Generic.List<string>) _localctx.def_t_enum_0__4.result);
            }
;
def_t_extern returns [Def result]
: def_t_extern_0__1='extern' 'type' def_t_extern_0__3=IDENTIFIER { $result = (Def) DefExtern((Pos) (Pos) getPos((IToken) _localctx.def_t_extern_0__1), (string) (string) lexeme((IToken) _localctx.def_t_extern_0__3));
            }
;
def_t_record returns [Def result]
: def_t_record_0__1='type' def_t_record_0__2=ctor { $result = (Def) DefRecord((Pos) (Pos) getPos((IToken) _localctx.def_t_record_0__1), (Ctor) _localctx.def_t_record_0__2.result);
            }
;
optional_t_annotation returns [bool result]
: '?' ':' { $result = true;
            }
| ':' { $result = false;
            }
;
fieldname returns [string result]
: fieldname_0__1=IDENTIFIER { $result = (string) lexeme((IToken) _localctx.fieldname_0__1);
            }
| 'fn' { $result = "fn";
            }
| 'type' { $result = "type";
            }
| 'extern' { $result = "extern";
            }
| 'enum' { $result = "enum";
            }
| 'json' { $result = "json";
            }
;
field returns [Field result]
: field_0__1=fieldname field_0__2=optional_t_annotation field_0__3=def_t_typ { $result = (Field) Field((string) _localctx.field_0__1.result, (Ty) _localctx.field_0__3.result, (bool) _localctx.field_0__2.result);
            }
;
ctor returns [Ctor result]
: ctor_0__1=IDENTIFIER '(' ctor_0__3=seplist_o__i__s__i__s_field_p_ ')' { $result = (Ctor) Ctor((string) (string) lexeme((IToken) _localctx.ctor_0__1), (System.Collections.Generic.List<Field>) _localctx.ctor_0__3.result);
            }
| ctor_1__1=IDENTIFIER { $result = (Ctor) Ctor((string) (string) lexeme((IToken) _localctx.ctor_1__1), (System.Collections.Generic.List<Field>) new System.Collections.Generic.List<Field> {  });
            }
;
constructor returns [Ctor result]
: '|' constructor_0__2=ctor { $result = _localctx.constructor_0__2.result;
            }
;
list_o_constructor_p_ returns [System.Collections.Generic.List<Ctor> result]
: list_o_constructor_p__0__1=list_o_constructor_p_ list_o_constructor_p__0__2=constructor { $result = (System.Collections.Generic.List<Ctor>) appendList<Ctor>((System.Collections.Generic.List<Ctor>) _localctx.list_o_constructor_p__0__1.result, (Ctor) _localctx.list_o_constructor_p__0__2.result);
            }
| list_o_constructor_p__1__1=constructor { $result = new System.Collections.Generic.List<Ctor> { _localctx.list_o_constructor_p__1__1.result };
            }
;
def_t_sort returns [Def result]
: def_t_sort_0__1='type' def_t_sort_0__2=IDENTIFIER '=' def_t_sort_0__4=list_o_constructor_p_ { $result = (Def) DefSort((Pos) (Pos) getPos((IToken) _localctx.def_t_sort_0__1), (Sort) (Sort) Sort((string) (string) lexeme((IToken) _localctx.def_t_sort_0__2), (System.Collections.Generic.List<Ctor>) _localctx.def_t_sort_0__4.result));
            }
;
seplist_o__i__s__i__s_def_t_typ_p_ returns [System.Collections.Generic.List<Ty> result]
: seplist_o__i__s__i__s_def_t_typ_p__0__1=seplist_o__i__s__i__s_def_t_typ_p_ ',' seplist_o__i__s__i__s_def_t_typ_p__0__3=def_t_typ { $result = (System.Collections.Generic.List<Ty>) appendList<Ty>((System.Collections.Generic.List<Ty>) _localctx.seplist_o__i__s__i__s_def_t_typ_p__0__1.result, (Ty) _localctx.seplist_o__i__s__i__s_def_t_typ_p__0__3.result);
            }
| seplist_o__i__s__i__s_def_t_typ_p__1__1=def_t_typ { $result = new System.Collections.Generic.List<Ty> { _localctx.seplist_o__i__s__i__s_def_t_typ_p__1__1.result };
            }
;
seplist_o__i__s__i__s_field_p_ returns [System.Collections.Generic.List<Field> result]
: seplist_o__i__s__i__s_field_p__0__1=seplist_o__i__s__i__s_field_p_ ',' seplist_o__i__s__i__s_field_p__0__3=field { $result = (System.Collections.Generic.List<Field>) appendList<Field>((System.Collections.Generic.List<Field>) _localctx.seplist_o__i__s__i__s_field_p__0__1.result, (Field) _localctx.seplist_o__i__s__i__s_field_p__0__3.result);
            }
| seplist_o__i__s__i__s_field_p__1__1=field { $result = new System.Collections.Generic.List<Field> { _localctx.seplist_o__i__s__i__s_field_p__1__1.result };
            }
;
emptyseplist_o__i__s__i__s_field_p_ returns [System.Collections.Generic.List<Field> result]
: emptyseplist_o__i__s__i__s_field_p__0__1=seplist_o__i__s__i__s_field_p_ { $result = _localctx.emptyseplist_o__i__s__i__s_field_p__0__1.result;
            }
|  { $result = new System.Collections.Generic.List<Field> {  };
            }
;
def_t_typ returns [Ty result]
: '[' def_t_typ_0__2=seplist_o__i__s__i__s_def_t_typ_p_ ']' { $result = (Ty) select<Ty>((bool) (bool) notSingular<Ty>((System.Collections.Generic.List<Ty>) _localctx.def_t_typ_0__2.result), (Ty) (Ty) TyTuple((System.Collections.Generic.List<Ty>) _localctx.def_t_typ_0__2.result), (Ty) (Ty) TyList((Ty) (Ty) firstElt<Ty>((System.Collections.Generic.List<Ty>) _localctx.def_t_typ_0__2.result)));
            }
| '{' def_t_typ_1__2=def_t_typ ':' def_t_typ_1__4=def_t_typ '}' { $result = (Ty) TyMap((Ty) _localctx.def_t_typ_1__2.result, (Ty) _localctx.def_t_typ_1__4.result);
            }
| def_t_typ_2__1=IDENTIFIER { $result = (Ty) TyNamed((string) (string) lexeme((IToken) _localctx.def_t_typ_2__1));
            }
| 'fn' '[' def_t_typ_3__3=emptyseplist_o__i__s__i__s_field_p_ ']' '=>' def_t_typ_3__6=nullable_t_ret { $result = (Ty) TyFn((System.Collections.Generic.List<Field>) _localctx.def_t_typ_3__3.result, (Ret) _localctx.def_t_typ_3__6.result);
            }
| 'fn' '[' def_t_typ_4__3=emptyseplist_o__i__s__i__s_field_p_ ']' { $result = (Ty) TyFn((System.Collections.Generic.List<Field>) _localctx.def_t_typ_4__3.result, (Ret) (Ret) NoRet());
            }
| 'enum' def_t_typ_5__2=IDENTIFIER { $result = (Ty) TyEnum((string) (string) lexeme((IToken) _localctx.def_t_typ_5__2));
            }
| 'json' { $result = (Ty) TyJSON();
            }
;
nullable_t_ret returns [Ret result]
: nullable_t_ret_0__1=def_t_typ '?' { $result = (Ret) HasRet((Ty) _localctx.nullable_t_ret_0__1.result, (bool) true);
            }
| nullable_t_ret_1__1=def_t_typ { $result = (Ret) HasRet((Ty) _localctx.nullable_t_ret_1__1.result, (bool) false);
            }
;
fragment UNICODE : ([\u00FF-\uD7FF] | [\uDFFF-\uFFFF]) ;
fragment LETTER : ([\u0061-\u007A] | [\u0041-\u005A]) ;
fragment DIGIT : [\u0030-\u0039] ;
fragment INT : DIGIT+ ;
fragment IDENTIFIER_HEAD : (LETTER | UNICODE | '_') ;
fragment IDENTIFIER_FOLLOW : (IDENTIFIER_HEAD | DIGIT) ;
IDENTIFIER : IDENTIFIER_HEAD IDENTIFIER_FOLLOW* '!'? ;
LINE_COMMENT : '-' '-' ~'\n'* '\n' -> channel(HIDDEN);
SPACE : (' ' | '\t' | '\r' | '\n') -> channel(HIDDEN);
