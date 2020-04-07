using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Hugula.Manager
{
    ///<summary>
    ///单例模式
    ///</summary>
    public class Singleton<T> where T : class ,IDisposable
    {
        protected static T m_Instance;
        public static readonly object SyncObject = new object();
        public static T instance
        {
            get
            {
                if (m_Instance == null)
                {
                    lock (SyncObject)
                    {
                        if (m_Instance == null)//Double-Check Locking 
                        {
                            m_Instance = (T)Activator.CreateInstance(typeof(T), true);
                        }
                    }
                }
                return m_Instance;
            }

        }
    }
}
