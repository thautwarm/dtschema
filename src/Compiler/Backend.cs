using System.Collections.Generic;

namespace DTSchema.Compiler;

public abstract class Backend(TypeStore store)
{
    public readonly TypeStore Store = store;

    protected readonly HashSet<string> AllGeneratedTypes = [];

    public abstract string CodeGen(TypeStore store);
}