using System.Collections.Generic;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Hugula
{

    [SLua.CustomLuaClassAttribute]
    public class HugulaSetting : ScriptableObject
    {

        //分离扩展文件夹
        //"打包时候是否移除扩展文件夹"
        public bool spliteExtensionFolder
        {
            get
            {
#if HUGULA_SPLITE_ASSETBUNDLE
                return true;
#else
                return false;
#endif
            }
        }

        //"是否压缩streamingAssts下面的所有assetbundle 启动时候自动解压"
        public bool compressStreamingAssets
        {
            get
            {
#if SEVENZIP || HUGULA_COMPRESS_STREAMINGASSETS
                return true;
#else
                return false;
#endif
            }
        }

        public string httpVerHostRelease = string.Empty;
        public string httpVerHostDev = string.Empty;

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

        ///<summary >
        /// 增量文件名是否带crc编码,注意发布后就不要更改
        ///</summary >
        public bool appendCrcToFile
        {
            get
            {
#if HUGULA_APPEND_CRC
                return true;
#else
                return false;
#endif               
            }
        }

        //备份资源方式
        //[TooltipAttribute("资源目录组织方式 \r\n 0 OneResFolder [/res /v{d}/ver.u /ver.txt]  \r\n 1 VerResFolder[/v{d}/res /v{d}/ver.txt]")]
        // public CopyResType backupResType
        // {
        //     get
        //     {
        //         return CopyResType.OneResFolder;
        //     }
        // }

        //打包包涵的变体       
        [TooltipAttribute("默认包含的变体")]
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


    /// <summary>
    /// zip out put platform
    /// </summary>
    [Flags]
    public enum ZipPlatform
    {
        None = 1,
        Android = 2,
        iOS = 4,
        Standalonewindows = 8
    }


    /// <summary>
    /// zip file list config
    /// </summary>
    public class ZipConfigs : ScriptableObject
    {
        public List<string> zipFiles = new List<string>();

        public static List<string> GetZipFiles()
        {
            if (instance != null)
                return instance.zipFiles;
            else
                return null;
        }

        private static ZipConfigs _instance = null;
        public static ZipConfigs instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = Resources.Load<ZipConfigs>("zip_file_list");
                }
                return _instance;
            }
        }

#if UNITY_EDITOR
        public static ZipConfigs CreateInstance()
        {
            if (_instance == null)
            {
                _instance = HugulaSetting.CreateInstance<ZipConfigs>();
                AssetDatabase.CreateAsset(_instance, "Assets/Hugula/Config/Resources/zip_file_list.asset");
            }
            return _instance;
        }

        public static void Delete()
        {
            System.IO.File.Delete("Assets/Hugula/Config/Resources/zip_file_list.asset");
            _instance = null;
        }
#endif

    }
}