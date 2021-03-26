using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Hugula.Framework;
using UnityEngine;

namespace Hugula
{
    public class Executor : BehaviourSingleton<Executor>
    {

        private static List<object> m_Tasks = new List<object>(); //lock
        private static List<object> m_CancelQueue = new List<object>(); //lock
#if UNITY_EDITOR && !HUGULA_PROFILER_DEBUG
        private static Stopwatch watch = new Stopwatch();
#endif

        #region  mono
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
#if UNITY_EDITOR && !HUGULA_PROFILER_DEBUG
            watch.Restart();
#endif
            for (int i = 0; i < m_Tasks.Count; i++)
            {
                object act = m_Tasks[i];
                if (act is Action)
                {
#if HUGULA_PROFILER_DEBUG 
                    var profiler = Hugula.Profiler.ProfilerFactory.GetAndStartProfiler("Executor.action {0}", act.GetHashCode().ToString());
                    ((Action)act)();
                    if (profiler != null) profiler.Stop();
#else
                    ((Action)act)();

#endif
                }
                else if (act is IEnumerator)
                {
                    StartCoroutine((IEnumerator)act);
                }
            }

#if UNITY_EDITOR && !HUGULA_PROFILER_DEBUG
            watch.Stop();
            long time = watch.ElapsedMilliseconds;
            if (time > 10)
                UnityEngine.Debug.LogWarningFormat("the executor's binding cost too long.  it  take {0} milliseconds. task count = {1}.", time, m_Tasks.Count);
#endif
            m_Tasks.Clear();
        }

        void OnApplicationQuit()
        {
            this.StopAllCoroutines();
            m_Tasks.Clear();
            m_CancelQueue.Clear();
            // Destroy(this.gameObject);
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