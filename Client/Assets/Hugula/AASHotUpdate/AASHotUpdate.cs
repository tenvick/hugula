using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.Networking;

namespace Hugula
{
    /// <summary>
    /// 本类是基于AAS框架的热更新类
    /// </summary>
    public class AASHotUpdate
    {
        #region cdn config
        static readonly string localVersionPath = Path.Combine(Application.persistentDataPath, "version.txt");//本地资源版本号缓存位置
        static string remoteVersionUrl
        {
            get 
            {
                return platformPath + "/version.txt";//服务器资源版本号地址
            }
        } 
        static readonly string localCatalogPath = Path.Combine(Application.persistentDataPath, "catalog.json");//本地catalog缓存位置

        static string remoteCatalogUrl
        {
            get
            {
                return platformPath + "/catalog.json";//服务器catalog地址
            }
        }

        private static string m_PersistPath = string.Empty;
        private static string m_PackagePath = string.Empty;
        private const string m_FileName = "CDN_Config.txt";
        private const string m_Key_CDN = "CDN_URL";
        private static Dictionary<string, string> m_ConfigMap = new Dictionary<string, string>();

        private static string m_RemoteUrl = string.Empty; //服务器cdn地址
        public static string remoteUrl
        {
            get
            {
                if(string.IsNullOrEmpty(m_RemoteUrl))
                {
                    if (m_ConfigMap != null && m_ConfigMap.ContainsKey(m_Key_CDN))
                    {
                        m_RemoteUrl = m_ConfigMap[m_Key_CDN];
                    }
                }
                LogFormat("get m_RemoteUrl = " + m_RemoteUrl);
                return m_RemoteUrl;
            }
        }

