#if UNITY_EDITOR
using System;
using UnityEngine.Profiling;

namespace MemoryInfo.Internal
{
    public sealed class MemoryInfoForEditor : IPlatform
    {
        void IDisposable.Dispose()
        {
            
        }

        MemoryInfo IPlatform.GetMemoryInfo()
        {
            return new MemoryInfo() {
                TotalSize = (int)(Profiler.GetTotalReservedMemoryLong() / 1024),
                UsedSize = (int)(Profiler.GetTotalAllocatedMemoryLong() / 1024),
            };
        }

        bool IPlatform.LowMemory()
        {
            return false;
        }
    }
}
#endif
