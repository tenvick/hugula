using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hugula
{

    [SLua.CustomLuaClassAttribute]
    public class HugulaSetting : ScriptableObject
    {

        //分离扩展文件夹
        [TooltipAttribute("打包时候是否移除扩展文件夹")]
        public bool spliteExtensionFolder = false;

        ///<summary >
        /// 增量文件名是否带crc编码,注意发布后就不要更改
        ///</summary >
        public bool appendCrcToFile
        {
            get
            {
                return false;
            }
        }

        [TooltipAttribute("增量文件名是否带crc编码,次属性只做提示用修改无效。 \r\n 如果要修改请手动修改appendCrcToFile的值， \r\n注意发布后就不要更改此属性。")]
        [SerializeField]
        private bool appendCrcToFileTips = false;

        //备份资源方式
        /// 0  OneResFolder  /res ver.txt
        /// 1 VerResFolder  /v{d}/res /v{d} md5.u
        [TooltipAttribute("资源目录组织方式 \r\n 0 OneResFolder [/res /v{d}/ver.u /ver.txt]  \r\n 1 VerResFolder[/v{d}/res /v{d}/ver.txt]")]
        public CopyResType backupResType = CopyResType.OneResFolder;

        //打包包涵的变体       
        [TooltipAttribute("默认包涵的变体")]
        public List<string> inclusionVariants = new List<string>();

        //所有变体
        [TooltipAttribute("项目中所有变体，使用[AssetBundle/Set AssetBundle Variants And Name]设置")]
        public List<string> allVariants = new List<string>();

        public void AddVariant(string key)
        {
            if (allVariants != null && !allVariants.Contains(key))
            {
                allVariants.Add(key);
            }
        }

        public bool ContainsVariant(string key)
        {
            if (allVariants != null && allVariants.Contains(key))
                return true;
            else
                return false;
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

    /// <summary>
    /// 更新包资源导出结构类型
    /// </summary>
    public enum CopyResType
    {
        /// <summary>
        /// 仅保持最新包res和最新版本文件 ver.txt
        /// </summary>
        OneResFolder,
        /// <summary>
        /// v{d}/res 放资源 和版本文件
        /// </summary>
        VerResFolder
    }
}