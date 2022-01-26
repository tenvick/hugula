using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Databinding;
using UnityEngine;

namespace Hugula.Converter
{
    [XLua.LuaCallCSharp]
    public class StringToSprite : MonoBehaviour, IValueConverter
    {
        [SerializeField]
        Sprite[] sprites;

        [SerializeField] bool m_Debug = false;

        private Dictionary<string, Sprite> m_Map;

        public Sprite GetSprite(string name)
        {
            if (m_Map == null)
            {
                m_Map = new Dictionary<string, Sprite>(sprites.Length);
                string m_Name;
                Sprite m_Sprite;
                for (int i = 0; i < sprites.Length; i++)
                {
                    m_Sprite = sprites[i];
                    m_Name = i.ToString(); //以索引为key
                    if (m_Map.ContainsKey(m_Name))
                        continue;
                    m_Map.Add(m_Name, m_Sprite);
                }
            }
            Sprite re = null;
            m_Map.TryGetValue(name, out re);
            return re;
        }

        public object Convert(object value, Type targetType)
        {
            var sprite = GetSprite(value.ToString());
#if UNITY_EDITOR
            if (m_Debug) Debug.LogFormat(" Convert {0} to {1}; {2}", value, sprite, this);
#endif
            return sprite;
        }

        public object ConvertBack(object value, Type targetType)
        {
            if (value is Sprite)
                return ((Sprite)value).name;

            return string.Empty;
        }

        void Awake()
        {
            ValueConverterRegister.instance?.AddConverter(this.GetType().Name, this);
        }

        void OnDestroy()
        {
            ValueConverterRegister.instance?.RemoveConverter(this.GetType().Name);

            if (m_Map != null)
                m_Map.Clear();

        }

    }
}