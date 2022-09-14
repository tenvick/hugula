using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Hugula.Databinding
{
    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public class NotifyCollectionChangedEventHandlerEvent
    {
        List<NotifyCollectionChangedEventHandler> m_Events = new List<NotifyCollectionChangedEventHandler>();

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

        public void Invoke(object sender, HugulaNotifyCollectionChangedEventArgs arg)
        {
            NotifyCollectionChangedEventHandler hander = null;
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

        public void Add(NotifyCollectionChangedEventHandler hander)
        {
            if (hander == null)
                throw new System.NullReferenceException("Add(hander) argment hander is null)");

            m_Events.Add(hander);
        }

        public void Remove(NotifyCollectionChangedEventHandler hander)
        {
            m_Events.Remove(hander);
        }

        public static NotifyCollectionChangedEventHandlerEvent operator +(NotifyCollectionChangedEventHandlerEvent dele, NotifyCollectionChangedEventHandler hander)
        {
            dele.Add(hander);
            return dele;
        }

        public static NotifyCollectionChangedEventHandlerEvent operator -(NotifyCollectionChangedEventHandlerEvent dele, NotifyCollectionChangedEventHandler hander)
        {
            dele.Remove(hander);
            return dele;
        }
    }
}