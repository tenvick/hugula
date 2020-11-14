using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Hugula
{
    /// <summary>
    /// 资源与key的映射。
    /// 用于sprite与atlas名字映射
    /// </summary>
    public class MappingAsset : ScriptableObject, IDisposable
    {
        /// <summary>
        /// 名称
        /// </summary>
        public int[] names;
        /// <summary>
        /// 对应的key
        /// </summary>
        public string[] keys;

        private Dictionary<int, string> m_Map;

        /// <summary>
        /// 获取当前sprite对应的atlas key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetKey(string name)
        {
            if (m_Map == null)
            {
                m_Map = new Dictionary<int, string>(names.Length);
                int m_AssetName;
                string m_ABName;
                for (int i = 0; i < names.Length; i++)
                {
                    m_ABName = keys[i];
                    m_AssetName = names[i];
                    if (m_Map.ContainsKey(m_AssetName))
                        continue;
                    m_Map.Add(m_AssetName, m_ABName);
                }
            }
            int idx = UnityEngine.Animator.StringToHash(name);
            string re = null;
            // Debug.LogFormat("StringToHash:{0} ", idx);
            m_Map.TryGetValue(idx, out re);
            return re;
        }

        public void Dispose()
        {
            names = null;
            keys = null;
        }
    }

}