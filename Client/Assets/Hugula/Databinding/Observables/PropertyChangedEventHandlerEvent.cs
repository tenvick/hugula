using System.Collections;
using System.Collections.Generic;

namespace Hugula.Databinding
{
    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public class PropertyChangedEventHandlerEvent
    {
        List<PropertyChangedEventHandler> m_Events = new List<PropertyChangedEventHandler>();

        public int Count
        {
            get
            {
                return m_Events.Count;
            }
        }
        public void Clear()
        {
            m_Events.Clear();
        }

        public void Invoke(object sender, string arg)
        {
            PropertyChangedEventHandler hander = null;
            int i = 0;
            int count = m_Events.Count;
            while (i < count)
            {
                hander = m_Events[i];
                hander(sender, arg);
                if (count > m_Events.Count) count = m_Events.Count;
                else
                {
                    i++;
                }
            }
        }

        public void Add(PropertyChangedEventHandler hander)
        {
            if (hander == null)
                throw new System.NullReferenceException("Add(hander) argment hander is null)");

            m_Events.Add(hander);
        }

        public void Remove(PropertyChangedEventHandler hander)
        {
            m_Events.Remove(hander);
        }

        public static PropertyChangedEventHandlerEvent operator +(PropertyChangedEventHandlerEvent dele, PropertyChangedEventHandler hander)
        {
            dele.Add(hander);
            return dele;
        }

        public static PropertyChangedEventHandlerEvent operator -(PropertyChangedEventHandlerEvent dele, PropertyChangedEventHandler hander)
        {
            dele.Remove(hander);
            return dele;
        }
    }

}