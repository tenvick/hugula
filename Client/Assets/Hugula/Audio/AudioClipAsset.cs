using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Hugula;

namespace Hugula.Audio
{

    /// <summary>
    /// AudioClip Prefab
    /// </summary>
    public class AudioClipAsset : ScriptableObject, IMappingAsset<AudioClip>
    {
        /// <summary>
        /// 所有音效的默认clipasset集合名称
        /// </summary>
        public const string DEFALUT_SOUND_ASSET_NAME = "sound_audio";
        public AudioClip[] audioClips;
        public int[] audioNames;

        private Dictionary<int, AudioClip> m_Map;

        public AudioClip GetAsset(string name)
        {
            if (m_Map == null)
            {
                m_Map = new Dictionary<int, AudioClip>(audioClips.Length);
                int m_Name;
                AudioClip m_audioClip;
                for (int i = 0; i < audioNames.Length; i++)
                {
                    m_audioClip = audioClips[i];
                    m_Name = audioNames[i];
                    if (m_Map.ContainsKey(m_Name))
                        continue;
                    m_Map.Add(m_Name, m_audioClip);
                }
            }
            int idx = UnityEngine.Animator.StringToHash(name);
            AudioClip re = null;
            m_Map.TryGetValue(idx, out re);
            return re;
        }

    }
}