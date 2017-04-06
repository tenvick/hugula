using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hugula {

    [SLua.CustomLuaClassAttribute]
    public class HugulaSetting : ScriptableObject {

        //分离扩展文件夹
        [TooltipAttribute("打包时候是否移除扩展文件夹")]
        public bool spliteExtensionFolder = false;

        //打包包涵的变体       
         [TooltipAttribute("默认包涵的变体")]
        public List<string> inclusionVariants = new List<string>();

        //所有变体
        [TooltipAttribute("项目中所有变体，使用[AssetBundle/Set AssetBundle Variants And Name]设置")]
        public List<string> allVariants = new List<string>();

        public void AddVariant(string key)
        {
            if(allVariants!=null && !allVariants.Contains(key))
            {
                allVariants.Add(key);
            }
        }

        public bool ContainsVariant(string key)
        {
            if(allVariants!=null && allVariants.Contains(key))
                return true;
            else
            return false;
        }

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