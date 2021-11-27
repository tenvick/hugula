using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Utils;
using System.Linq;

namespace Hugula.Framework
{

    public class Timer : BehaviourSingleton<Timer>
    {
        class TimerInfo
        {
            internal float begin;
            internal float delay;
            public int id;
            public object arg;
            public System.Action<object> action;
            internal int cycle = 1;

            internal int currCycle = 0;

            internal float time
            {
                get
                {
                    return begin + currCycle * delay;
                }
            }

            internal bool isDone
            {
                get
                {
                    return (cycle != -1) && (currCycle >= cycle);
                }
            }

            public void OnDestroy()
            {
                action = null;
                arg = null;

            }
        }
        private static ObjectPool<TimerInfo> m_pool = new ObjectPool<TimerInfo>(null, ActionOnRelease);
        private static List<TimerInfo> m_Timers = new List<TimerInfo>(16);
        private static int m_ActID = 1;

        public static int Add(float delay, System.Action<object> action, object arg)
        {
            if (action == null)
                throw new System.ArgumentNullException("action");
            var timerInfo = m_pool.Get();
            timerInfo.id = m_ActID++;
            timerInfo.action = action;
            timerInfo.arg = arg;
            timerInfo.delay = delay;
            timerInfo.begin = Time.unscaledTime;
            timerInfo.cycle = 1;
            timerInfo.currCycle++;
            m_Timers.Add(timerInfo);
            return timerInfo.id;
        }

        public static int Add(float delay, int cycle, System.Action<object> action, object arg)
        {
            if (action == null)
                throw new System.ArgumentNullException("action");
            var timerInfo = m_pool.Get();
            timerInfo.id = m_ActID++;
            timerInfo.action = action;
            timerInfo.arg = arg;
            timerInfo.delay = delay;
            timerInfo.begin = Time.unscaledTime;
            timerInfo.cycle = cycle;
            timerInfo.currCycle++;
            m_Timers.Add(timerInfo);
            return timerInfo.id;
        }
        public static void Remove(int id)
        {
            TimerInfo timerInfo = m_Timers.Find((t) =>
            {
                return t.id == id;
            });

            if (timerInfo != null)
                m_Timers.Remove(timerInfo);

        }

        /// <summary>
        /// 清理所有的对象
        /// </summary>
        public static void Clear()
        {
            foreach (var t in m_Timers)
            {
                t.OnDestroy();
            }
            m_Timers.Clear();
        }

        static void ActionOnRelease(TimerInfo timerInfo)
        {
            timerInfo.id = 0;
            timerInfo.action = null;
            timerInfo.arg = null;
            timerInfo.currCycle = 0;
            timerInfo.cycle = 1;
        }

        // Update is called once per frame
        void Update()
        {
            var time = Time.unscaledTime;
            TimerInfo timerInfo = null;
            System.Action<object> action = null;
            object arg = null;
            for (int i = 0; i < m_Timers.Count;)
            {
                timerInfo = m_Timers[i];
                if (timerInfo.begin + timerInfo.delay <= time)
                {
                    timerInfo.begin = time;
                    action = timerInfo.action;
                    arg = timerInfo.arg;
                    if (timerInfo.isDone)
                    {
                        m_Timers.RemoveAt(i);
                        m_pool.Release(timerInfo);
                    }
                    else
                    {
                        timerInfo.currCycle++;
                    }
                    action(arg);
                }
                else
                {
                    i++;
                }
            }
        }

        protected override void OnDestroy()
        {
            foreach (var t in m_Timers)
                t.OnDestroy();

            m_Timers.Clear();

            base.OnDestroy();

        }
    }
}
