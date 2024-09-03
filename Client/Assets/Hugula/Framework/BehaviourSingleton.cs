using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Framework
{
    [XLua.LuaCallCSharp]
    public abstract class BehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T m_Instance;
        public static T instance
        {
            get
            {
                if (m_Instance == null && BehaviourSingletonManager.m_CanCreateInstance)//Double-Check Locking 
                {
#if UNITY_EDITOR
                    Debug.LogErrorFormat("BehaviourSingleton<{0}> instance from new GameObject（） not support ,frame:{1} ", typeof(T).Name, Time.frameCount);
#endif
                    // var find = GameObject.Find("/BehaviourSingleton");
                    // if (find == null)
                    // {
                    //     find = new GameObject("BehaviourSingleton");
                    //     DontDestroyOnLoad(find);
                    // }
                    // find.CheckAddComponent<T>();


                }

                return m_Instance;
            }
        }

        public static bool Created
        {
            get
            {
                return m_Instance != null;
            }
        }

        protected virtual void Awake()
        {
            if (m_Instance)
            {
                BehaviourSingletonManager.RemoveInstance(m_Instance);
                if (m_Instance != this)
                    Destroy(m_Instance);
            }
            #if UNITY_EDITOR    
            else
            {
                Debug.LogWarningFormat("BehaviourSingleton<{0}> instance already exists check Destroy=({1}) . frame:{2} ", typeof(T).Name,m_Instance != this,Time.frameCount);
            }
            #endif

            m_Instance = this as T;
            BehaviourSingletonManager.AddInstance(m_Instance);
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