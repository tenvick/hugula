// Copyright (c) 2017 hugula
// direct https://github.com/tenvick/hugula

using System.Collections.Generic;
using UnityEngine;
using Hugula.Utils;

namespace Hugula.Loader
{

    [SLua.CustomLuaClass]
    public static class ABDelayUnloadManager
    {

        //delay unload time
        public static int delayClearFrameCount = 1;//0.1f;
        private static Dictionary<int, int> removeDic = new Dictionary<int, int>();

        private static List<int> deleteList = new List<int>();

        internal static void Add(int keyhashcode)
        {

            if (removeDic.ContainsKey(keyhashcode))
                removeDic.Remove(keyhashcode);
#if HUGULA_CACHE_DEBUG                
            Debug.LogFormat("<color=#00ff00> ABDelayUnloadManager.add  keyhashcode({0}),frame={1} </color>",keyhashcode,Time.frameCount);
#endif
            removeDic.Add(keyhashcode, Time.frameCount + delayClearFrameCount);
        }

        internal static void AddDependenciesReferCount(CacheData cache)
        {
            if (cache != null)
            {
                int keyhash = 0;
                int[] alldep = cache.dependencies;
#if HUGULA_CACHE_DEBUG
                HugulaDebug.FilterLogFormat(cache.assetBundleKey, "<color=#ffff00> CheckRemove  abName({0})'s allDependencies from ABDelayUnloadManager , Dependencies.count = {1} frameCount={2} </color>", cache.assetBundleKey, alldep == null ? 0 : alldep.Length, UnityEngine.Time.frameCount);
#endif          
                if (alldep != null)
                {
                    CacheData pCache = null;
                    for (int i = 0; i < alldep.Length; i++)
                    {
                        keyhash = alldep[i];
                        pCache = CacheManager.TryGetCache(keyhash);
                        if (pCache != null)
                        {
                            pCache.count++;
#if HUGULA_CACHE_DEBUG
                            HugulaDebug.FilterLogFormat(pCache.assetBundleKey, " <color=#fcfcfc>check add  (assetBundle={0},parent={1},hash={2},count={3}) frameCount={4}</color>", pCache.assetBundleKey,cache.assetBundleKey ,keyhash, pCache.count, UnityEngine.Time.frameCount);
#endif
                            if (removeDic.ContainsKey(keyhash)) //如果在回收列表
                            {
                                removeDic.Remove(keyhash);
                                // if (pCache.count == 1) //相加后为1，在回收列表，需要对所有依赖项目引用+1
                                AddDependenciesReferCount(pCache);
                            }

                           
                        }
                    }//end for
                }// end if

            }// end cache

        }

        internal static bool CheckRemove(int keyhashcode)
        {
            if (removeDic.ContainsKey(keyhashcode))
            {
                removeDic.Remove(keyhashcode);
                CacheData cache = CacheManager.TryGetCache(keyhashcode);
                AddDependenciesReferCount(cache);
 #if HUGULA_LOADER_DEBUG
                HugulaDebug.FilterLogFormat(cache.assetBundleKey, "CheckRemove AssetBundle(key={0},hash={1}),frameCount={2}</color>", cache.assetBundleKey, cache.assetHashCode, Time.frameCount);
#endif
                return true;
            }
            else
                return false;
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
                    CacheManager.Unload(kv.Key);
                }
            }

            for (int i = 0; i < deleteList.Count; i++)
            {
                removeDic.Remove(deleteList[i]);
            }

        }
    }
}