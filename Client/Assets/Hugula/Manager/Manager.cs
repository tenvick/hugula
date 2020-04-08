using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Manager {
    public interface IManager {
        void Initialize ();
        void Terminate ();
    }

    ///<summary>
    ///manager 对各种manager的生命周期管理
    ///</summary>
    public class Manager : MonoBehaviour {
        #region  static
        static bool m_Initialize = false;
        static Manager m_Instance;
        static Manager instance {
            get {
                if (m_Instance == null) {
                    var gObj = new GameObject ("Manager");
                    GameObject.DontDestroyOnLoad (gObj);
                    m_Instance = gObj.AddComponent<Manager> ();
                }
                return m_Instance;
            }
        }
        static List<IManager> m_Register = new List<IManager> ();
        static Dictionary<string, IManager> m_Initialized = new Dictionary<string, IManager> ();

        public static void Register<T> () where T : IManager {
            Type tp = typeof (T);
            var ins = instance.CreateManager (tp);
            if (ins == null) {
                Debug.LogErrorFormat ("Manager register can't find type {0} ", typeof (T));
                return;
            }
            m_Register.Add (ins);
            if (m_Initialize) Initialize ();
        }

        public static void UnRegister<T> () where T : IManager {
            Type tp = typeof (T);
            IManager reged = null;
            if (m_Initialized.TryGetValue (tp.Name, out reged)) {
                reged.Terminate ();
                if (tp.IsAssignableFrom (typeof (MonoBehaviour))) {
                    GameObject.Destroy ((MonoBehaviour) reged);
                }
                m_Initialized.Remove (tp.Name);
            }
        }

        internal static void Add(Type type, IManager manager)
        {
            string name = type.Name;
            if (m_Initialized.TryGetValue(name, out var exist))
            {
                exist.Terminate();
                Debug.LogWarningFormat("type {0},{1} is already existed in Managers ", name, exist);
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
            if (m_Initialized.TryGetValue (name, out reged)) {
                return reged;
            }
            return null;
        }

        public static T Get<T> () where T : IManager {
            IManager reged = null;
            if (m_Initialized.TryGetValue (typeof (T).Name, out reged)) {
                return (T) reged;
            }

            return default (T);
        }

        ///<summary>
        /// 销毁所有Mangaer
        ///</summary>
        public static void Terminate () {
            var kv = m_Initialized.Values;
            foreach (var v in kv) {
                v.Terminate ();
                if (v.GetType ().IsAssignableFrom (typeof (MonoBehaviour))) {
                    GameObject.Destroy ((MonoBehaviour) v);
                }
            }

            m_Initialized.Clear ();
        }

        public static void Initialize () {
            var kv = m_Register;
            foreach (var v in kv) {
                v.Initialize ();
                m_Initialized.Add (v.GetType ().Name, v);
            }
            m_Register.Clear ();
            m_Initialize = true;
        }
        #endregion

        void Awake () {
            if (m_Instance == null) m_Instance = this;
        }

        void OnDestory () {
            Terminate ();
        }

        IManager CreateManager (Type type) {
            IManager ret = null;
            if (type.IsSubclassOf (typeof (MonoBehaviour)))
                ret = gameObject.AddComponent (type) as IManager;
            else
                ret = Activator.CreateInstance (type) as IManager;
            return ret;
        }
    }

}