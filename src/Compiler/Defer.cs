using System;

namespace DTSchema.Compiler;

public struct Defer : IDisposable
{
    Action m_Dispose;
    public Defer(Action dispose) => m_Dispose = dispose;

    public void Dispose()
    {
        m_Dispose();
    }
}