using Antlr4.Runtime;
using System.Collections.Generic;
using System;
namespace DTSchema{
public partial interface Def {  }
public partial record DefExtern(Pos pos,string name) : Def;
public partial record DefRecord(Pos pos,Ctor ctor) : Def;
public partial record DefSort(Pos pos,Sort sort) : Def;
public partial interface Ret {  }
public partial record HasRet(Ty ret,bool nullable) : Ret;
public partial record NoRet() : Ret;
public partial interface Ty {  }
public partial record TyFn(System.Collections.Generic.List<Field> args,Ret ret) : Ty;
public partial record TyList(Ty elt) : Ty;
public partial record TyMap(Ty key,Ty value) : Ty;
public partial record TyNamed(string name) : Ty;
public partial record TyTuple(System.Collections.Generic.List<Ty> elts) : Ty;
public partial record Field(string name,Ty typ,bool nullable);
public partial record Ctor(string name,System.Collections.Generic.List<Field> fields);
public partial record Sort(string name,System.Collections.Generic.List<Ctor> variants);
}