        public static string platformPath
        {
            get
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    return remoteUrl + "Android";
                }
                else if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    return remoteUrl + "iOS";
                }
                else if (Application.platform == RuntimePlatform.WindowsPlayer)
                {
                    return remoteUrl + "Windows";
                }
                return remoteUrl + "Windows";
            }
        }

        private static string m_HotAssetBundlePersistPath = "";
        public static string  LocalHotAssetBundlePath
        {
            get
            {
                if (string.IsNullOrEmpty(m_HotAssetBundlePersistPath))
                {
#if UNITY_EDITOR
                    m_HotAssetBundlePersistPath = Application.persistentDataPath + "/HotAssetBundle/";
#elif UNITY_ANDROID
                m_HotAssetBundlePersistPath = Application.persistentDataPath + "/HotAssetBundle/";
#elif UNITY_IOS
                m_HotAssetBundlePersistPath = Application.persistentDataPath + "/Raw/HotAssetBundle/";
#else
                m_HotAssetBundlePersistPath = Application.persistentDataPath + "/HotAssetBundle/";
#endif
                }

                return m_HotAssetBundlePersistPath;
            }
        }




        public static string PackageCDNConfigPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_PackagePath))
                {
#if UNITY_EDITOR
                m_PackagePath = System.IO.Path.Combine(Application.streamingAssetsPath, m_FileName);
#elif UNITY_ANDROID
                m_PackagePath = "jar:file://" + Application.dataPath + "!/assets/" + m_FileName;
#elif UNITY_IOS
                m_PackagePath = Application.dataPath + "/Raw/"+ m_FileName;
#else
                m_PackagePath = System.IO.Path.Combine(Application.streamingAssetsPath, m_FileName);
#endif
                }
                return m_PackagePath;
            }
        }

        public static string PersistCDNConfigPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_PersistPath))
                {
#if UNITY_EDITOR
                    m_PersistPath = Application.persistentDataPath + "/StreamingAssets/PC/Assets/" + m_FileName;
#elif UNITY_ANDROID
                m_PersistPath = Application.persistentDataPath + "/StreamingAssets/Android/Assets/" + m_FileName;
#elif UNITY_IOS
                m_PersistPath = Application.persistentDataPath + "/StreamingAssets/iOS/Assets/" + m_FileName;
#else
                m_PersistPath = Application.persistentDataPath + "/StreamingAssets/PC/Assets/" + m_FileName;
#endif
                }
                return m_PersistPath;
            }
        }

        private static IEnumerator LoadCDNConfig()
        {
            if (string.IsNullOrEmpty(m_RemoteUrl))
            {
                if (File.Exists(PersistCDNConfigPath)) //优先读取持久化目录的cdn配置
                {
                    var persistCDNHandle = LoadText(PersistCDNConfigPath, "persist_cdn_cfg_path");
                    yield return persistCDNHandle;
                    string content = persistCDNHandle.Result;
                    LogFormat("persist cdn config:" + content);
                    Addressables.Release(persistCDNHandle);
                    if (!string.IsNullOrEmpty(content))
                    {
                        ParseFile(content);
                    }
                }
                else
                {
                    var packageCDNHandle = LoadText(PackageCDNConfigPath, "package_cdn_cfg_path");
                    yield return packageCDNHandle;
                    string content = packageCDNHandle.Result;
                    LogFormat("package cdn config:" + content);
                    Addressables.Release(packageCDNHandle);
                    if (!string.IsNullOrEmpty(content))
                    {
                        ParseFile(content);
                        string persistCDNPathWithoutFileName = PersistCDNConfigPath.Replace(m_FileName, "");
                        if (!Directory.Exists(persistCDNPathWithoutFileName))
                        {
                            Directory.CreateDirectory(persistCDNPathWithoutFileName);
                        }
                        File.WriteAllText(PersistCDNConfigPath, content);
                    }
                }
                if (m_ConfigMap != null && m_ConfigMap.ContainsKey(m_Key_CDN))
                {
                    m_RemoteUrl = m_ConfigMap[m_Key_CDN];
                    LogFormat("set m_RemoteUrl = " + m_RemoteUrl);
                }
            }
        }

        private static void ParseFile(string kConfig)
        {
            if(m_ConfigMap == null)
            {
                m_ConfigMap = new Dictionary<string, string>();
            }
            string[] kLines = kConfig.Split('\n');
            for (int iIndex = 0; kLines != null && iIndex < kLines.Length; iIndex++)
            {
                string strLineRaw = kLines[iIndex];
                string strLine = strLineRaw.Trim();
                string[] kResult = strLine.Split('=');

                if (kResult == null || kResult.Length != 2)
                {
                    Debug.LogWarning("[AASHotUpdate.ParseFile]::Config error,line=" + strLine);
                    continue;
                }
                string kKey = kResult[0];
                string kValue = kResult[1];

                if (m_ConfigMap.ContainsKey(kKey))
                    m_ConfigMap.Remove(kKey);

                if (!m_ConfigMap.ContainsKey(kKey))
                    m_ConfigMap.Add(kKey, kValue);
                LogFormat("add config:" + kKey + "=" + kValue);
            }
            LogFormat("m_ConfigMap count:" + m_ConfigMap.Count);
        }

        #endregion cdn config

        static List<string> dirtyBundle;

        public static IEnumerator Start()
        {
            float startTime = Time.realtimeSinceStartup;
#if (UNITY_EDITOR && !USE_CDN) || DISABLE_CDN
            yield break;
#endif
            //初始化AAS
            yield return Addressables.InitializeAsync();
            LogTimeDurFormat("Addressables.InitializeAsync", ref startTime, true);
            //修改provider fixed AAS bug
            foreach (var provider in Addressables.ResourceManager.ResourceProviders)
            {
                if (typeof(TextDataProvider).IsAssignableFrom(provider.GetType()))
                {
                    (provider as TextDataProvider).IgnoreFailures = false;
                }
            }

            LogTimeDurFormat("foreach Addressables.ResourceManager.ResourceProviders ", ref startTime, true);
            //拉取本地资源版本号
            var localVersion = Application.version;
            if (File.Exists(localVersionPath))
            {
                var localVersionHandle = LoadText(localVersionPath, "LocalVersion"); ;
                yield return localVersionHandle;
                localVersion = localVersionHandle.Result;
                Addressables.Release(localVersionHandle);
            }
            var localResVersion = ResVersion.parseVersion(localVersion);
            var libResVersion = ResVersion.parseVersion(Application.version);

            LogFormat("local version:{0},library version:{1}", localVersion, Application.version);
            if (null == localResVersion || null == libResVersion)
            {
                yield break;
            }

            //包内版本大于本地持续化版本号（安装了新包） 清除本地缓存数据
            if (libResVersion.high > localResVersion.high || libResVersion.mid > localResVersion.mid)
            {
                LogFormat("清除本地缓存");
                Caching.ClearCache();
                File.Delete(localCatalogPath);
                File.Delete(localVersionPath);
                File.Delete(PersistCDNConfigPath);
                if(Directory.Exists(LocalHotAssetBundlePath))
                {
                    Directory.Delete(LocalHotAssetBundlePath, true);
                }
                
                //yield break;
            }
            LogTimeDurFormat("load localVersion", ref startTime, true);
            //读取CDN 地址配置
            yield return LoadCDNConfig();
            LogTimeDurFormat("load cdn config", ref startTime, true);
            //拉取CDN资源版本号
            var remoteVersionHandle = LoadText(remoteVersionUrl, "RemoteVersion");
            yield return remoteVersionHandle;
            var remoteVersion = remoteVersionHandle.Result;
            Addressables.Release(remoteVersionHandle);
            LogTimeDurFormat("load remoteVersion", ref startTime, true);
            if (null == remoteVersion)
            {
                LogErrorFormat("远程资源版本号加载失败");
                yield break;
            }
            LogFormat("remoteVersion:{0}", remoteVersion);

            var remoteResVersion = ResVersion.parseVersion(remoteVersion);
            if (null == remoteResVersion)
            {
                yield break;
            }
            if (remoteResVersion.high < localResVersion.high || (remoteResVersion.high == localResVersion.high && remoteResVersion.mid < localResVersion.mid))
            {
                LogFormat("服务器资源版本过低");
                yield break;
            }
            if (remoteResVersion.high > localResVersion.high || remoteResVersion.mid > localResVersion.mid)
            {
                LogFormat("需要更新整包");
                yield break;
            }
            else if (remoteResVersion.small > localResVersion.small)
            {
                LogFormat("need hot update");
                //拉取CDN catalog 信息
                LogFormat("remoteCatalogUrl:" + remoteCatalogUrl);
                var remoteCatalogHandle = LoadText(remoteCatalogUrl, "RemoteCatalog");
                yield return remoteCatalogHandle;
                var remoteCatalog = remoteCatalogHandle.Result;
                Addressables.Release(remoteCatalogHandle);
                if (null == remoteCatalog)
                {
                    LogErrorFormat("远程资源加载失败");
                    yield break;
                }

                //将最近的catalog缓存到本地持续化目录
                File.WriteAllText(localCatalogPath, remoteCatalog);
                File.WriteAllText(localVersionPath, remoteVersion);
                LogTimeDurFormat("load remote catalog", ref startTime, true);
            }
            LogFormat("localCatalogPath:" + localCatalogPath);
            //本地持续化目录没有缓存catalog
            if (!File.Exists(localCatalogPath))
            {
                yield break;
            }

            //对比catalog 解析出需要热更的bundle
            LogFormat("localCatalogPath:" + localCatalogPath);
            var localCatalogLocation = new ResourceLocationBase("LocalCatalog", localCatalogPath, typeof(JsonAssetProvider).FullName, typeof(ContentCatalogData));
            var localCatalogHandle = Addressables.LoadAssetAsync<ContentCatalogData>(localCatalogLocation);
            yield return localCatalogHandle;
            var data = localCatalogHandle.Result;
            Addressables.Release(localCatalogHandle);
            LogTimeDurFormat("load local catalog", ref startTime, true);

            ResourceLocationMap RemoteMap = data.CreateLocator(null);
            var newBundles = parseBundleList(RemoteMap, "remote");
            LogTimeDurFormat("foreach remote catalog", ref startTime, true);
            dirtyBundle = null;
           // Addressables.CheckForCatalogUpdates(false);
            foreach (ResourceLocationMap item in Addressables.ResourceLocators)
            {
                if ("AddressablesMainContentCatalog" == item.LocatorId)
                {
                    var oldBundles = parseBundleList(item, "local");
                    LogTimeDurFormat("load local catalog", ref startTime, true);
                    dirtyBundle = compareResLocation(newBundles, oldBundles);
                    Addressables.RemoveResourceLocator(item);
                    Addressables.AddResourceLocator(RemoteMap);
                    LogTimeDurFormat("compare catalog", ref startTime, true);
                    break;
                }
            }

            if (null == dirtyBundle || 0 == dirtyBundle.Count) yield break;

            //该方法计算下载大小只对远程bundle有用
            //long size = 0;
            //foreach (var key in dirtyBundle)
            //{
            //    IResourceLocation location;
            //    newBundles.TryGetValue(key, out location);
            //    var sizeData = location.Data as ILocationSizeData;
            //    if (sizeData != null)
            //        size += sizeData.ComputeSize(location, Addressables.ResourceManager);
            //}
           // LogFormat("需要下载的补丁大小:{0}", size);

            yield return DownLoadRemoteAssetBundles(dirtyBundle);
            LogTimeDurFormat("dowmload hot file", ref startTime, true);
            Addressables.InternalIdTransformFunc = convertBundleInternalId;
            LogFormat("下载完，预加载到内存");
            AsyncOperationHandle handler = Addressables.DownloadDependenciesAsync(dirtyBundle, Addressables.MergeMode.Union);
            while (!handler.IsDone)
            {
                    yield return null;
            }
            Addressables.Release(handler);
            LogTimeDurFormat("download dependency", ref startTime, true);
            yield return null;
        }

        static List<string> compareResLocation(Dictionary<string, IResourceLocation> newBundles, Dictionary<string, IResourceLocation> oldBundles)
        {
            List<string> bundleList = new List<string>();
            foreach (var key in newBundles.Keys)
            {
                IResourceLocation newLocation;
                newBundles.TryGetValue(key, out newLocation);
                IResourceLocation oldLocation;
                oldBundles.TryGetValue(key, out oldLocation);
                if (null != oldLocation)
                {
                    var newAbData = newLocation.Data as AssetBundleRequestOptions;
                    var oldAbData = oldLocation.Data as AssetBundleRequestOptions;
                    if (newAbData.Hash != oldAbData.Hash)
                    {
                        bundleList.Add(key);
                    }
                }
                else
                {
                    bundleList.Add(key);
                }
            }
            return bundleList;
        }

        /// <summary>
        /// 转换InternalId，本地的旧bundle需要重定位到服务器的bundle
        /// </summary>
        /// <param name="resourceLocation"></param>
        /// <returns></returns>
        static string convertBundleInternalId(IResourceLocation resourceLocation)
        {
            if (dirtyBundle.Contains(resourceLocation.PrimaryKey))
            {
                //LogErrorFormat("contert dirtybundle path to:" + LocalHotAssetBundlePath + resourceLocation.PrimaryKey);
                //return "file://" +  LocalHotAssetBundlePath  + resourceLocation.PrimaryKey;
                return  LocalHotAssetBundlePath + resourceLocation.PrimaryKey;
            }
           // LogErrorFormat("contert un dirtybundle path to original:" + resourceLocation.InternalId);
            return resourceLocation.InternalId;
        }


        static IEnumerator DownLoadRemoteAssetBundles(List<string> dirtyBundles)
        {
            float startTime = Time.realtimeSinceStartup;
            float singleFileStartTime = 0;
            string sourceBundlePath = "";
            string destBundlePath = "";
            if(dirtyBundles != null && dirtyBundles.Count > 0)
            {
                if(!Directory.Exists(LocalHotAssetBundlePath))
                {
                    Directory.CreateDirectory(LocalHotAssetBundlePath);
                }
                foreach (string key in dirtyBundles)
                {
                    destBundlePath = LocalHotAssetBundlePath + key;
                    if (File.Exists(destBundlePath))
                    {
                        LogErrorFormat("the file of same name already exist, skip this download: " + destBundlePath);
                         continue;
                    }
                    singleFileStartTime = Time.realtimeSinceStartup;
                    sourceBundlePath = platformPath + "/bundles/" + key;
                    UnityWebRequest reuqest = UnityWebRequest.Get(sourceBundlePath);
                    yield return reuqest.SendWebRequest();
                   while(!reuqest.isDone)
                    {
                        LogFormat( string.Format("正在下载{0}\n当前下载进度:", sourceBundlePath, reuqest.downloadProgress)); 
                       yield return null;
                    }
                    
                    if (reuqest.isNetworkError)
                    {
                        LogErrorFormat("Download Error:" + reuqest.error + "______  sourceBundlePath");
                    }
                    else
                    {
                        LogFormat(string.Format("文件{0}下载完成，准备写，耗时 {1} 秒", sourceBundlePath, Time.realtimeSinceStartup - singleFileStartTime));

                        singleFileStartTime = Time.realtimeSinceStartup;
                        var File = reuqest.downloadHandler.data;

                        //创建文件写入对象

                        FileStream nFile = new FileStream(LocalHotAssetBundlePath + key, FileMode.Create);

                        //写入数据

                        nFile.Write(File, 0, File.Length);
                        nFile.Flush();
                        nFile.Dispose();
                        nFile.Close();
                        LogFormat(string.Format("文件写入完成：{0}, 耗时 {1} 秒", LocalHotAssetBundlePath, Time.realtimeSinceStartup - singleFileStartTime));
                    }
                }
                yield return null;
                float durTime = Time.realtimeSinceStartup - startTime;
                LogFormat("所有文件下载完成,耗时：" + durTime + "秒");
            }
        }

        /// <summary>
        /// load text 加载文本  使用 TextDataProvider  支持本地和http
        /// </summary>
        /// <param name="loalUri"> 需要加载的URI </param>
        /// <param name="debugName"> 加载项debug名称 </param>
        /// <returns></returns>
        static AsyncOperationHandle<string> LoadText(string loalUri, string debugName)
        {
            if (null == debugName)
            {
                debugName = string.Format("LoadText {0}", loalUri);
            }
            LogFormat("LoadText: " + loalUri);
            var location = new ResourceLocationBase(debugName, loalUri, typeof(TextDataProvider).FullName, typeof(string));
            var handle = Addressables.LoadAssetAsync<string>(location);
            return handle;
        }


        /// <summary>
        /// 解析ResourceLocationMap 中所有的bundle
        /// </summary>
        /// <param name="locMap"></param>
        /// <returns></returns>
        static Dictionary<string, IResourceLocation> parseBundleList(ResourceLocationMap locMap, string flag)
        {
            Dictionary<string, IResourceLocation> bundleMap = new Dictionary<string, IResourceLocation>();
            foreach (var key in locMap.Keys)
            {
                IList<IResourceLocation> locations;
                locMap.Locations.TryGetValue(key, out locations);
                foreach (var _location in locations)
                {
                    if (_location.ResourceType == typeof(IAssetBundleResource) && !bundleMap.ContainsKey(_location.PrimaryKey))
                    {
                        bundleMap.Add(_location.PrimaryKey, _location);
                    }
                }
            }
            return bundleMap;
        }


        static void LogFormat(string format, params object[] args)
        {
            Debug.LogFormat(format, args);
        }

        static void LogErrorFormat(string format, params object[] args)
        {
            Debug.LogErrorFormat(format, args);
        }

        static void LogTimeDurFormat(string format, ref float startTime, bool updateStartTime = false)
        {
            Debug.Log("launch time log-----------" + format + ", 耗时:" + (Time.realtimeSinceStartup - startTime));
            if(updateStartTime)
            {
                startTime = Time.realtimeSinceStartup;
            }
        }

        /// <summary>
        /// 版本号类，低中高版本号
        /// </summary>
        class ResVersion
        {

            public string version;
            public int high;
            public int mid;
            public int small;
            public static ResVersion parseVersion(string version)
            {

                var vers = version.Split(new char[] { '.' });
                if (3 != vers.Length)
                {
                    return null;
                }
                try
                {
                    int high = int.Parse(vers[0]);
                    int mid = int.Parse(vers[1]);
                    int small = int.Parse(vers[2]);
                    return new ResVersion { version = version, high = high, mid = mid, small = small };
                }
                catch (System.Exception)
                {
                    LogErrorFormat("can't parse version : {0}", version);
                    return null;
                }
            }
        }
    }
}


