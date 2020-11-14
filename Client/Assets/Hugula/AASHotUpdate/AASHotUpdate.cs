using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
/// <summary>
/// 本类是基于AAS框架的热更新类
/// </summary>
public class AASHotUpdate
{
    const string remoteUrl = "http://10.23.7.212:8000";//服务器cdn地址，最好支持配置

    static readonly string localVersionPath = Path.Combine(Application.persistentDataPath, "version.txt");//本地资源版本号缓存位置
    static readonly string remoteVersionUrl = remoteUrl + "/version.txt";//服务器资源版本号地址
    static readonly string localCatalogPath = Path.Combine(Application.persistentDataPath, "catalog.json");//本地catalog缓存位置
    static readonly string remoteCatalogUrl = remoteUrl + "/catalog.json";//服务器catalog地址

    static List<string> dirtyBundle;
    
    public static IEnumerator Start()
    {
        //初始化AAS
        yield return Addressables.InitializeAsync();

        //修改provider fixed AAS bug
        foreach (var provider in Addressables.ResourceManager.ResourceProviders)
        {
            if (typeof(TextDataProvider).IsAssignableFrom(provider.GetType()))
            {
                (provider as TextDataProvider).IgnoreFailures = false;
            }
        }

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
            //yield break;
        }

        //拉取CDN资源版本号


        var remoteVersionHandle = LoadText(remoteVersionUrl, "RemoteVersion");
        yield return remoteVersionHandle;
        var remoteVersion = remoteVersionHandle.Result;
        Addressables.Release(remoteVersionHandle);

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
        }

        //本地持续化目录没有缓存catalog
        if (!File.Exists(localCatalogPath))
        {
            yield break;
        }

        //对比catalog 解析出需要热更的bundle

        var localCatalogLocation = new ResourceLocationBase("LocalCatalog", localCatalogPath, typeof(JsonAssetProvider).FullName, typeof(ContentCatalogData));
        var localCatalogHandle = Addressables.LoadAssetAsync<ContentCatalogData>(localCatalogLocation);
        yield return localCatalogHandle;
        var data = localCatalogHandle.Result;
        Addressables.Release(localCatalogHandle);
        ResourceLocationMap RemoteMap = data.CreateLocator(null);
        var newBundles = parseBundleList(RemoteMap);
        dirtyBundle = null;
        foreach (ResourceLocationMap item in Addressables.ResourceLocators)
        {
            if ("AddressablesMainContentCatalog" == item.LocatorId)
            {
                var oldBundles = parseBundleList(item);
                dirtyBundle = compareResLocation(newBundles, oldBundles);
                Addressables.RemoveResourceLocator(item);
                Addressables.AddResourceLocator(RemoteMap);
                break;
            }
        }

        if (null == dirtyBundle || 0 == dirtyBundle.Count) yield break;
        Addressables.InternalIdTransformFunc = convertBundleInternalId;

        long size = 0;

        foreach (var key in dirtyBundle)
        {
            IResourceLocation location;
            newBundles.TryGetValue(key, out location);
            var sizeData = location.Data as ILocationSizeData;
            if (sizeData != null)
                size += sizeData.ComputeSize(location, Addressables.ResourceManager);
        }

        LogFormat("需要下载的补丁大小:{0}", size);

        AsyncOperationHandle handler = Addressables.DownloadDependenciesAsync(dirtyBundle, Addressables.MergeMode.Union);
        
        while (!handler.IsDone)
        {
            LogFormat("当前下载进度:{0}",handler.PercentComplete);
            yield return null;
        }
        if (AsyncOperationStatus.Succeeded != handler.Status)
        {
            LogErrorFormat("下载补丁失败");
        }
        else
        {
            LogFormat("补丁下载完成");
        }
        Addressables.Release(handler);

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
            return remoteUrl + "/" + resourceLocation.PrimaryKey;
        }
        return resourceLocation.InternalId;
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
        var location = new ResourceLocationBase(debugName, loalUri, typeof(TextDataProvider).FullName, typeof(string));
        var handle = Addressables.LoadAssetAsync<string>(location);
        return handle;
    }


    /// <summary>
    /// 解析ResourceLocationMap 中所有的bundle
    /// </summary>
    /// <param name="locMap"></param>
    /// <returns></returns>
    static Dictionary<string, IResourceLocation> parseBundleList(ResourceLocationMap locMap)
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

#if UNITY_EDITOR

    [UnityEditor.MenuItem("Hugula/ASS Build")]
    public static void OnBuildScript()
    {
        UnityEditor.AddressableAssets.AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilderIndex = 3;
        UnityEditor.AddressableAssets.Settings.AddressableAssetSettings.BuildPlayerContent();
    }
#endif
}
