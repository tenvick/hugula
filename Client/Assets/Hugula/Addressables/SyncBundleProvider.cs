using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.AddressableAssets;

[DisplayName("Sync Bundle Provider")]
public class SyncBundleProvider : AssetBundleProvider
{

    class SyncAssetBundleResource : IAssetBundleResource
    {
        AssetBundle m_AssetBundle;
        string bundleLoadPath = "";
        Func<IResourceLocation, string> func = Addressables.InternalIdTransformFunc;
        public AssetBundle GetAssetBundle()
        {
            return m_AssetBundle;
        }

        internal void Start(ProvideHandle provideHandle, out bool result)
        {
            if(func != null)
            {
                bundleLoadPath = func(provideHandle.Location);
            }
           else
            {
                bundleLoadPath = provideHandle.Location.InternalId;
            }
            m_AssetBundle = AssetBundle.LoadFromFile(bundleLoadPath);
            // UnityEngine.Debug.Log($"AssetBundle.LoadFromFile({bundleLoadPath})");
            if (m_AssetBundle == null)
            {
                Debug.LogError("try load bundle sync failed " + bundleLoadPath); //同步加载失败
                result = false;
            }
            else
            {
                provideHandle.Complete(this, m_AssetBundle != null, null);
                result = true;
            }
        }

        internal void Unload()
        {
            if (m_AssetBundle != null)
            {
                m_AssetBundle.Unload(true);
                m_AssetBundle = null;
            }
        }
    }
    
    public override void Provide(ProvideHandle providerInterface)
    {
        bool syncLoadSuccess = false;
        new SyncAssetBundleResource().Start(providerInterface, out syncLoadSuccess);
        if(!syncLoadSuccess)
        {
            base.Provide(providerInterface);
        }
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
        if (asset is SyncAssetBundleResource syncResource)
            syncResource.Unload();
    }
}
