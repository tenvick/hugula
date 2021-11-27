using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Framework
{
    public abstract class BehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T m_Instance;
        public static T instance
        {
            get
            {
                if (m_Instance == null && BehaviourSingletonManager.m_CanCreateInstance)//Double-Check Locking 
                {
                    GameObject obj = new GameObject(typeof(T).Name);
                    DontDestroyOnLoad(obj);
                    m_Instance = obj.AddComponent<T>();
                    BehaviourSingletonManager.AddInstance(m_Instance);
                }

                return m_Instance;
            }
        }

        protected virtual void Awake()
        {
            if (m_Instance == null)
            {
                m_Instance = this as T;
                BehaviourSingletonManager.AddInstance(m_Instance);
            }
        }

        protected virtual void OnDestroy()
        {
            BehaviourSingletonManager.RemoveInstance(m_Instance);
            m_Instance = null;
        }

    }

    internal static class BehaviourSingletonManager
    {
        internal static bool m_CanCreateInstance = true;

        static List<MonoBehaviour> m_Instances = new List<MonoBehaviour>();

        internal static void AddInstance(MonoBehaviour t)
        {
            m_Instances.Add(t);
        }

        internal static void RemoveInstance(MonoBehaviour t)
        {
            m_Instances.Remove(t);
        }

        internal static void CanCreateInstance()
        {
            m_CanCreateInstance = true;
        }

        internal static void DisposeAll()
        {
            m_CanCreateInstance = false;
            int i = 0;
            int count = m_Instances.Count;
            MonoBehaviour it;
            while (i < count)
            {
                it = m_Instances[i];
                if (it != null)
                {
                    var gobj = it.gameObject;
                    if (gobj)
                    {
#if UNITY_EDITOR
                        Debug.LogFormat("BehaviourSingleton.Destroy({0},{1}) remain={2} ", i, it.GetType().Name, m_Instances.Count);
#endif
                        GameObject.Destroy(gobj);
                    }
                }

                if (count > m_Instances.Count) count = m_Instances.Count;
                else
                {
                    i++;
                }
            }

            m_Instances.Clear();
        }
    }
}