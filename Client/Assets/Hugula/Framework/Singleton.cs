using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Hugula.Framework
{
    ///<summary>
    ///单例模式
    ///</summary>
    [XLua.LuaCallCSharp]
    public abstract class Singleton<T> where T : class, IDisposable
    {
        protected static T m_Instance;
        public static readonly object SyncObject = new object();
        public static T instance
        {
            get
            {
                if (m_Instance == null && SingletonManager.m_CanCreateInstance)
                {
                    lock (SyncObject)
                    {
                        if (m_Instance == null)//Double-Check Locking 
                        {
                            m_Instance = (T)Activator.CreateInstance(typeof(T), true);
                            SingletonManager.AddInstance(m_Instance);

                        }
                    }
                }
                return m_Instance;
            }

        }

        /// <summary>
        /// 重置状态
        /// </summary>
        public abstract void Reset();
        // {

        // }

        public virtual void Dispose()
        {
            SingletonManager.RemoveInstance(m_Instance);
            m_Instance = null;
        }
    }

    internal static class SingletonManager
    {
        internal static bool m_CanCreateInstance = true;

        static List<IDisposable> m_Instances = new List<IDisposable>();

        internal static void AddInstance(IDisposable t)
        {
            m_Instances.Add(t);
        }

        internal static void RemoveInstance(IDisposable t)
        {
            m_Instances.Remove(t);
        }

        internal static void CanCreateInstance()
        {
            m_CanCreateInstance = true;
        }

        internal static void DisposeAll()
        {
            m_CanCreateInstance = false; //标记为不可以创建
            int i = 0;
            int count = m_Instances.Count;
            while (i < count)
            {
#if UNITY_EDITOR
                Debug.LogFormat("Singleton.Destroy({0},{1}) remain={2} ", i, m_Instances[i]?.GetType().Name, m_Instances.Count);
#endif
                m_Instances[i]?.Dispose();

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
