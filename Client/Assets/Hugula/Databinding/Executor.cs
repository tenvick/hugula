using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using UnityEngine;

namespace Hugula.Databinding
{
    public class Executor : MonoBehaviour
    {

        private static List<object> m_Tasks = new List<object>(); //lock
        private static List<object> m_CancelQueue = new List<object>(); //lock
        private static Stopwatch watch = new Stopwatch();
        // tasks
        static Executor m_Instance;
        public static void Initialize()
        {
            if (m_Instance == null)
            {
                var obj = new GameObject("Executor");
                m_Instance = obj.AddComponent<Executor>();
                DontDestroyOnLoad(obj);
            }
        }

        #region  mono
        void Awake()
        {
            m_Instance = this;
        }

        // void FixedUpdate () {
        void LateUpdate()
        {
            if (m_CancelQueue.Count > 0)
            {
                foreach (var act in m_CancelQueue)
                {
                    if (act is IEnumerator)
                    {
                        StopCoroutine((IEnumerator)act);
                    }
                    else if (act is Coroutine)
                        StopCoroutine((Coroutine)act);

                }
                m_CancelQueue.Clear();
            }

            watch.Restart();
            for (int i = 0; i < m_Tasks.Count; i++)
            {
                object act = m_Tasks[i];
                if (act is Action)
                {
#if HUGULA_PROFILER_DEBUG
                    UnityEngine.Profiling.Profiler.BeginSample("action" + act.GetHashCode());
                    ((Action)act)();
                    UnityEngine.Profiling.Profiler.EndSample();
#else
                    ((Action) act) ();

#endif
                    // Debug.LogFormat ("m_Task.action = {0} ", act);
                }
                else if (act is IEnumerator)
                {
                    StartCoroutine((IEnumerator)act);
                }
            }
            watch.Stop();
            long time = watch.ElapsedMilliseconds;
            if (time > 10)
                UnityEngine.Debug.LogWarningFormat("the executor's binding cost too long.  it  take {0} milliseconds. task count = {1}.", time, m_Tasks.Count);

            m_Tasks.Clear();
        }

        void OnApplicationQuit()
        {
            this.StopAllCoroutines();
            m_Tasks.Clear();
            m_CancelQueue.Clear();
            if (m_Instance != null)
            {
                Destroy(this.gameObject);
            }
            m_Instance = null;
        }
        #endregion

        /// <summary>
        /// Execute Action in main thread
        /// </summary>
        /// <param name="action"></param>
        public static void Execute(Action action)
        {
            if (action != null)
            {
                m_Tasks.Add(action);
                // Debug.LogFormat ("Execute.Add {0}", action);
            }
        }

        /// <summary>
        /// cancel Coroutine
        /// </summary>
        /// <param name="action"></param>
        public void Cancel(IEnumerator action)
        {
            if (action == null)
                return;

            if (m_Tasks.Contains(action))
            {
                m_Tasks.Remove(action);
                return;
            }

            lock (m_CancelQueue)
            {
                m_CancelQueue.Add(action);
            }
        }

        /// <summary>
        /// Cancel Coroutine
        /// </summary>
        /// <param name="action"></param>
        public void Cancel(Coroutine action)
        {
            if (action == null)
                return;

            m_CancelQueue.Add(action);
        }

        /// <summary>
        /// Execute IEnumerator in main thread
        /// </summary>
        /// <param name="action"></param>
        public static void Execute(IEnumerator action)
        {
            if (action != null)
            {
                m_Tasks.Add(action);
            }
        }

        public static Task<T> InvokeOnMainThreadAsync<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            Execute(() =>
            {
                try
                {
                    var result = func();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
            });
            return tcs.Task;
        }
    }

}