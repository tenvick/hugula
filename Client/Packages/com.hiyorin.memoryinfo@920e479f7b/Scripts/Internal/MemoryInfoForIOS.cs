#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace MemoryInfo.Internal
{
    public sealed class MemoryInfoForIOS : IPlatform
    {
        [DllImport("__Internal")]
        private static extern uint _GetUsedMemorySize();

        [DllImport("__Internal")]
        private static extern uint _GetFreeMemorySize();

        [DllImport("__Internal")]
        private static extern uint _GetTotalMemorySize();

        void IDisposable.Dispose()
        {
            
        }

        MemoryInfo IPlatform.GetMemoryInfo()
        {;
            return new MemoryInfo() {
                UsedSize = (int)(_GetUsedMemorySize() / 1024),
                TotalSize = (int)(_GetTotalMemorySize() / 1024),
                AvailMem = (int)(_GetFreeMemorySize() / 1024),
            };
        }

        /// <summary>
        /// 可用内存小于20%
        /// </summary>
        /// <returns></returns>
        public bool LowMemory()
        {
           var f = _GetFreeMemorySize();
           float t = _GetTotalMemorySize();
            if( t >0 && f/t<=0.2f )
            {
                return true;
            }
            else
            {
                return false;
            }           
        }
    }
}
#endif
