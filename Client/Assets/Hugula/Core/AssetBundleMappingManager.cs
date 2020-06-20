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
        private Dictionary<string, IMappingAsset<R>> m_AudioClipAssetCache;
        private Dictionary<string, int> m_AudioClipAssetRefer;
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
            m_AudioClipAssetCache = new Dictionary<string, IMappingAsset<R>>();
            m_AudioClipAssetRefer = new Dictionary<string, int>();
            m_ABCacheSubCount = new Dictionary<string, int>();
            m_ABGroups = new Dictionary<string, Dictionary<string, int>>();
        }

        #region  protected

        protected int AddRefer(string abName)
        {
            //引用计数+1
            if (!m_AudioClipAssetRefer.ContainsKey(abName))
            {
                m_AudioClipAssetRefer.Add(abName, 1);
                return 1;
            }
            else
                return m_AudioClipAssetRefer[abName]++;
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
            IMappingAsset<R> audioClipAsset;
            //find assetbundle name
            string abName = m_AssetBundleMappingAsset.GetAassetBundleName(assetName);
            if (string.IsNullOrEmpty(abName))
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("could't find  asset ({0})'s {1}", assetName, typeof(IMappingAsset<R>));
#endif
                return null;
            }

            int refCount = AddRefer(abName);

            if (!string.IsNullOrEmpty(groupName))
                AddGroup(groupName, abName);

            if (m_AudioClipAssetCache.TryGetValue(abName, out audioClipAsset))
            {
                return audioClipAsset.GetAsset(assetName);
            }
            else
            {
                audioClipAsset = ResourcesLoader.LoadAsset<IMappingAsset<R>>(abName, Utils.CUtils.GetAssetName(abName));
                m_AudioClipAssetCache.Add(abName, audioClipAsset);
                return audioClipAsset.GetAsset(assetName);
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
                Debug.LogWarningFormat("could't find  asset name ({0})'s  assetBundleMappingAsset({1})", assetName, typeof(IMappingAsset<R>));
#endif
                return;
            }

            int refCount = AddRefer(abName);

            if (!string.IsNullOrEmpty(groupName))
                AddGroup(groupName, abName);

            if (m_AudioClipAssetCache.TryGetValue(abName, out mappingAsset))
            {
                onComplete(mappingAsset.GetAsset(assetName));
            }
            else
            {
                Action<object, object> onComplete1 = (data, userData) =>
                {
                    IMappingAsset<R> mappingAsset1 = (IMappingAsset<R>)data;
                    if (!m_AudioClipAssetCache.ContainsKey(abName))
                        m_AudioClipAssetCache.Add(abName, mappingAsset1);
                    R audio = mappingAsset1.GetAsset(assetName);
                    onComplete(audio);

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
            if (abName!=null && m_AudioClipAssetRefer.TryGetValue(abName, out count))
            {
                count = --m_AudioClipAssetRefer[abName];
                if (count <= 0)
                {
                    m_AudioClipAssetCache.Remove(abName);
                    m_AudioClipAssetRefer.Remove(abName);
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
                    count = m_AudioClipAssetRefer[abName] -= count;
                    if (count <= 0)
                    {
                        m_AudioClipAssetRefer.Remove(abName);
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
            m_AudioClipAssetRefer.Clear();
            m_AudioClipAssetCache.Clear();
        }
    }
}
