using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Framework {
    public abstract class BehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour {
        protected static T m_Instance;
        public static T instance {
            get {
                if (m_Instance == null) //Double-Check Locking 
                {
                    GameObject obj = new GameObject (typeof (T).Name);
                    DontDestroyOnLoad (obj);
                    m_Instance = obj.AddComponent<T> ();
                }

                return m_Instance;
            }
        }

        protected virtual void Awake () {
            if (m_Instance == null)
                m_Instance = this as T;
        }

        protected virtual void OnDestroy () {
            m_Instance = null;
        }
    }
}