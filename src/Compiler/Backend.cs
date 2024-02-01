namespace DTSchema.Compiler;

public abstract class Backend
{
    public readonly TypeStore Store;
    public Backend(TypeStore store)
    {
        Store = store;
    }
    public abstract string CodeGen(TypeStore store);
}