using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Framework
{
    public interface IManager
    {
        void Initialize();
        void Terminate();
    }

    ///<summary>
    ///manager 对各种manager的生命周期管理
    ///</summary>
    [XLua.LuaCallCSharp]
    public class Manager : MonoBehaviour
    {
        #region  static
        static bool m_Initialize = false;
        internal static Manager m_Instance;
        public static Manager instance
        {
            get
            {
                if (m_Instance == null && BehaviourSingletonManager.m_CanCreateInstance)
                {
                    var gObj = new GameObject("Manager");
                    GameObject.DontDestroyOnLoad(gObj);
                    m_Instance = gObj.AddComponent<Manager>();
                }
                return m_Instance;
            }
        }
        static List<IManager> m_Register = new List<IManager>();
        static Dictionary<string, IManager> m_Initialized = new Dictionary<string, IManager>();

        public static void Register<T>() where T : IManager
        {
            Type tp = typeof(T);
            var ins = instance.CreateManager(tp);
            if (ins == null)
            {
                Debug.LogErrorFormat("Manager register can't find type {0} ", typeof(T));
                return;
            }
            m_Register.Add(ins);
            if (m_Initialize) Initialize();
        }

        public static void UnRegister<T>() where T : IManager
        {
            Type tp = typeof(T);
            IManager reged = null;
            if (m_Initialized.TryGetValue(tp.Name, out reged))
            {
                reged.Terminate();
                if (tp.IsAssignableFrom(typeof(MonoBehaviour)))
                {
                    GameObject.Destroy((MonoBehaviour)reged);
                }
                m_Initialized.Remove(tp.Name);
            }
        }

        internal static void Add(Type type, IManager manager)
        {
            string name = type.Name;
            if (m_Initialized.TryGetValue(name, out var exist))
            {
#if UNITY_EDITOR
                Debug.LogWarningFormat("type {0},{1} is already existed in Managers ", name, exist);
#endif
                m_Initialized.Remove(name);
                exist?.Terminate();
            }
            manager.Initialize();
            m_Initialized.Add(name, manager);
        }

        internal static void Remove(Type type)
        {
            IManager reged = null;
            if (m_Initialized.TryGetValue(type.Name, out reged))
            {
                reged.Terminate();
                // if (type.IsAssignableFrom(typeof(MonoBehaviour)))
                // {
                //     GameObject.Destroy((MonoBehaviour)reged);
                // }
                m_Initialized.Remove(type.Name);
            }
        }

        public static IManager Get(string name)
        {
            IManager reged = null;
            if (m_Initialized.TryGetValue(name, out reged))
            {
                return reged;
            }
            return null;
        }

        public static T Get<T>() where T : IManager
        {
            IManager reged = null;
            if (m_Initialized.TryGetValue(typeof(T).Name, out reged))
            {
                return (T)reged;
            }

            return default(T);
        }

        ///<summary>
        /// 销毁所有Mangaer
        ///</summary>
        public static void Terminate()
        {
            var kv = m_Initialized.Values;
            int i = 0;
            List<IManager> all = new List<IManager>(kv);
            int count = all.Count;
            IManager v = null;
            while (i < count)
            {
                v = all[i];
                i++;
#if UNITY_EDITOR
                Debug.LogFormat("Manager.Terminate {0} frame:{1} ", v != null ? v.GetType().Name : "", Time.frameCount);
#endif
                if (v != null && v is MonoBehaviour)
                {
                    var mono = (MonoBehaviour)v;
                    if (mono)
                    {
                        var gobj = mono.gameObject;
                        if (gobj)
                        {
#if UNITY_EDITOR
                            Debug.LogFormat("Manager.Terminate.Destroy {0} frame:{1}", gobj, Time.frameCount);
#endif
                            GameObject.Destroy(gobj);
                        }
                    }
                }
                else if (v != null)
                {
                    v.Terminate();
                }
            }

            m_Initialized.Clear();
        }

        public static void Initialize()
        {
            var kv = m_Register;
            foreach (var v in kv)
            {
                m_Initialized.Add(v.GetType().Name, v);
                v.Initialize();
            }
            m_Register.Clear();
            m_Initialize = true;
        }
        #endregion

        void Awake()
        {
            if (m_Instance == null) m_Instance = this;
        }

        void OnDestroy()
        {
            Terminate();
        }

        IManager CreateManager(Type type)
        {
            IManager ret = null;
            if (type.IsSubclassOf(typeof(MonoBehaviour)))
                ret = gameObject.AddComponent(type) as IManager;
            else
                ret = Activator.CreateInstance(type) as IManager;
            return ret;
        }
    }


    /// <summary>
    ///Manager  MonoBehaviour base
    /// </summary>
    public abstract class MonoBehaviourManager : MonoBehaviour, IManager
    {
        #region  imanger
        public abstract void Initialize();

        public abstract void Terminate();
        #endregion

        protected virtual void Awake()
        {
            if (Application.isPlaying)
                Manager.Add(this.GetType(), this);
        }

        protected virtual void OnDestroy()
        {
            Manager.Remove(this.GetType());
        }
    }

}