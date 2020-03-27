using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Atlas
{

    public class AtlasAsset : ScriptableObject
    {
        public List<int> names = new List<int>();
        public List<Sprite> sprites = new List<Sprite>();
        public Texture2D alphaTex;
        private Dictionary<int, Sprite> m_Map;

        public Sprite GetSprite(string name)
        {
            if (m_Map == null)
            {
                m_Map = new Dictionary<int, Sprite>(sprites.Count);
                int m_Name;
                Sprite m_Sprite;
                for (int i = 0; i < names.Count; i++)
                {
                    m_Sprite = sprites[i];
                    m_Name = names[i];
                    if (m_Map.ContainsKey(m_Name))
                        continue;
                    m_Map.Add(m_Name, m_Sprite);
                }
            }
            int idx = UnityEngine.Animator.StringToHash(name);
            Sprite re = null;
            m_Map.TryGetValue(idx, out re);
            return re;
        }
    }
}