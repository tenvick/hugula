using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hugula
{

    public class HugulaSetting : ScriptableObject
    {
        public string httpVerHostRelease = string.Empty;
        public string httpVerHostDev = string.Empty;
        [Tooltip("是否启用google aab fast模式 如果启用走google play方式下载fast包")]
        public bool aabFastEnable = false;
        public string httpVerHost
        {
            get
            {
#if HUGULA_RELEASE
                return httpVerHostRelease;
#else
                return httpVerHostDev;
#endif
            }
        }

        private static HugulaSetting _instance = null;
        public static HugulaSetting instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<HugulaSetting>("hugulasetting");
#if UNITY_EDITOR
                    if (_instance == null)
                    {
                        _instance = HugulaSetting.CreateInstance<HugulaSetting>();
                        AssetDatabase.CreateAsset(_instance, "Assets/Hugula/Config/Resources/hugulasetting.asset");
                    }
#endif

                }
                return _instance;
            }
        }

#if UNITY_EDITOR && !SLUA_STANDALONE
        [MenuItem("Hugula/Setting")]
        internal static void Open()
        {
            Selection.activeObject = instance;
        }
#endif
    }
}