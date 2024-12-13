#define USE_LIST_HANDLE_EVENT
// #define ENABLE_FOREACH_MODIFY
// #define DEBUG_FOREACH_MODIFY
using System;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using UnityEngine.Animations;
using Hugula.Utils;
using UnityEngine;

namespace Hugula.Databinding
{
    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public class PropertyChangedEventHandlerEvent
    {
#if USE_LIST_HANDLE_EVENT
        readonly HashSet<Action<object, string>> m_Events = new HashSet<Action<object, string>>();
#if DEBUG_FOREACH_MODIFY
        bool m_IsInvoke = false;
#endif
#else
        Dictionary<int, PropertyChangedEventHandler> m_Events = new Dictionary<int, PropertyChangedEventHandler>(12);

        static int EmptyKey = UnityEngine.Animator.StringToHash("");
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
#if USE_LIST_HANDLE_EVENT && ENABLE_FOREACH_MODIFY
            List<Action<object, string>> invoking = ListPool<Action<object, string>>.Get();
            try
            {
                //filter
                foreach (var hander in m_Events)
                {
                    if (hander.Target is Binding binding)
                    {
                        if (!binding.CheckInvoke(property)) //
                        {
                            continue;
                        }
                    }
                    invoking.Add(hander);
                }

                //invoke
                foreach (var hander in invoking)
                {
                    hander(sender, property);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
            finally
            {
                ListPool<Action<object, string>>.Release(invoking);
            }
#elif USE_LIST_HANDLE_EVENT

#if DEBUG_FOREACH_MODIFY
            m_IsInvoke = true;
#endif
            foreach (var hander in m_Events)
            {
                if (hander.Target is Binding binding)
                {
                    if (!binding.CheckInvoke(property)) //如果不能调用则跳过
                    {
                        continue;
                    }
                }

                hander(sender, property);
            }
#if DEBUG_FOREACH_MODIFY
            m_IsInvoke = false;
#endif
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
#if DEBUG_FOREACH_MODIFY
            if(m_IsInvoke && hander.Target is Binding binding)
                Debug.LogError($"Add({hander},{property}) is invoke {binding} \r\n {CUtils.GetFullPath((MonoBehaviour)binding.target)}");
#endif
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
#if DEBUG_FOREACH_MODIFY
            if(m_IsInvoke && hander.Target is Binding binding)
                Debug.LogError($"Remove({hander},{property}) is invoke {binding} \r\n {CUtils.GetFullPath((MonoBehaviour)binding.target)}");
#endif
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


        public static PropertyChangedEventHandlerEvent operator +(PropertyChangedEventHandlerEvent dele, Action<object, string> hander)
        {
            dele.Add(hander);
            return dele;
        }

        public static PropertyChangedEventHandlerEvent operator -(PropertyChangedEventHandlerEvent dele, Action<object, string> hander)
        {
            dele.Remove(hander);
            return dele;
        }

        #region  pool

        public static PropertyChangedEventHandlerEvent Get()
        {
            return m_PropertyChangedPool.Get();
        }

        public static void Release(PropertyChangedEventHandlerEvent obj)
        {
            m_PropertyChangedPool.Release(obj);
        }

        static void m_ActionOnRelease(PropertyChangedEventHandlerEvent dele)
        {
            dele.Clear();
        }
        static int capacity = 2048;
        static int initial = 512;
        static ObjectPool<PropertyChangedEventHandlerEvent> m_PropertyChangedPool = new ObjectPool<PropertyChangedEventHandlerEvent>(null, m_ActionOnRelease, capacity, initial);

        #endregion
    }

}