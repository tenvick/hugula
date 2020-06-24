using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Loader;
using Hugula.Utils;
using Hugula.Framework;
using System;


namespace Hugula
{

    public abstract class AssetBundleMappingManager<T, R> : Singleton<T>, IDisposable where T : AssetBundleMappingManager<T, R>, IDisposable where R : UnityEngine.Object
    {
        private AssetBundleMappingAsset m_AssetBundleMappingAsset;
        private Dictionary<string, IMappingAsset<R>> m_MappingAssetCache;
        private Dictionary<string, int> m_AssetRefer;
        private Dictionary<string, int> m_ABCacheSubCount;

        private Dictionary<string, Dictionary<string, int>> m_ABGroups;
        #region  static

        public static string GetAssetbundleName(string audioclipName)
        {
            if (instance != null)
            {
                instance.m_AssetBundleMappingAsset.GetAassetBundleName(audioclipName);
            }
            return null;
        }

        #endregion

        public AssetBundleMappingManager()
        {
            string mappingAssetName = GetMappingAssetName();
            m_AssetBundleMappingAsset = ResourcesLoader.LoadAsset<AssetBundleMappingAsset>(mappingAssetName + Common.CHECK_ASSETBUNDLE_SUFFIX, mappingAssetName);
            m_MappingAssetCache = new Dictionary<string, IMappingAsset<R>>();
            m_AssetRefer = new Dictionary<string, int>();
            m_ABCacheSubCount = new Dictionary<string, int>();
            m_ABGroups = new Dictionary<string, Dictionary<string, int>>();
        }

        #region  protected

        protected int AddRefer(string abName)
        {
            //引用计数+1
            if (!m_AssetRefer.ContainsKey(abName))
            {
                m_AssetRefer.Add(abName, 1);
                return 1;
            }
            else
                return m_AssetRefer[abName]++;
        }

        protected int AddGroup(string abGrounpName, string abName)
        {
            Dictionary<string, int> abReferCount;
            if (!m_ABGroups.TryGetValue(abGrounpName, out abReferCount))
            {
                abReferCount = DictionaryPool<string, int>.Get();
                m_ABGroups.Add(abGrounpName, abReferCount);
            }

            if (!abReferCount.ContainsKey(abName))
            {
                abReferCount.Add(abName, 1);
                return 1;
            }
            else
                return abReferCount[abName]++;
        }

        protected abstract string GetMappingAssetName();
        #endregion

        public R GetAsset(string assetName, string groupName = "")
        {
            IMappingAsset<R> mappingAsset;
            //find assetbundle name
            string abName = m_AssetBundleMappingAsset.GetAassetBundleName(assetName);
            if (string.IsNullOrEmpty(abName))
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("{1} doesn't contains asset {0}.", assetName, typeof(IMappingAsset<R>));
#endif
                return null;
            }

            int refCount = AddRefer(abName);

            if (!string.IsNullOrEmpty(groupName))
                AddGroup(groupName, abName);

            if (m_MappingAssetCache.TryGetValue(abName, out mappingAsset))
            {
                return mappingAsset.GetAsset(assetName);
            }
            else
            {
                mappingAsset = ResourcesLoader.LoadAsset<IMappingAsset<R>>(abName, Utils.CUtils.GetAssetName(abName));
                m_MappingAssetCache.Add(abName, mappingAsset);
                return mappingAsset.GetAsset(assetName);
            }
        }

        public void GetAssetAsync(string assetName, System.Action<R> onComplete, string groupName = "")
        {
            IMappingAsset<R> mappingAsset;
            //find assetbundle name
            string abName = m_AssetBundleMappingAsset.GetAassetBundleName(assetName);
            if (string.IsNullOrEmpty(abName))
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("{1} doesn't contains asset {0}.", assetName, typeof(IMappingAsset<R>));
#endif
                return;
            }

            int refCount = AddRefer(abName);

            if (!string.IsNullOrEmpty(groupName))
                AddGroup(groupName, abName);

            if (m_MappingAssetCache.TryGetValue(abName, out mappingAsset))
            {
                var asset = mappingAsset.GetAsset(assetName);
                onComplete(asset);
                #if UNITY_EDITOR
                if(asset == null)
                {
                    Debug.LogWarningFormat("{0} doesn't contains asset {1}.",mappingAsset,assetName);
                }
                #endif
            }
            else
            {
                Action<object, object> onComplete1 = (data, userData) =>
                {
                    IMappingAsset<R> mappingAsset1 = (IMappingAsset<R>)data;
                    if (!m_MappingAssetCache.ContainsKey(abName))
                        m_MappingAssetCache.Add(abName, mappingAsset1);
                    R asset = mappingAsset1.GetAsset(assetName);
                    onComplete(asset);

                    //keep reference count as 1
                    int count = 0;
                    if (m_ABCacheSubCount.TryGetValue(abName, out count))
                    {
                        for (int i = 0; i < count; i++)
                            CacheManager.Subtract(abName);
                        m_ABCacheSubCount.Remove(abName);
                    }
                };

                m_ABCacheSubCount[abName] = refCount;//记录异步加载次数
                ResourcesLoader.LoadAssetAsync(abName, null, typeof(IMappingAsset<R>), onComplete1, null);
            }
        }

        public void Subtract(string assetName)
        {
            if (string.IsNullOrEmpty(assetName)) return;

            string abName = m_AssetBundleMappingAsset.GetAassetBundleName(assetName);
            int count = 0;
            if (abName != null && m_AssetRefer.TryGetValue(abName, out count))
            {
                count = --m_AssetRefer[abName];
                if (count <= 0)
                {
                    m_MappingAssetCache.Remove(abName);
                    m_AssetRefer.Remove(abName);
                    CacheManager.Subtract(abName);
                }
            }
        }

        public void SubtractGroup(string groupName)
        {
            Dictionary<string, int> abGroups;
            if (m_ABGroups.TryGetValue(groupName, out abGroups))
            {
                string abName;
                int count;
                foreach (var kv in abGroups)
                {
                    abName = kv.Key;
                    count = kv.Value;
                    count = m_AssetRefer[abName] -= count;
                    if (count <= 0)
                    {
                        m_MappingAssetCache.Remove(abName);
                        m_AssetRefer.Remove(abName);
                        CacheManager.Subtract(abName);
                    }
                }
                DictionaryPool<string, int>.Release(abGroups);
                m_ABGroups.Remove(groupName);
            }

        }

        public override void Dispose()
        {
            string groupName;
            foreach (var kv in m_ABGroups)
            {
                groupName = kv.Key;
                SubtractGroup(groupName);
            }
            m_ABGroups.Clear();
            m_AssetBundleMappingAsset.Dispose();
            m_AssetRefer.Clear();
            m_MappingAssetCache.Clear();
        }
    }
}
