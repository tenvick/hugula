// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula

using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Loader
{

    public static class ABDelayUnloadManager
    {

        //delay unload time
        public static int delayClearFrameCount = 1;//0.1f;
        private static Dictionary<string, int> removeDic = new Dictionary<string, int>();
        private static List<string> dependencies = new List<string>();

        private static List<string> deleteList = new List<string>();

        internal static void Add(string key)
        {

            if (removeDic.ContainsKey(key))
                removeDic.Remove(key);
#if HUGULA_CACHE_DEBUG                
            Debug.LogFormat("<color=#00ff00> ABDelayUnloadManager.add  key({0}),frame={1} </color>", key, Time.frameCount);
#endif
            removeDic.Add(key, Time.frameCount + delayClearFrameCount);
        }

        ///<summary>
        /// 依赖项目
        ///<summary>
        internal static void AddDep(string key)
        {

            if (!dependencies.Contains(key))
                dependencies.Add(key);
#if HUGULA_CACHE_DEBUG                
            Debug.LogFormat("<color=#00ff00> ABDelayUnloadManager.AddDep  key({0}),frame={1} </color>", key, Time.frameCount);
#endif
        }
        internal static void Remove(string key)
        {
            if (removeDic.ContainsKey(key))
                removeDic.Remove(key);
        }

        internal static void Update()
        {
            var frameCount = Time.frameCount;
            deleteList.Clear();

            var items = removeDic.GetEnumerator();

            while (items.MoveNext())
            {
                var kv = items.Current;
                if (frameCount >= kv.Value)
                {
                    deleteList.Add(kv.Key);
                    CacheManager.UnloadSecurity(kv.Key);
                }
            }

            if (dependencies.Count > 0)
            {
                foreach (var item in dependencies)
                {
                    Add(item);
                }
                dependencies.Clear();
            }


            for (int i = 0; i < deleteList.Count; i++)
            {
                removeDic.Remove(deleteList[i]);
            }

        }
    }
}