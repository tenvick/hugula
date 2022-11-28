using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Hugula.ResUpdate;
using Hugula.Utils;
using Hugula;

namespace HugulaEditor.ResUpdate
{
    public class BuildConfig
    {
        //热更新文件输出目录名称
        public const string  UpdateResOutFolderName = "UpdateRes";
        //构建的bunlde name与读取路径
        public const string BuildBundlePathDataFileName = "BuildBundlePathData.json";
        //版本配置文件目录
        public const string VersionConfigPath = "Assets/Hugula/Config/Version";

        //输出的version.json在本地的存放位置
        public const string VersionLocalOutPath = "Assets/Hugula/Config/Resources";

        public readonly static byte[] OFFSET_DATA =  new byte[] { 0x32, 0x0f, 0x8d, 0xe9, 0x3b, 0x24, 0xa5, 0xd3, 0xf2, 0xd3, 0x64, 0x58, 0xb7, 0xae, 0x3f, 0x28 };
        public static byte[] GetOffsetData()
        {
            // var bytes = new byte[Common.BUNDLE_OFF_SET];
            // var b = bytes[0];

            // for(int i=0;i<Common.BUNDLE_OFF_SET;i++)
            // {
            //     bytes[i] = (byte)Random.Range(0,256);
            // }
            return OFFSET_DATA;
        }
        
        //更新资源输出目录
        private static string UpdateResOutPath
        {
            get
            {
                DirectoryInfo firstDir = new DirectoryInfo(Application.dataPath);
                string firstPath = Path.Combine(firstDir.Parent.Parent.FullName, UpdateResOutFolderName);
                return firstPath;
            }

        }

        /// <summary>
        /// 当前平台的热更新资源存放总目录
        /// </summary>
        public static string CurrentUpdateResOutPath
        {
            get{
                return Path.Combine(UpdateResOutPath, CUtils.platform);
            }
        }

        /// <summary>
        /// 本地缓存热更新资源存放总目录
        /// </summary>
        public static string LocalResourcesUpdateResOutPath
        {
            get{
                return "Assets/Config/Version/Resources";
                // return Path.Combine(UpdateResOutPath, CUtils.platform);
            }
        }

        /// <summary>
        /// 当前平台带版本号信息的热更新资源存放目录
        /// </summary>
        public static string UpdateResOutVersionPath
        {
            get
            {
                string updateOutPath = string.Format("{0}/{1}", CurrentUpdateResOutPath,Common.RES_VER_FOLDER);//" CodeVersion.APP_VERSION");
                return updateOutPath;
            }
        }

        /// <summary>
        /// 当前平台zip包资源存放目录
        /// </summary>
        public static string UpdatePackagesOutVersionPath
        {
            get
            {
                string outPath = string.Format("{0}/{1}", CurrentUpdateResOutPath,"packages");//" CodeVersion.APP_VERSION");
                return outPath;
            }
        }

         /// <summary>
        /// 当前平台addressables_content_state.bin存放目录
        /// </summary>
        public static string UpdateContentStateDataPath
        {
            get
            {
                string outPath = string.Format("{0}/{1}", CurrentUpdateResOutPath,"aas_bin");//" aas_bin");
                return outPath;
            }
        }
        

        private static UnityEditor.BuildTarget m_BuildTarget = UnityEditor.BuildTarget.NoTarget;
        /// <summary>
        /// 构建目标平台，用于资源copy
        /// </summary>
        public static UnityEditor.BuildTarget BuildTarget
        {
            get
            {
                if (m_BuildTarget == UnityEditor.BuildTarget.NoTarget)
                {
#if UNITY_IOS
                    m_BuildTarget = UnityEditor.BuildTarget.iOS;
#elif UNITY_ANDROID
                    m_BuildTarget = UnityEditor.BuildTarget.Android;
#elif UNITY_FACEBOOK || USE_GAMEROOM
                    m_BuildTarget = UnityEditor.BuildTarget.StandaloneWindows64;
#elif UNITY_METRO
                    m_BuildTarget = UnityEditor.BuildTarget.MetroPlayer;
#elif UNITY_OSX || UNITY_STANDALONE_OSX
                    m_BuildTarget = UnityEditor.BuildTarget.StandaloneOSX;
#else
                    m_BuildTarget = UnityEditor.BuildTarget.StandaloneWindows64;
#endif
                }

                return m_BuildTarget;
            }
            set
            {
                m_BuildTarget = value;
            }
        } //= ;

        public static string GetTmpZipName(string folderName)
        {
           return string.Format("{0}_{1}.zip", folderName, Application.version);
        }
    }

    ///<summary>
    /// 存放bundle构建位置信息
    ///</summary>
    [System.Serializable]
    public class BuildBundlePath
    {
        public string fileName;
        public string fullBuildPath;
        public uint crc;

        public BuildBundlePath(string name,string path,uint crc)
        {
            this.fileName = name;
            this.fullBuildPath = path;
            this.crc = crc;
        }
    }

    [System.Serializable]
    public class BuildBundlePathData
    {
        public List<BuildBundlePath> allBuildPath = new List<BuildBundlePath>();

        Dictionary<string,BuildBundlePath> m_AllBuildPathDic;
        private Dictionary<string,BuildBundlePath> allBuildPathDic
        {
            get
            {
                if(m_AllBuildPathDic==null)
                {
                    m_AllBuildPathDic = new Dictionary<string, BuildBundlePath>();
                    foreach(var f in allBuildPath)
                    {
                        m_AllBuildPathDic[f.fileName]=f;
                    }
                }

                return m_AllBuildPathDic;
            }
        }

        public void AddBuildBundlePath(string name,string path,uint crc)
        {
            var bbp = new BuildBundlePath(name,path,crc);
            allBuildPath.Add(bbp);
            allBuildPathDic[name] = bbp;
        }

        public BuildBundlePath GetBundleBuildPath(string name)
        {
            BuildBundlePath path ;
            if(allBuildPathDic.TryGetValue(name,out path ))
                return path;
            else
                return null;
        }
    

        public static void SerializeBuildBundlePathData(BuildBundlePathData buildBundlePathData)
        {
            string contents = JsonUtility.ToJson(buildBundlePathData);
            string tmpPath = EditorUtils.GetProjectTempPath();
            EditorUtils.CheckDirectory(tmpPath);
            string path = Path.Combine(tmpPath, BuildConfig.BuildBundlePathDataFileName);
            File.WriteAllText(path, contents);
            Debug.Log($"序列化文件保存地址:{path}");
            // Debug.Log(contents);
        }

        public static void ClearBuildBundlePathData()
        {
             string tmpPath = EditorUtils.GetProjectTempPath();
            string path = Path.Combine(tmpPath, BuildConfig.BuildBundlePathDataFileName);
            if(File.Exists(path))
            {
                File.Delete(path);
                Debug.Log($"删除文件地址:{path}");
            }
        }

        public static BuildBundlePathData ReadBuildBundlePathData()
        {
            string tmpPath = EditorUtils.GetProjectTempPath();
            string path = Path.Combine(tmpPath, BuildConfig.BuildBundlePathDataFileName);
            if(File.Exists(path))
            {
                return JsonUtility.FromJson<BuildBundlePathData>(File.ReadAllText(path));
            }
            else
            {
                Debug.LogError($"没有找到文件:{path}");
                return new BuildBundlePathData();
            }
        }
    }
}