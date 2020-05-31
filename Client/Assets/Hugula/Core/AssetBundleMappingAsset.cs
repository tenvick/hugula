using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Hugula
{
    public class AssetBundleMappingAsset : ScriptableObject, IDisposable
    {
        public int[] assetNames;
        public string[] abNames;

        private Dictionary<int, string> m_Map;

        public string GetAassetBundleName(string name)
        {
            if (m_Map == null)
            {
                m_Map = new Dictionary<int, string>(assetNames.Length);
                int m_AssetName;
                string m_ABName;
                for (int i = 0; i < assetNames.Length; i++)
                {
                    m_ABName = abNames[i];
                    m_AssetName = assetNames[i];
                    if (m_Map.ContainsKey(m_AssetName))
                        continue;
                    m_Map.Add(m_AssetName, m_ABName);
                }
            }
            int idx = UnityEngine.Animator.StringToHash(name);
            string re = null;
            m_Map.TryGetValue(idx, out re);
            return re;
        }

        public void Dispose()
        {
            assetNames = null;
            abNames = null;
        }
    }

    public interface IMappingAsset<T>
    {
        T GetAsset(string name);
    }
}