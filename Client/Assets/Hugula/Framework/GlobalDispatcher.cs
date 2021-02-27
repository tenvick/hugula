using System;
using System.Collections;
using System.Collections.Generic;
using Hugula.Framework;
using UnityEngine;

namespace Hugula.Framework
{

    ///<summary>
    /// 全局事件派发
    ///</summary>
    public class GlobalDispatcher : Singleton<GlobalDispatcher>, IDisposable
    {
        private Dictionary<DispatcherEvent, List<object>> m_Dispatcher = new Dictionary<DispatcherEvent, List<object>>();
         
        public override void Dispose()
        {
            foreach (var i in m_Dispatcher)
            {
                i.Value.Clear();
            }
            m_Dispatcher.Clear();
            base.Dispose();
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
            AddListener<object>(key, action);
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

        public void Dispatch(DispatcherEvent key, object arg)
        {
            Dispatch<object>(key, arg);
        }

        public void Dispatch<T>(DispatcherEvent key, T arg)
        {
            List<object> events = null;
            if (m_Dispatcher.TryGetValue(key, out events))
            {
                object e = null;
                // for (int i = events.Count - 1; i >= 0; i--)
                for (int i = 0; i < events.Count; i++)
                {
                    if (events.Count > i) // for events.Clear()
                    {
                        e = events[i];
                        if (e is System.Action<T>)
                            ((System.Action<T>)e)(arg);
                        else if (e is System.Action<object>)
                            ((System.Action<object>)e)(arg);
                    }
                }

            }
        }

    }


    ///<summary>
    /// 全局事件派发泛型展开
    ///</summary>
    public static class GlobalDispatcherExtension
    {
        public static void AddListenerVector3(this GlobalDispatcher self, DispatcherEvent key, System.Action<Vector3> action)
        {
            self.AddListener<Vector3>(key, action);
        }

        public static void RemoveListenerVector3(this GlobalDispatcher self, DispatcherEvent key, System.Action<Vector3> action)
        {
            self.RemoveListener<Vector3>(key, action);
        }

        public static void DispatchVector3(this GlobalDispatcher self, DispatcherEvent key, Vector3 arg)
        {
            self.Dispatch<Vector3>(key, arg);
        }

        public static void AddListenerVector2(this GlobalDispatcher self, DispatcherEvent key, System.Action<Vector2> action)
        {
            self.AddListener<Vector2>(key, action);
        }

        public static void RemoveListenerVector2(this GlobalDispatcher self, DispatcherEvent key, System.Action<Vector2> action)
        {
            self.RemoveListener<Vector2>(key, action);
        }

        public static void DispatchVector2(this GlobalDispatcher self, DispatcherEvent key, Vector2 arg)
        {
            self.Dispatch<Vector2>(key, arg);
        }

        public static void AddListenerBool(this GlobalDispatcher self, DispatcherEvent key, System.Action<bool> action)
        {
            self.AddListener<bool>(key, action);
        }

        public static void RemoveListenerBool(this GlobalDispatcher self, DispatcherEvent key, System.Action<bool> action)
        {
            self.RemoveListener<bool>(key, action);
        }

        public static void DispatchBool(this GlobalDispatcher self, DispatcherEvent key, bool arg)
        {
            self.Dispatch<bool>(key, arg);
        }

        public static void AddListenerInt(this GlobalDispatcher self, DispatcherEvent key, System.Action<int> action)
        {
            self.AddListener<int>(key, action);
        }

        public static void RemoveListenerInt(this GlobalDispatcher self, DispatcherEvent key, System.Action<int> action)
        {
            self.RemoveListener<int>(key, action);
        }

        public static void DispatchInt(this GlobalDispatcher self, DispatcherEvent key, int arg)
        {
            self.Dispatch<int>(key, arg);
        }

        public static void AddListenerFloat(this GlobalDispatcher self, DispatcherEvent key, System.Action<float> action)
        {
            self.AddListener<float>(key, action);
        }

        public static void RemoveListenerFloat(this GlobalDispatcher self, DispatcherEvent key, System.Action<float> action)
        {
            self.RemoveListener<float>(key, action);
        }

        public static void DispatchFloat(this GlobalDispatcher self, DispatcherEvent key, float arg)
        {
            self.Dispatch<float>(key, arg);
        }
    }

}