#if UNITY_STANDALONE_WIN
using System;
using UnityEngine.Profiling;
using System.Diagnostics;
using System.Runtime.InteropServices;
using UnityEngine.Profiling;

namespace MemoryInfo.Internal
{
    public sealed class MemoryInfoForWindows : IPlatform
    {
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            //系统内存总量
            public ulong ullTotalPhys;
            //系统可用内存
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        //extern  调用 其他平台 dll 关键字
        [DllImport("kernel32.dll")]
        static extern void GlobalMemoryStatus(ref MEMORYSTATUSEX lpBuff);


        /// <summary>
        /// 获取可用内存
        /// PC
        /// </summary>
        /// <returns></returns>
        ulong GetWinAvailMemory()
        {
            MEMORYSTATUSEX ms = new MEMORYSTATUSEX();
            ms.dwLength = 64;//(uint)Marshal.SizeOf(typeof(MEMORYSTATUSEX));

            GlobalMemoryStatus(ref ms);
            return ms.ullAvailPhys;
        }

        /// <summary>
        /// 获取总内存
        /// PC
        /// </summary>
        /// <returns></returns>
        ulong GetWinTotalMemory()
        {
            MEMORYSTATUSEX ms = new MEMORYSTATUSEX();
            ms.dwLength = 64;
            GlobalMemoryStatus(ref ms);
            return ms.ullTotalPhys;
        }

#if !UNITY_EDITOR
        Process currentProcess = Process.GetCurrentProcess();
#endif
        /// <summary>
        /// 返回应用使用的内存
        /// PC
        /// </summary>
        /// <returns></returns>
        long GetWinUsedMemory()
        {
#if !UNITY_EDITOR
            currentProcess.Refresh();
            return currentProcess.WorkingSet64;
#else
            return Profiler.GetTotalReservedMemoryLong()+ Profiler.GetTotalAllocatedMemoryLong();
#endif
        }
        void IDisposable.Dispose()
        {

        }

        MemoryInfo IPlatform.GetMemoryInfo()
        {
            return new MemoryInfo()
            {
                TotalSize = (int)(GetWinTotalMemory() / 1024),
                UsedSize = (int)(GetWinUsedMemory() / 1024),
                AvailMem = (int)(GetWinAvailMemory() / 1024),
            };
        }

        bool IPlatform.LowMemory()
        {
            return false;
        }
    }
}
#endif
