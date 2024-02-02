using System.Collections.Generic;

namespace DTSchema.Compiler;

public abstract class Backend
{
    public readonly TypeStore Store;

    protected readonly HashSet<string> AllGeneratedTypes = [];
    public Backend(TypeStore store)
    {
        Store = store;
    }
    public abstract string CodeGen(TypeStore store);
}