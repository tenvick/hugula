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
        internal static List<FileManifest> streamingFolderManifest = new List<FileManifest>();

        /// <summary>
        /// 本地简单clone
        /// </summary>
        internal static List<FolderManifest> streamingFolderManifestSimpleClone = new List<FolderManifest>();

        /// <summary>
        /// 本地更新的远端配置
        /// </summary>
        internal static List<FileManifest> persistentFolderManifest = new List<FileManifest>();

        private static int m_LocalResNum = Hugula.CodeVersion.APP_NUMBER;

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

        private static int m_LocalStreamingResNum = Hugula.CodeVersion.APP_NUMBER;

        /// <summary>
        /// 本地最新数字版本号
        /// </summary>
        public static int localStreamingResNum
        {
            get
            {
                return m_LocalStreamingResNum;
            }
            set
            {
                m_LocalStreamingResNum = value;
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

        #region mem cache
        /// <summary>
        /// address的状态
        /// </summary>
        enum ZipAddressState
        {
            //需要判断
            NeedCheck,
            //通过判断下载成功
            Success,
            //没有全部下载
            Fail
        }

        /// <summary>
        /// 检测扩展包的资源是否已经下载
        /// </summary>
        static Dictionary<string, ZipAddressState> addressIsDownDic = new Dictionary<string, ZipAddressState>();

        /// <summary>
        /// 设置扩展包里加载address key的完成状态
        /// </summary>
        internal static void SetFolderAddressIsDown(FolderManifest folder, bool isAllDown)
        {
            var keys = folder.allAddressKeys;
            if (keys == null) return;

            var key = string.Empty;
            for (var i = 0; i < keys.Count; i++)
            {
                key = keys[i];

                if (isAllDown)
                {
                    addressIsDownDic.Remove(key);
                    // addressIsDownDic[key] = ZipAddressState.NeedCheck;
                }
                else
                {
                    addressIsDownDic[key] = ZipAddressState.Fail;
                }
            }
        }

        #endregion

        #region 资源 check  相关



        /// <summary>
        /// 检测zip包里面的bundle是否已经下载
        /// </summary>
        public static bool CheckBundleIsDown(string bundleName)
        {

            FolderManifest find = null;
            for (int i = 0; i < streamingFolderManifest.Count; i++)
            {
                find = streamingFolderManifest[i] as FolderManifest;
                if (find != null && find.isZipDone && find.GetFileResInfo(bundleName) != null)
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
                    curr = persistentFolderManifest[i] as FolderManifest;
                    if (curr != null && curr.GetFileResInfo(abName) != null)
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

            bool check = activeIndex == curIndex;

            if (!check)
                return true;

#endif

#if DONT_SUB_PACKAGE
            return true;
#else

            if (string.IsNullOrEmpty(address))
            {
#if !HUGULA_NO_LOG
                Debug.LogError("FileManifestManager.CheckAddressIsDown() argument address is null or Empty");
#endif
                return false;
            }

            if (type == null) type = DEFAULT_TYPE;

            if (addressIsDownDic.TryGetValue(address, out var addState))
            {
                if (addState == ZipAddressState.NeedCheck)
                {
                    var isAllDown = CheckAddressDependenciesIsDown(address, type);
                    if (isAllDown)
                    {
                        addState = ZipAddressState.Success;
                        addressIsDownDic.Remove(address); //移除
                    }
                    else
                    {
                        addState = ZipAddressState.Fail;
                        addressIsDownDic[address] = addState;
                    }
                }
#if !HUGULA_NO_LOG && LOG_ZIP_ADDRESS
                Debug.LogWarning($"FileManifestManager.CheckAddressIsDown({address}) isdown:{addState == ZipAddressState.Success}");
#endif
                return addState == ZipAddressState.Success;
            }
            else
            {
#if !HUGULA_NO_LOG && LOG_ZIP_ADDRESS
                Debug.LogWarning($"FileManifestManager.CheckAddressIsDown({address}) not in addressIsDownDic true");
#endif

                return true;
            }
#endif
        }

        /// <summary>
        /// 通过address地址判断资源是否已经下载
        /// </summary>
        public static bool CheckAddressDependenciesIsDown(string address, System.Type type = null)
        {
            var folders = FindFolderManifestByAddress(address, type);
            FolderManifest folder;
            bool isDown = true;
            for (int i = 0; i < folders.Count; i++)
            {
                folder = folders[i];
                if (!folder.isZipDone)
                {
                    isDown = false;
                    break;
                }
            }

            ListPool<FolderManifest>.Release(folders);
            return isDown;
        }

        /// <summary>
        /// 通过文件列表判断zip扩展包是否下载完成
        /// </summary>
        public static bool CheckZipFolderManifestsIsDown(List<FolderManifest> folderManifests)
        {
            foreach (var f in folderManifests)
            {
                if (f.zipSize > 0 && !f.isZipDone) //如果有zip文件并且没有下载完成
                {
                    return false;
                }
            }

            return true;
        }


#if UNITY_EDITOR
        [XLua.DoNotGen]
        [System.ObsoleteAttribute]
        /// <summary>
        ///  分析依赖bundle
        /// </summary>
        public static void AnalyzeAddressDependencies(string address, System.Type type, List<string> dependenciesInfo)
        {
            var abNames = FindBundleNameByAddress(address, type);
            FolderManifest find = null;
            string bundleName = null;
            for (int i = 0; i < streamingFolderManifest.Count; i++)
            {
                find = streamingFolderManifest[i] as FolderManifest;
                if (find)
                {
                    for (int j = 0; j < abNames.Count;)
                    {
                        bundleName = abNames[j];
                        if (find.GetFileResInfo(bundleName) != null)
                        {
                            abNames.RemoveAt(j);
                            dependenciesInfo.Add(find.fileName);
                            dependenciesInfo.Add(find.priority.ToString());
                            dependenciesInfo.Add(bundleName);
                        }
                        else
                            j++;
                    }
                }
            }

            ListPool<string>.Release(abNames);
        }


#endif

        /// <summary>
        /// 检测当前是否可以覆盖stream中文件夹信息
        /// </summary>      
        internal static bool CheckStreamingFolderManifestResNumber(FolderManifest newFolderManifest)
        {
            FolderManifest curr;
            for (int i = 0; i < streamingFolderManifest.Count; i++)
            {
                curr = streamingFolderManifest[i] as FolderManifest;
                if (curr && curr.fileName == newFolderManifest.fileName)
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
        internal static bool CheckAddOrUpdatePersistentFolderManifest(FileManifest persistent)
        {
            FileManifest find = null;
            for (int i = 0; i < persistentFolderManifest.Count; i++)
            {
                find = persistentFolderManifest[i];
                if (find != null && find.fileName == persistent.fileName)
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
        /// 查询热更新目录的BundleManifest信息，用于单bundle的增量更新，比如lua
        /// </summary>
        public static BundleManifest FindPersistentBundleManifest(string name)
        {
            if (persistentFolderManifest == null) return null;
            BundleManifest curr = null;
            for (int i = 0; i < persistentFolderManifest.Count; i++)
            {
                curr = persistentFolderManifest[i] as BundleManifest;
                if (curr && curr.fileName == name)
                {
                    return curr;
                }
            }
            return curr;
        }

        /// <summary>
        /// 返回streamingFolderManifestSimpleClone文件夹简单内容，不要修改。
        /// </summary>
        public static List<FolderManifest> GetStreamingFolderManifests()
        {
            return streamingFolderManifestSimpleClone;
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
                find = streamingFolderManifest[i] as FolderManifest;
                if (find)
                {
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
        /// 查找streaming中 filemanifest
        /// </summary>   
        internal static FileManifest FindStreamingFolderManifest(string fileName, bool onlyFolderManifest = true)
        {
            FileManifest curr = null;
            for (int i = 0; i < streamingFolderManifest.Count; i++)
            {
                curr = streamingFolderManifest[i];
                if (onlyFolderManifest && curr is FolderManifest && curr.fileName == fileName)
                {
                    return curr;
                }
                else if (!onlyFolderManifest && curr.fileName == fileName)
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
                curr = persistentFolderManifest[i] as FolderManifest;
                if (curr && curr.fileName == folderName)
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
        public static void LoadStreamingFolderManifests(System.Action onComplete = null)
        {
            var fileListName = Common.STREAMING_ALL_FOLDERMANIFEST_BUNDLE_NAME;
            var url = CUtils.PathCombine(CUtils.GetRealStreamingAssetsPath(), fileListName);

#if UNITY_EDITOR
            if (!FileHelper.FileExists(url))
            {
                Debug.LogWarning($"there is no folderManifest in {url} use (Addressabkes Groups /Build/New Build/Hot Resource Update) to build ");
                return;
            }
#endif

            url = CUtils.GetAndroidABLoadPath(url);
            AssetBundle ab = AssetBundle.LoadFromFile(url, 0, Common.BUNDLE_OFF_SET);
            if (ab != null)
            {
                var assets = ab.LoadAllAssets<FileManifest>();
                FileManifestManager.streamingFolderManifest = new List<FileManifest>(assets);
                if (assets.Length == 0)
                    Debug.LogError("there is no streamingManifest at " + url);
                else
                {
                    var item = assets[0];
                    // localVersion = item.version; //更新到本地版本号
                    localResNum = item.resNumber;
                    localStreamingResNum = localResNum;
                }

                streamingFolderManifestSimpleClone.Clear();
                FolderManifest folder = null;
                foreach (var s in assets)
                {
                    folder = s as FolderManifest;
                    if (folder)
                        streamingFolderManifestSimpleClone.Add(folder.CloneWithOutAllFileInfos());
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
            else
            {
#if UNITY_EDITOR
                File.Delete(url); //版本不对删除
                Debug.LogWarning("there is no folderManifest in StreamingAssetsPath use (Addressabkes Groups /Build/New Build/Hot Resource Update) to build ");
#endif
            }

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
            if (FileHelper.FileExists(url))
            {
                if ((ab = AssetBundle.LoadFromFile(url, 0, Common.BUNDLE_OFF_SET)) != null)
                {
                    var folderManifests = ab.LoadAllAssets<FileManifest>();

                    if (folderManifests.Length == 0)
                        Debug.LogError("there is no  persistentFolderManifest  at  " + url);
                    FileManifest item;
                    FileManifest streamingItem;
                    var cacheLocalResNum = localStreamingResNum;

                    for (int i = 0; i < folderManifests.Length; i++)
                    {
                        item = folderManifests[i];
                        if (cacheLocalResNum < item.resNumber) //新增加的folder
                        {
                            streamingItem = FindStreamingFolderManifest(item.fileName, false);
                            if (streamingItem != null)
                                item.RemoveSameFileResInfoFrom(streamingItem);
                            // else
                            //     streamingFolderManifest.Add(item); //暂时不支持新增加zip包

                            CheckAddOrUpdatePersistentFolderManifest(item);

                            if (item is FolderManifest)
                                GenUpdatePackageTransform((FolderManifest)item);

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
                else
                {
                    File.Delete(url);
                }
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
                item = streamingFolderManifest[i] as FolderManifest;
                if (item && item.zipSize > 0)
                {
                    FileManifestManager.GenZipPackageTransform(item);
                }
            }
        }

        ///<summary>
        /// 通过 address key 构建当前foldername的资源address地址重定向
        ///</summary>
        public static void GenOverrideAddressByAddress(string address, System.Type type, string defaultKey, System.Func<string, string> onTransform = null)
        {
            var folders = FindFolderManifestByAddress(address, type);
            List<string> allAddressKeys;
            FolderManifest folder;
            FolderManifest found = null;

            for (int i = 0; i < folders.Count; i++)
            {
                folder = folders[i];
                if (folder.fileName != Common.FOLDER_STREAMING_NAME && !folder.isZipDone)
                {
                    allAddressKeys = folder.allAddressKeys;
                    for (int j = 0; j < allAddressKeys.Count; j++)
                    {
                        if (allAddressKeys[j].Equals(address))
                        {
                            found = folder;
                        }
                    }
                }
            }

            if (found != null)
            {
                var kes = found.allAddressKeys;
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
                Debug.Log($"GenOverrideAddressTransformFunc:{found}");
#endif
            }
        }


        ///<summary>
        /// 构建有依赖加载的资源address地址重定向
        ///</summary>
        public static void GenOverrideAddressByFolderName(string folderName, string defaultKey, System.Func<string, string> onTransform = null)
        {
            var folder = FindStreamingFolderManifest(folderName) as FolderManifest;
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
            else
            {
                Debug.LogWarning($"GenOverrideAddressTransformFunc:{folderName} fail, streaming_all.u3d does't contains {folderName}");
            }

        }

        ///<summary>
        /// 清理有依赖加载的资源address地址重定向
        ///</summary>
        public static void ClearOverrideAddressTransformFunc(string folderName)
        {
            var folder = FindStreamingFolderManifest(folderName) as FolderManifest;
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
        internal static string OverrideAddressTransformFunc(string address)
        {
            if (address != null && addressOverridePath.TryGetValue(address, out var newAddres))
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
                GenUpdatePackageTransform(persistentFolderManifest[i] as FolderManifest);
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
                    sb.AppendLine($"\r\n new Addressables.ResourceLocators:({item.LocatorId}):");
                    keys.Clear();
                    keys.AddRange(item.Keys);
                    sb.AppendLine($"\r\n  -------------------------------{item.LocatorId}-Count:{keys.Count}------------------");

                    foreach (var key in keys)
                    {
                        if (item.Locate(key, typeof(object), out var locations))
                        {
                            sb.AppendLine($"\r\n\r\n            -----{item.LocatorId} key({key.ToString()})      locations.Count:{locations.Count}");
                            foreach (var loc in locations)
                            {
                                sb.AppendLine($"                    {key.ToString()}   ResourceType:({loc.ResourceType}) PrimaryKey({loc.PrimaryKey}) InternalId({loc.InternalId}) ");

                                if (loc.HasDependencies)
                                {
                                    sb.Append($"                        HasDependencies:{loc.HasDependencies}  DependencyHashCode:{loc.DependencyHashCode} , \r\n                        Dependencies:");
                                    foreach (var dep in loc.Dependencies)
                                    {
                                        sb.Append($"\r\n                          Dep.PrimaryKey({dep.PrimaryKey}) Dep.InternalId({dep.InternalId})  ,");
                                    }

                                }
                            }
                        }
                        else
                            sb.AppendLine($"\r\n\r\n            -----{item.LocatorId} key({key.ToString()})--Count:0");
                    }
                    Debug.Log(sb.ToString());
                    sb.Clear();
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
                Debug.Log($"generate zip folder {folderPackage.fileName} Transform size:{folderPackage.zipSize}) ");
#endif
                var allFiles = folderPackage.allFileInfos;
                var zipOutPath = folderPackage.GetZipOutFolderPath();
                for (int i = 0; i < allFiles.Count; i++)
                {
                    finfo = allFiles[i];
                    if (!locationIdPath.ContainsKey(finfo.name))
                    {
                        locationIdPath[finfo.name] = zipOutPath;
                        #if !HUGULA_NO_LOG
                                                Debug.Log($" zip folder:{folderPackage.fileName} transform({finfo.name}={zipOutPath}) ");
                        #endif
                    }
                }

                //清理address状态
                SetFolderAddressIsDown(folderPackage, true);
            }
            else
            {
                SetFolderAddressIsDown(folderPackage, false);
#if !HUGULA_NO_LOG
                Debug.LogWarning($"generate zip folder transform fail , {folderPackage.fileName} have't loaded size:{folderPackage.zipSize}) ");
#endif
            }
        }

        /// <summary>
        ///  构建热更新包文件地址映射
        /// </summary>
        internal static void GenUpdatePackageTransform(FolderManifest folderPackage)
        {
            if (folderPackage)
            {
                var allFiles = folderPackage.allFileInfos;
                FileResInfo finfo;
                for (int i = 0; i < allFiles.Count; i++)
                {
                    finfo = allFiles[i];
                    locationIdPath[finfo.name] = CUtils.realPersistentDataPath;
#if !HUGULA_NO_LOG
                    Debug.Log($" update folder:{folderPackage.fileName} transform({finfo.name}={CUtils.realPersistentDataPath}) ");
#endif
                }
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
            //Debug.Log($"OverrideLocationURL PrimaryKey={location.PrimaryKey}  InternalId={location.InternalId} ResourceType={location.ResourceType} data={location.Data} ");
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