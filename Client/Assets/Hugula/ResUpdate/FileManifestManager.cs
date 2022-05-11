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
        private static System.Type DEFAULT_TYPE = typeof(UnityEngine.Object);

        #region  static field
        /// <summary>
        /// 本地streamingAssets目录的manifest
        /// </summary>
        internal static List<FolderManifest> streamingFolderManifest;

        /// <summary>
        /// 本地更新的远端配置
        /// </summary>
        internal static List<FolderManifest> persistentFolderManifest = new List<FolderManifest>();

        private static int m_LocalResNum;

        /// <summary>
        /// 本地最新数字版本号
        /// </summary>
        public static int localResNum
        {
            get
            {
                return m_LocalResNum;
            }
            set
            {
                m_LocalResNum = value;
            }
        }

        /// <summary>
        /// 除fast包之外其他zip下载控制
        /// </summary>
        public static OtherZipMode otherZipMode
        {
            get;
            internal set;
        }

        private static string m_LocalVersion;
        /// <summary>
        /// 本地最新版本号
        /// </summary>
        public static string localVersion
        {
            get
            {
                if (string.IsNullOrEmpty(m_LocalVersion))
                    return Application.version;
                else
                    return m_LocalVersion;
            }
            set
            {
                m_LocalVersion = value;
            }
        }

        /// <summary>
        /// bundle 地址重定向映射
        /// </summary>
        static Dictionary<string, string> locationIdPath = new Dictionary<string, string>();

        /// <summary>
        /// address key重定向用于默认资源显示
        /// </summary>
        static Dictionary<string, string> addressOverridePath = new Dictionary<string, string>();

        #endregion

        #region 资源 check  相关
        /// <summary>
        /// 检测bundle是否已经下载
        /// </summary>
        public static bool CheckBundleIsDown(string bundleName)
        {
            FolderManifest find = null;
            for (int i = 0; i < streamingFolderManifest.Count; i++)
            {
                find = streamingFolderManifest[i];
                if (find.GetFileResInfo(bundleName) != null && find.isZipDone)
                {
                    return true;
                }
            }
            return false;
        }

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
        /// check file is download from remote
        /// </summary>
        public static bool CheckIsUpdateFile(string abName)
        {
            if (persistentFolderManifest != null)
            {
                FolderManifest curr;
                for (int i = 0; i < persistentFolderManifest.Count; i++)
                {
                    curr = persistentFolderManifest[i];
                    if (curr.GetFileResInfo(abName) != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 通过address地址判断资源是否已经下载
        /// </summary>
        public static bool CheckAddressIsDown(string address, System.Type type = null)
        {
#if UNITY_EDITOR
            //如果非ab模式
            List<ScriptableObject> allDataBuilders = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.DataBuilders;
            var activeIndex = UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.ActivePlayModeDataBuilderIndex;
            var curIndex = allDataBuilders.IndexOf(allDataBuilders.Find(s => s.GetType() == typeof(UnityEditor.AddressableAssets.Build.DataBuilders.BuildScriptPackedPlayMode)));

            bool check =  activeIndex == curIndex;
               
            if (!check)
                return true;

#endif
            var abNames = FindBundleNameByAddress(address, type);
            var folders = FindFolderManifestByBundleName(abNames);
            ListPool<string>.Release(abNames);
            foreach (var f in folders)
            {
                if (f.folderName != Common.FOLDER_STREAMING_NAME && !f.isZipDone) return false;
            }
            if (folders.Count <= 0) return false;
            ListPool<FolderManifest>.Release(folders);
            return true;
        }
        /// <summary>
        /// 检测当前是否可以覆盖stream中文件夹信息
        /// </summary>      
        internal static bool CheckStreamingFolderManifestResNumber(FolderManifest newFolderManifest)
        {
            if (streamingFolderManifest == null) return false;
            FolderManifest curr;
            for (int i = 0; i < streamingFolderManifest.Count; i++)
            {
                curr = streamingFolderManifest[i];
                if (curr.folderName == newFolderManifest.folderName)
                {
                    bool canAppend = curr.resNumber < newFolderManifest.resNumber;
                    return canAppend;
                }
            }
            return false;
        }

        /// <summary>
        /// 添加或者更新PersistentManifest
        /// </summary>
        internal static bool CheckAddOrUpdatePersistentFolderManifest(FolderManifest persistent)
        {
            FolderManifest find = null;
            for (int i = 0; i < persistentFolderManifest.Count; i++)
            {
                find = persistentFolderManifest[i];
                if (find != null && find.folderName == persistent.folderName)
                {
                    persistentFolderManifest[i] = persistent;
                    return true;
                }
            }
            persistentFolderManifest.Add(persistent);
            return false;
        }
        #endregion

        #region 查找相关

        /// <summary>
        /// 返回streamingFolderManifest列表所有内容，注意不要修改里面内容。
        /// </summary>
        public  static List<FolderManifest> GetStreamingFolderManifests()
        {
            return  streamingFolderManifest;
        }

        /// <summary>
        /// 通过address地址查找bundle Name
        /// </summary>
        internal static List<string> FindBundleNameByAddress(string address, System.Type type = null)
        {
            List<string> depStr = ListPool<string>.Get();
            var resourceLocators = Addressables.ResourceLocators;
            if (type == null) type = DEFAULT_TYPE;
            foreach (var locator in resourceLocators)
            {
                if (locator.Locate(address, type, out var locations))
                {
                    var dependencies = locations[0].Dependencies;
                    for (int i = 0; i < dependencies.Count; i++)
                    {
                        depStr.Add(Path.GetFileName(dependencies[i].InternalId));
                    }
                }
            }

            return depStr;
        }

        /// <summary>
        /// 通过bundle name地址查找 folder manifest
        /// </summary>
        internal static List<FolderManifest> FindFolderManifestByBundleName(List<string> bundleName)
        {
            List<FolderManifest> listFolder = ListPool<FolderManifest>.Get();
            FolderManifest find = null;
            for (int i = 0; i < streamingFolderManifest.Count; i++)
            {
                find = streamingFolderManifest[i];
                for (int j = 0; j < bundleName.Count;)
                {
                    if (find.GetFileResInfo(bundleName[j]) != null)
                    {
                        if (!listFolder.Contains(find))
                            listFolder.Add(find);
                        bundleName.RemoveAt(j);
                    }
                    else
                        j++;
                }
            }
            return listFolder;
        }

        /// <summary>
        /// 通过address查找FolderManifest
        /// </summary>
        internal static List<FolderManifest> FindFolderManifestByAddress(string address, System.Type type = null)
        {
            var abNames = FindBundleNameByAddress(address, type);
            var ret = FindFolderManifestByBundleName(abNames);
            ListPool<string>.Release(abNames);
            return ret;
        }

        /// <summary>
        /// 查找本地folder
        /// </summary>   
        internal static FolderManifest FindStreamingFolderManifest(string folderName)
        {
            if (streamingFolderManifest == null) return null;
            FolderManifest curr = null;
            for (int i = 0; i < streamingFolderManifest.Count; i++)
            {
                curr = streamingFolderManifest[i];
                if (curr.folderName == folderName)
                {
                    return curr;
                }
            }
            return null;
        }

        /// <summary>
        /// 查找本地持续化目录folder,注意已经去重。
        /// </summary>   
        internal static FolderManifest FindPersistentFolderManifest(string folderName)
        {
            if (persistentFolderManifest == null) return null;
            FolderManifest curr = null;
            for (int i = 0; i < persistentFolderManifest.Count; i++)
            {
                curr = persistentFolderManifest[i];
                if (curr.folderName == folderName)
                {
                    return curr;
                }
            }
            return null;
        }

        #endregion

        #region  加载相关
        /// <summary>
        /// 读取Streaming中的folderManifest
        /// </summary>
        internal static void LoadStreamingFolderManifests(System.Action onComplete)
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
                else
                {
                    var item = assets[0];
                    // localVersion = item.version; //更新到本地版本号
                    localResNum = item.resNumber;
                }

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
        internal static void LoadPersistentFolderManifest(System.Action onComplete)
        {
            var url = CUtils.PathCombine(CUtils.GetRealPersistentDataPath(), Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME);//首先查找热更新目录
            AssetBundle ab = null;
            if (FileHelper.FileExists(url) && (ab = AssetBundle.LoadFromFile(url)) != null)
            {
                var folderManifests = ab.LoadAllAssets<FolderManifest>();

                if (folderManifests.Length == 0)
                    Debug.LogError("there is no  persistentFolderManifest  at  " + url);
                FolderManifest item;
                FolderManifest streamingItem;
                var cacheLocalResNum = localResNum;
                for (int i = 0; i < folderManifests.Length; i++)
                {
                    item = folderManifests[i];
                    if (cacheLocalResNum < item.resNumber) //新增加的folder
                    {
                        streamingItem = FindStreamingFolderManifest(item.folderName);
                        if (streamingItem != null)
                            item.RemoveSameFileResInfoFrom(streamingItem);
                        // else
                        //     streamingFolderManifest.Add(item); //暂时不支持新增加zip包

                        CheckAddOrUpdatePersistentFolderManifest(item);

                        GenUpdatePackageTransform(item);

                        if (CodeVersion.Subtract(CodeVersion.APP_VERSION, item.version) <= 0)
                            localVersion = item.version; //更新到下载的版本号
                        if (localResNum < item.resNumber)
                            localResNum = item.resNumber;
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

        #endregion

        #region  重定向

        ///<summary>
        /// 统一执行下载完毕的zip包文件地址重定向 
        ///</summary>
        internal static void GenLoadedZipTransform()
        {
            FolderManifest item;
            for (int i = 0; i < streamingFolderManifest.Count; i++)
            {
                item = streamingFolderManifest[i];
                if (item.zipSize > 0)
                {
                    FileManifestManager.GenZipPackageTransform(item);
                }
            }
        }

        ///<summary>
        /// 构建有依赖加载的资源address地址重定向
        ///</summary>
        public static void GenOverrideAddressTransformFunc(string folderName, string defaultKey, System.Func<string, string> onTransform = null)
        {
            var folder = FindStreamingFolderManifest(folderName);
            if (folder != null)
            {
                var kes = folder.allAddressKeys;
                string key, value;
                value = defaultKey;
                for (int i = 0; i < kes.Count; i++)
                {
                    key = kes[i];
                    if (onTransform != null)
                    {
                        value = onTransform(key);
                        if (string.IsNullOrEmpty(value)) value = defaultKey;

                    }
                    else
                    {
                        value = defaultKey;
                    }

#if !HUGULA_NO_LOG
                    Debug.Log($"GenOverrideAddressTransformFunc:{key} = {value}");
#endif
                    addressOverridePath[key] = value;
                }
#if !HUGULA_NO_LOG
                Debug.Log($"GenOverrideAddressTransformFunc:{folderName}");
#endif
            }

        }

        ///<summary>
        /// 清理有依赖加载的资源address地址重定向
        ///</summary>
        public static void ClearOverrideAddressTransformFunc(string folderName)
        {
            var folder = FindStreamingFolderManifest(folderName);
            if (folder != null)
            {
                var kes = folder.allAddressKeys;
                string key;
                for (int i = 0; i < kes.Count; i++)
                {
                    key = kes[i];
                    addressOverridePath.Remove(key);
                }
            }
#if !HUGULA_NO_LOG
            Debug.Log($"ClearOverrideAddressTransformFunc:{folderName}");
#endif

        }

        ///<summary>
        /// 有依赖加载的资源address地址重定向
        ///</summary>
        public static string OverrideAddressTransformFunc(string address)
        {
            if (addressOverridePath.TryGetValue(address, out var newAddres))
            {
#if !HUGULA_NO_LOG
                Debug.Log($"OverrideAddressTransformFunc:{address} to:{newAddres}");
#endif
                return newAddres;
            }
            else
                return address;
        }

        ///<summary>
        /// 统一执行持续化目录的热更新文件地址重定向 
        ///</summary>
        internal static void GenUpdatedPackagesTransform()
        {
            for (int i = 0; i < persistentFolderManifest.Count; i++)
            {
                GenUpdatePackageTransform(persistentFolderManifest[i]);
            }
        }

        /// <summary>
        /// Load content catalog from persistent path
        /// </summary>
        internal static IEnumerator RefreshCatalog()
        {
            //刷新catalog
            var catelogPersistentPath = CUtils.PathCombine(CUtils.realPersistentDataPath, "catalog.bundle");
#if !HUGULA_NO_LOG
            Debug.Log($"check RefreshPersistentCatelog :{catelogPersistentPath} !");
#endif
            if (!File.Exists(catelogPersistentPath) && !File.Exists(catelogPersistentPath = CUtils.PathCombine(CUtils.realPersistentDataPath, "catalog.json")))
            {
                yield break;
            }
            //check file crc
            if (CheckIsUpdateFile(Path.GetFileName(catelogPersistentPath)))
            {
                // Addressables.ClearDependencyCacheAsync();
                // Caching.ClearCache();
                yield return null;
                Addressables.ClearResourceLocators();
                yield return null;
                var op = Addressables.LoadContentCatalogAsync(catelogPersistentPath, true);
                yield return op;
                // var kCatalogAddress = UnityEngine.AddressableAssets.Initialization.ResourceManagerRuntimeData.kCatalogAddress;
                //remove 
                // List<UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator> remove = new List<UnityEngine.AddressableAssets.ResourceLocators.IResourceLocator>();
                // foreach (var item in Addressables.ResourceLocators)
                // {
                //     if(item.LocatorId == kCatalogAddress)
                //     {
                //         remove.Add(item);
                //     }
                // }

                // foreach(var rem in remove)
                // {
                //     Addressables.RemoveResourceLocator(rem);
                //     Debug.Log($"remove catalog:{rem.LocatorId}");
                // }

                Debug.Log($"refreshed catalog:{catelogPersistentPath}");
#if !HUGULA_NO_LOG
                var sb = new System.Text.StringBuilder();
                var keys = new List<object>();
                foreach (var item in Addressables.ResourceLocators)
                {
                    sb.AppendLine("/r/n new Addressables.ResourceLocators:(");
                    sb.Append(item.LocatorId);
                    sb.AppendLine("):");
                    keys.Clear();
                    keys.AddRange(item.Keys);
                    sb.AppendLine($"  -------------------------------{item.LocatorId}-Count:{keys.Count}------------------");

                    foreach (var key in keys)
                    {
                        if (item.Locate(key, typeof(object), out var locations))
                        {
                            sb.AppendLine($"            -----{item.LocatorId}-{key.ToString()}--Count:{locations.Count}");
                            foreach (var loc in locations)
                            {
                                sb.AppendLine($"                    {key.ToString()}   ({loc.ResourceType}){loc.PrimaryKey}:{loc.InternalId}");
                            }
                            sb.AppendLine($"            end-----{item.LocatorId}-{key.ToString()}--Count:{locations.Count}");
                        }
                        else
                            sb.AppendLine($"            -----{item.LocatorId}-{key.ToString()}--Count:0");
                    }
                    sb.AppendLine($"  end-------------------------------{item.LocatorId}-Count:{keys.Count}-------------------");

                    Debug.Log(sb.ToString());
                }
#endif
            }
        }

        /// <summary>
        ///  构建zip文件地址映射 覆盖优先级靠后
        /// </summary>
        internal static void GenZipPackageTransform(FolderManifest folderPackage)
        {
            FileResInfo finfo;
            var isZipDone = folderPackage.isZipDone;
            if (isZipDone)
            {
#if !HUGULA_NO_LOG
                Debug.Log($"generate zip folder {folderPackage.folderName} Transform size:{folderPackage.zipSize}) ");
#endif
                var allFiles = folderPackage.allFileInfos;
                var zipOutPath = folderPackage.GetZipOutFolderPath();
                for (int i = 0; i < allFiles.Count; i++)
                {
                    finfo = allFiles[i];
                    if (!locationIdPath.ContainsKey(finfo.name))
                    {
                        locationIdPath[finfo.name] = zipOutPath;
                        // #if !HUGULA_NO_LOG
                        //                         Debug.Log($" zip folder:{folderPackage.folderName} transform({finfo.name}={zipOutPath}) ");
                        // #endif
                    }
                }
            }
            else
            {
#if !HUGULA_NO_LOG
                Debug.LogWarning($"generate zip folder transform fail , {folderPackage.folderName} have't loaded size:{folderPackage.zipSize}) ");
#endif
            }
        }

        /// <summary>
        ///  构建热更新包文件地址映射
        /// </summary>
        internal static void GenUpdatePackageTransform(FolderManifest folderPackage)
        {
            var allFiles = folderPackage.allFileInfos;
            FileResInfo finfo;
            for (int i = 0; i < allFiles.Count; i++)
            {
                finfo = allFiles[i];
                locationIdPath[finfo.name] = CUtils.realPersistentDataPath;
#if !HUGULA_NO_LOG
                Debug.Log($" update folder:{folderPackage.folderName} transform({finfo.name}={CUtils.realPersistentDataPath}) ");
#endif
            }
        }

        /// <summary>
        /// set  Addressables.InternalIdTransformFunc function.
        /// </summary>
        /// <value>The instance.</value>
        internal static void OverrideInternalIdTransformFunc()
        {
            Addressables.InternalIdTransformFunc = OverrideLocationURL;
            Addressables.ResourceManager.InternalIdTransformFunc = OverrideLocationURL;
            ResLoader.AddressTransformFunc = OverrideAddressTransformFunc;
        }

        /// <summary>
        /// check find files in PersistentDataPath
        /// </summary>
        internal static string OverrideLocationURL(IResourceLocation location)
        {
#if !HUGULA_NO_LOG
            // Debug.Log($"OverrideLocationURL PrimaryKey={location.PrimaryKey}  InternalId={location.InternalId} ResourceType={location.ResourceType} data={location.Data} ");
#endif

            if (!location.InternalId.StartsWith("http")) //&& location.ResourceType == typeof(IAssetBundleResource)
            {
                string bundleName = Path.GetFileName(location.InternalId);
                string path = null;
                if (locationIdPath.TryGetValue(bundleName, out path))
                {
                    var id = CUtils.PathCombine(path, bundleName);
#if !HUGULA_NO_LOG
                    Debug.Log($"from cache {id} exist:{File.Exists(id)}");
#endif
                    if (!File.Exists(id))
                        id = location.InternalId;
                    return id; //热更读取
                }
                else
                {
                    return location.InternalId;
                }
            }
            else
            {
                return location.InternalId;
            }
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