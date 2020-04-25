using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Framework;
using UnityEngine;

namespace Hugula.Mvvm {

    ///<summary>
    /// 全局事件派发
    ///</summary>
    public class GlobalDispatcher : IManager
    {
        private Dictionary<DispatcherEvent, List<object>> m_Dispatcher = new Dictionary<DispatcherEvent, List<object>>();
        public void Initialize()
        {

        }

        public void Terminate()
        {
            m_Dispatcher.Clear();
        }

        public void AddListener<T>(DispatcherEvent key, System.Action<T> action)
        {
            List<object> events = null;
            if (!m_Dispatcher.TryGetValue(key, out events))
            {
                events = new List<object>();//new List<TriggerEvent>();
                m_Dispatcher.Add(key, events);
            }

            events.Add(action);
        }

        public void RemoveListener<T>(DispatcherEvent key, System.Action<T> action)
        {
            List<object> events = null;
            if (m_Dispatcher.TryGetValue(key, out events))
            {
                events.Remove(action);
            }
        }

        public void AddListener(DispatcherEvent key, System.Action<object> action)
        {
            List<object> events = null;
            if (!m_Dispatcher.TryGetValue(key, out events))
            {
                events = new List<object>();
                m_Dispatcher.Add(key, events);
            }

            events.Add(action);
        }

        public void RemoveListener(DispatcherEvent key, System.Action<object> action)
        {
            List<object> events = null;
            if (m_Dispatcher.TryGetValue(key, out events))
            {
                events.Remove(action);
            }
        } 

        public void RemoveListenerByKey(DispatcherEvent key)
        {
            List<object> events = null;
            if (m_Dispatcher.TryGetValue(key, out events))
            {
                events.Clear(); 
            }
        }

        public void Dispatcher(DispatcherEvent key, object arg)
        {
            List<object> events = null;
            if (m_Dispatcher.TryGetValue(key, out events))
            {
                object e = null;
                for (int i = events.Count - 1; i >= 0; i--) //倒着执行防止删除后少执行一次bug
                {
                    e = events[i];
                    if (e is System.Action<object>)
                        ((System.Action<object>)e)(arg);
                }
            }
        }

        public void Dispatcher<T>(DispatcherEvent key, T arg)
        {
            List<object> events = null;
            if (m_Dispatcher.TryGetValue(key, out events))
            {
                object e = null;
                for (int i = events.Count - 1; i >= 0; i--)
                {
                    e = events[i];
                    if (e is System.Action<T>)
                        ((System.Action<T>)e)(arg);
                }

            }
        }



        // public void AddListener(DispatcherEnum event, System.Action<object> action )
        // {

        // }
    }


}