// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;

namespace Hugula.ResUpdate
{
    [XLua.LuaCallCSharp]
    public static class FileManifestManager
    {
        #region  static field
        /// <summary>
        /// 本地streamingAssets目录的manifest
        /// </summary>
        public static List<FolderManifest> streamingFolderManifest;

        /// <summary>
        /// 本地已经更新的文件列表
        /// </summary>
        public static FolderManifest updateFolderManifest;

        /// <summary>
        /// 本地更新的目录包括firstpackage下载或者其他方式下载的更新包目录
        /// </summary>
        public static List<FolderManifest> persistentFolderManifest;

        /// <summary>
        /// 需要下载的远端folderManifest文件
        /// </summary>
        public static List<FolderManifest> remoteFolderManifest;

        /// <summary>
        /// 本地最新版本号
        /// </summary>
        public static string localVersion
        {
            get
            {
                if (updateFolderManifest != null)
                    return updateFolderManifest.version;
                else
                    return Application.version;
            }
        }

        /// <summary>
        /// 文件地址映射
        /// </summary>
        static Dictionary<string, string> locationIdPath = new Dictionary<string, string>();
        #endregion

        #region  static method
        /// <summary>
        /// 检查持久化目录的文件crc码
        /// </summary>
        public static bool CheckPersistentCrc(FileResInfo fInfo)
        {

            if (fInfo.state != FileInfoState.Success)
            {
                var fname = Path.GetFileName(fInfo.name);
                var url = CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), fname);
                var crc = CrcCheck.GetLocalFileCrc(url, out var len);
                if (len == 0)
                {
                    fInfo.state = FileInfoState.NotExist;
                }
                else if ((crc == 0 && len == fInfo.size) ||
                (crc != 0 && crc == fInfo.crc32))
                {
                    fInfo.state = FileInfoState.Success;
                }
                else
                {
                    fInfo.state = FileInfoState.Fail;
                }
            }
            return fInfo.state == FileInfoState.Success;
        }

        /// <summary>
        /// 读取Streaming中的folderManifest
        /// </summary>
        public static void LoadStreamingFolderManifests(System.Action onComplete)
        {
            var fileListName = Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME;
            var url = CUtils.PathCombine(CUtils.GetRealStreamingAssetsPath(), fileListName);

            url = CUtils.GetAndroidABLoadPath(url);
            AssetBundle ab = AssetBundle.LoadFromFile(url);
            if (ab != null)
            {
                var assets = ab.LoadAllAssets<FolderManifest>();
                FileManifestManager.streamingFolderManifest = new List<FolderManifest>(assets);
                if (assets.Length == 0)
                    Debug.LogError("there is no streamingManifest at " + url);
#if !HUGULA_NO_LOG || UNITY_EDITOR
                Debug.LogFormat("Load streamingManifest {0} is done !\r\n ManifestManager.streamingManifest.count = {1}", url, FileManifestManager.streamingFolderManifest.Count);
                for (int i = 0; i < assets.Length; i++)
                {
                    Debug.Log(assets[i].ToString());
                }
#endif
                ab.Unload(false);
            }
#if UNITY_EDITOR
            else
                Debug.LogWarning("there is no folderManifest in StreamingAssetsPath use (Addressabkes Groups /Build/New Build/Hot Resource Update) to build ");
#endif

            if (onComplete != null)
            {
                onComplete();
            }
        }

        /// <summary>
        /// 读取Persistent中的所有folderManifest
        /// </summary>
        public static void LoadPersistentFolderManifest(System.Action onComplete)
        {
            var url = CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);//首先查找热更新目录
            AssetBundle ab = null;
            if (FileHelper.FileExists(url) && (ab = AssetBundle.LoadFromFile(url)) != null)
            {
                // updateFolderManifest = FolderManifestRuntionExtention.Create("update"); //每次都是新的对象

                var persistentFolderManifest = new List<FolderManifest>(ab.LoadAllAssets<FolderManifest>());
                FileManifestManager.persistentFolderManifest = persistentFolderManifest;
                if (persistentFolderManifest.Count == 0)
                    Debug.LogError("there is no  persistentFolderManifest  at  " + url);
                FolderManifest item;
                for (int i = 0; i < persistentFolderManifest.Count; i++)
                {
                    item = persistentFolderManifest[i];
                    //TODO:如果先下载热更新在下载依赖包会出现旧的依赖包覆盖新的热更新问题。
                    if(UpdateStreamingFolderManifest(item))//更新本地版本到最新，方便接下来的热更新对比
                    {
                        GenUpdatePackageTransform(item);//如果版本正确重定向地址
                    }
                }

#if !HUGULA_NO_LOG || UNITY_EDITOR
                Debug.LogFormat("Load persistentFolderManifest {0} is done !\r\n ManifestManager.persistentFolderManifest.count = {1}", url, FileManifestManager.persistentFolderManifest.Count);
                for (int i = 0; i < persistentFolderManifest.Count; i++)
                {
                    Debug.Log(persistentFolderManifest[i].ToString());
                }
#endif
                ab.Unload(false);
            }

            if (onComplete != null)
            {
                onComplete();
            }
        }

        /// <summary>
        ///  构建zip文件地址映射 覆盖优先级靠后
        /// </summary>
        public static void GenZipPackageTransform(FolderManifest folderPackage)
        {
            var allFiles = folderPackage.allFileInfos;
            FileResInfo finfo;
            var isZipDone = folderPackage.isZipDone;
            if (isZipDone)
            {
                var zipOutPath = folderPackage.GetZipOutFolderPath();
                for (int i = 0; i < allFiles.Count; i++)
                {
                    finfo = allFiles[i];
                    if (!locationIdPath.ContainsKey(finfo.name))
                        locationIdPath[finfo.name] = zipOutPath;
                }
            }
        }

        /// <summary>
        ///  构建热更新包文件地址映射
        /// </summary>
        public static void GenUpdatePackageTransform(FolderManifest folderPackage)
        {
            var allFiles = folderPackage.allFileInfos;
            FileResInfo finfo;
            for (int i = 0; i < allFiles.Count; i++)
            {
                finfo = allFiles[i];
                locationIdPath[finfo.name] = CUtils.realPersistentDataPath;
            }
        }



        /// <summary>
        /// set  Addressables.InternalIdTransformFunc function.
        /// </summary>
        /// <value>The instance.</value>
        public static void OverrideInternalIdTransformFunc()
        {
            Addressables.InternalIdTransformFunc = OverrideLocationURL;
        }

        /// <summary>
        /// check find files in PersistentDataPath
        /// </summary>
        public static string OverrideLocationURL(IResourceLocation location)
        {
            if (location.ResourceType == typeof(IAssetBundleResource))
            {
                string bundleName = Path.GetFileName(location.InternalId);
                string path = null;
                if (locationIdPath.TryGetValue(bundleName, out path))
                {
                    var id = CUtils.PathCombine(path, bundleName);
#if !HUGULA_NO_LOG
                    Debug.Log($"from cache {id} exist:{File.Exists(id)}");
#endif
                    return id; //热更读取
                }
                else
                    return location.InternalId;
            }
            else
            {
                return location.InternalId;
            }
        }

        /// <summary>
        /// check file is download from remote
        /// </summary>
        public static bool CheckIsUpdateFile(string abName)
        {
            if (updateFolderManifest != null)
            {
                return updateFolderManifest.GetFileResInfo(abName) != null;
            }
            return false;
        }

        /// <summary>
        /// 用新的文件列表，更新同名streaming foldermanifest的配置
        /// </summary>      
        public static bool UpdateStreamingFolderManifest(FolderManifest newFolderManifest)
        {
            FolderManifest curr;
            for (int i = 0; i < streamingFolderManifest.Count; i++)
            {
                curr = streamingFolderManifest[i];
                if (curr.folderName == newFolderManifest.folderName)
                {
                    return curr.AppendFileManifest(newFolderManifest);
                }
            }
            return false;
        }

        /// <summary>
        /// 查找本地folder
        /// </summary>   
        public static FolderManifest FindStreamingFolderManifest(string folderName)
        {
            FolderManifest curr = null;
            for (int i = 0; i < streamingFolderManifest.Count; i++)
            {
                curr = streamingFolderManifest[i];
                if (curr.folderName == folderName)
                {
                    return curr;
                }
            }
            return curr;
        }
        /**
        internal static IEnumerator StartClearOldFiles (System.Action<LoadingEventArg> onProgress, System.Action onComplete) {
            string path = CUtils.GetRealPersistentDataPath ();
            DirectoryInfo dinfo = new DirectoryInfo (path);
            if (dinfo.Exists) {
                var allFiles = dinfo.GetFiles ("*", SearchOption.AllDirectories);
                var loadingEventArg = new LoadingEventArg ();
                loadingEventArg.total = allFiles.Length;

                FileInfo fino;
                for (int i = 0; i < allFiles.Length; i++) {
                    fino = allFiles[i];
                    fino.Delete ();
                    loadingEventArg.current++;
                    loadingEventArg.progress = (float) loadingEventArg.current / (float) loadingEventArg.total;
                    if (onProgress != null) onProgress (loadingEventArg);
                    yield return null;
                };

                if (onProgress != null) onProgress (loadingEventArg);

            }

            if (onComplete != null) onComplete ();
        }
        **/
        #endregion
    }

}