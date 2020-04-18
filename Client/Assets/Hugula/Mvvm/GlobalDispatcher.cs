using System.Collections;
using System.Collections.Generic;
using Hugula.Framework;
using UnityEngine;

namespace Hugula.Mvvm {

    ///<summary>
    /// 全局事件派发
    ///</summary>
    public class GlobalDispatcher : IManager {
        public delegate void TriggerEvent (object arg);
        private Dictionary<int, List<TriggerEvent>> m_Dispatcher = new Dictionary<int, List<TriggerEvent>> ();
        public void Initialize () {

        }

        public void Terminate () {
            m_Dispatcher.Clear ();
        }

        public void AddListener (int key, TriggerEvent action) {
            List<TriggerEvent> events = null;
            if (!m_Dispatcher.TryGetValue (key, out events)) {
                events = new List<TriggerEvent> ();
                m_Dispatcher.Add (key, events);
            }

            events.Add (action);
        }

        public void RemoveListener (int key, TriggerEvent action) {
            List<TriggerEvent> events = null;
            if (m_Dispatcher.TryGetValue (key, out events)) {
                events.Remove (action);
            }
        }

        public void Dispatcher (int key, object arg) {
            List<TriggerEvent> events = null;
            if (m_Dispatcher.TryGetValue (key, out events)) {
                foreach (var e in events)
                    e (arg);
            }
        }

    }

}