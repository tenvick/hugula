// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula

using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Loader {

    public static class ABDelayUnloadManager {

        //delay unload time
        public static int delayClearFrameCount = 1; //0.1f;
        private static Dictionary<string, int> removeDic = new Dictionary<string, int> ();
        private static List<string> removeList = new List<string> ();
        private static List<string> deleteList = new List<string> ();

        internal static void Add (string key) {

            removeList.Remove (key);
            removeList.Add (key);
            removeDic[key] = Time.frameCount + delayClearFrameCount;
#if HUGULA_CACHE_DEBUG                
            Debug.LogFormat ("<color=#00ff00> ABDelayUnloadManager.add  key({0}),frame={1} </color>", key, Time.frameCount);
#endif
        }

        internal static void Remove (string key) {
            removeList.Remove (key);
            removeDic.Remove (key);
        }

        internal static void Update () {
            var frameCount = Time.frameCount;
            deleteList.Clear ();

            int count = removeList.Count;
            string Key;
            int delFrame;
            for (int i = 0; i < count; i++) {
                Key = removeList[i];
                if (removeDic.TryGetValue (Key, out delFrame) && frameCount >= delFrame) {
                    deleteList.Add (Key);
                    CacheManager.UnloadSecurity (Key);
                } else
                    break;
            }

            for (int i = 0; i < deleteList.Count; i++) {
                Remove (deleteList[i]);
            }

        }
    }
}