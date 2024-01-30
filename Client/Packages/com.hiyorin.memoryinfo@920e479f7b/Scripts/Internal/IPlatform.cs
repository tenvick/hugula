using System;

namespace MemoryInfo.Internal
{
    public interface IPlatform : IDisposable
    {
        MemoryInfo GetMemoryInfo();

        bool LowMemory();
    }
}
