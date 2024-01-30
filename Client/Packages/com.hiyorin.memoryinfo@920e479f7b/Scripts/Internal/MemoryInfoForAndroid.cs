#if UNITY_ANDROID
using System;
using UnityEngine;

namespace MemoryInfo.Internal
{
    public sealed class MemoryInfoForAndroid : IPlatform
    {
        private readonly AndroidJavaObject _activityManager;

        private readonly AndroidJavaObject _memoryInfo;

        private readonly AndroidJavaObject [] _memoryInfos;

        public MemoryInfoForAndroid()
        {
            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            using (AndroidJavaClass contextClass = new AndroidJavaClass("android.content.Context"))
            using (AndroidJavaClass processClass = new AndroidJavaClass("android.os.Process"))
            using (AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            using (AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext"))
            {
                _activityManager = context.Call<AndroidJavaObject>("getSystemService", contextClass.GetStatic<string>("ACTIVITY_SERVICE"));
                _memoryInfo = new AndroidJavaObject("android.app.ActivityManager$MemoryInfo");
                int[] pids = { processClass.CallStatic<int>("myPid") };
                _memoryInfos = _activityManager.Call<AndroidJavaObject[]>("getProcessMemoryInfo", pids);

                _activityManager.Call("getMemoryInfo", _memoryInfo);

            }
        }

        ~MemoryInfoForAndroid()
        {
            Dispose();
        }

        public void Dispose()
        {
            foreach (var memoryInfo in _memoryInfos)
                memoryInfo.Dispose();
            _memoryInfo.Dispose();
            _activityManager.Dispose();
        }

        MemoryInfo IPlatform.GetMemoryInfo()
        {

            int usedSize = 0;
            foreach (var memoryInfo in _memoryInfos)
            {
                usedSize += memoryInfo.Call<int>("getTotalPss");
            }

            return new MemoryInfo() {
                TotalSize = (int)(_memoryInfo.Get<long>("totalMem") / 1024f), //threshold  totalMem
                UsedSize = usedSize,
                AvailMem = (int)(_memoryInfo.Get<long>("availMem") / 1024f),
            };
        }
            
        /// <summary>
        /// 是否低内存
        /// </summary>
        /// <returns></returns>
        public bool LowMemory()
        {
            return _memoryInfo.Get<bool>("lowMemory");
        }



    }
}
#endif
