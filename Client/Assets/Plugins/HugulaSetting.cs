using System.Collections;
using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hugula {

    [SLua.CustomLuaClassAttribute]
    public class HugulaSetting : ScriptableObject {

        //分离扩展文件夹
        public bool spliteExtensionFolder = false;

        private static HugulaSetting _instance = null;
        public static HugulaSetting instance {
            get {
                if (_instance == null) {
                    _instance = Resources.Load<HugulaSetting> ("hugulasetting");
#if UNITY_EDITOR
                    if (_instance == null) {
                        _instance = HugulaSetting.CreateInstance<HugulaSetting> ();
                        AssetDatabase.CreateAsset (_instance, "Assets/Hugula/Config/Resources/hugulasetting.asset");
                    }
#endif

                }
                return _instance;
            }
        }

#if UNITY_EDITOR && !SLUA_STANDALONE
        [MenuItem ("Hugula/Setting")]
        internal static void Open () {
            Selection.activeObject = instance;
        }
#endif
    }
}