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
        /// 对应的key索引
        /// </summary>
        public int[] keys;

        /// <summary>
        /// 对应的key原始值
        /// </summary>
        public string[] sourceKeys;

        private Dictionary<int, int> m_Map;

        /// <summary>
        /// 获取当前sprite对应的atlas key
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetKey(string name)
        {
            if (m_Map == null)
            {
                m_Map = new Dictionary<int, int>(names.Length);
                int m_AssetName;
                // string m_ABName;
                int index =-1;
                for (int i = 0; i < names.Length; i++)
                {
                    index = keys[i];
                    m_AssetName = names[i];
                    if (m_Map.ContainsKey(m_AssetName))
                        continue;
                    m_Map.Add(m_AssetName, index);
                }
            }
            int idx = UnityEngine.Animator.StringToHash(name);
            if(m_Map.TryGetValue(idx,out var atlasIndex))
            {
                return sourceKeys[atlasIndex];
            }
            return null;
        }

        public void Dispose()
        {
            names = null;
            keys = null;
            sourceKeys = null;
        }
    }

    public interface IMappingAsset<T>
    {
        T GetAsset(string name);
    }
}