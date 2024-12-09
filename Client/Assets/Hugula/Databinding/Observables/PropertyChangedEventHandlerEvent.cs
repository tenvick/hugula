#define USE_LIST_HANDLE_EVENT
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine.Animations;

namespace Hugula.Databinding
{
    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public class PropertyChangedEventHandlerEvent
    {
#if USE_LIST_HANDLE_EVENT
        readonly HashSet<Action<object, string>> m_Events = new HashSet<Action<object, string>>();
#else
        Dictionary<int, PropertyChangedEventHandler> m_Events = new Dictionary<int, PropertyChangedEventHandler>(12);

        int EmptyKey = UnityEngine.Animator.StringToHash("");
#endif
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

        public void Invoke(object sender, string property)
        {
#if USE_LIST_HANDLE_EVENT
            Action<object, string> hander = null;
            int i = 0;
            int count = m_Events.Count;
            var getEnumerator = m_Events.GetEnumerator();
            while (getEnumerator.MoveNext())
            {
                hander = getEnumerator.Current;
                if (hander.Target is Binding binding)
                {
                    if (!binding.CheckInvoke(property)) //如果不能调用则跳过
                    {
                        continue;
                    }
                }
                hander(sender, property);

                if (count > m_Events.Count)
                    count = m_Events.Count;
            }           
#else

            Action<object,string> hander events = null;
            int id = UnityEngine.Animator.StringToHash(property);

            if (m_Events.TryGetValue(id, out events))
            {
                events?.Invoke(sender, property);
            }
            if (m_Events.TryGetValue(EmptyKey, out events))
            {
                events?.Invoke(sender, property);
            }
#endif
        }

        public void Add(Action<object, string> hander, string property = "")
        {
            if (hander == null)
                throw new System.NullReferenceException("Add(hander) argment hander is null)");

#if USE_LIST_HANDLE_EVENT
            m_Events.Add(hander);
#else
            int id = UnityEngine.Animator.StringToHash(property);

            if (m_Events.TryGetValue(id, out var events))
            {
                events += hander;
                m_Events[id] = events; //
            }
            else
            {
                m_Events.Add(id, hander);
            }
#endif

        }

        public void Remove(Action<object, string> hander, string property = "")
        {
#if USE_LIST_HANDLE_EVENT
            m_Events.Remove(hander);
#else
            int id = UnityEngine.Animator.StringToHash(property);

            if (m_Events.TryGetValue(id, out var events))
            {
                events -= hander;
                if (events == null)
                {
                    m_Events.Remove(id);
                }
                else
                {
                    m_Events[id] = events;
                }
            }
#endif
        }


        public static PropertyChangedEventHandlerEvent operator +(PropertyChangedEventHandlerEvent dele, Action<object,string> hander)
        {
            dele.Add(hander);
            return dele;
        }

        public static PropertyChangedEventHandlerEvent operator -(PropertyChangedEventHandlerEvent dele, Action<object,string> hander)
        {
            dele.Remove(hander);
            return dele;
        }

        // public static PropertyChangedEventHandlerEvent operator +(PropertyChangedEventHandlerEvent dele, (string propertyName, PropertyChangedEventHandler handler) tuple)
        // {
        //     dele.Add(tuple.propertyName, tuple.handler);
        //     return dele;
        // }

        // public static PropertyChangedEventHandlerEvent operator -(PropertyChangedEventHandlerEvent dele, (string propertyName, PropertyChangedEventHandler handler) tuple)
        // {
        //     dele.Remove(tuple.propertyName, tuple.handler);
        //     return dele;
        // }

        #region  pool

        public static PropertyChangedEventHandlerEvent Get()
        {
            return m_PropertyChangedPool.Get();
        }

        public static void Release(PropertyChangedEventHandlerEvent obj)
        {
            m_PropertyChangedPool.Release(obj);
        }

        // public static void ClearPool()
        // {

        // }

        static void m_ActionOnRelease(PropertyChangedEventHandlerEvent dele)
        {
            dele.Clear();
        }

        static int capacity = 2048;
        static int initial = 512;

        static Hugula.Utils.ObjectPool<PropertyChangedEventHandlerEvent> m_PropertyChangedPool = new Hugula.Utils.ObjectPool<PropertyChangedEventHandlerEvent>(null, m_ActionOnRelease, capacity, initial);

        #endregion
    }

}