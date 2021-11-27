using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

[DisplayName("Lua Sync Bundle Provider")]
public class LuaBundleProvider : AssetBundleProvider
{

    class LuaAssetBundleResource : IAssetBundleResource
    {
        AssetBundle m_AssetBundle;

        public AssetBundle GetAssetBundle()
        {
            return m_AssetBundle;
        }

        internal void LoadBundle(string bundleLoadPath)
        {
            m_AssetBundle = AssetBundle.LoadFromFile(bundleLoadPath);
            if (m_AssetBundle == null)
            {
                Debug.LogError("try load bundle sync failed " + bundleLoadPath); //同步加载失败
            }
        }

        internal void Start(ProvideHandle provideHandle, string bundleLoadPath)
        {
            if (m_AssetBundle == null) m_AssetBundle = AssetBundle.LoadFromFile(bundleLoadPath);
            provideHandle.Complete(this, m_AssetBundle != null, null);
        }

        internal void Unload()
        {
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(false);
                // m_AssetBundle = null;
            }
        }
    }

    Func<IResourceLocation, string> func = Addressables.InternalIdTransformFunc;
    Dictionary<string, LuaAssetBundleResource> m_LuaAssetBundleResDic = new Dictionary<string, LuaAssetBundleResource>();

    public override void Provide(ProvideHandle providerInterface)
    {

        string bundleLoadPath = "";
        if (func != null)
        {
            bundleLoadPath = func(providerInterface.Location);
        }
        else
        {
            bundleLoadPath = providerInterface.Location.InternalId;
        }

        if (!m_LuaAssetBundleResDic.TryGetValue(bundleLoadPath, out var luaAssetBundleResource)) //从缓存中寻找bundle 可能是热更新地址或者streaming
        {
            luaAssetBundleResource = new LuaAssetBundleResource();
            m_LuaAssetBundleResDic.Add(bundleLoadPath, luaAssetBundleResource);
            luaAssetBundleResource.LoadBundle(bundleLoadPath);//缓存bundle
            // Debug.LogWarningFormat(" new lua bundle path:{0}", bundleLoadPath);
        }
        // else
        // {
        //     Debug.LogWarningFormat(" cache lua bundle path:{0}", bundleLoadPath);
        // }
        luaAssetBundleResource.Start(providerInterface, bundleLoadPath);//

    }

    public override void Release(IResourceLocation location, object asset)
    {
        if (location == null)
            throw new ArgumentNullException("location");
        if (asset == null)
        {
            Debug.LogWarningFormat("Releasing null asset bundle from location {0}.  This is an indication that the bundle failed to load.", location);
            return;
        }
        if (asset is LuaAssetBundleResource syncResource)
            syncResource.Unload();
    }
}
