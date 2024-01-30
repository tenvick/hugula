using System;
using MemoryInfo.Internal;
using UnityEngine.Profiling;

namespace MemoryInfo
{
    public sealed class MemoryInfoPlugin : IDisposable
    {
        private readonly IPlatform _platform;

        public MemoryInfoPlugin()
        {
            _platform =

#if UNITY_EDITOR
                new MemoryInfoForEditor();
#elif UNITY_ANDROID
                new MemoryInfoForAndroid();
#elif UNITY_IOS
                new MemoryInfoForIOS();
#elif UNITY_STANDALONE_WIN
                new MemoryInfoForWindows();
#else
                null;
            // UnityEngine.Debug.unityLogger.LogError(GetType().Name, "This platform is not supported.");
#endif
        }

        ~MemoryInfoPlugin()
        {
            Dispose();
        }

        public void Dispose()
        {
            _platform.Dispose();
        }

        public MemoryInfo GetMemoryInfo()
        {
            if (_platform != null)
                return _platform.GetMemoryInfo();
            else
                return new MemoryInfo()
                {
                    TotalSize = (int)(Profiler.GetTotalReservedMemoryLong() / 1024),
                    UsedSize = (int)(Profiler.GetTotalAllocatedMemoryLong() / 1024),
                };
        }

        public bool LowMemory()
        {
            if (_platform != null)
                return _platform.LowMemory();
            else
                return false;
        }
    }
}
