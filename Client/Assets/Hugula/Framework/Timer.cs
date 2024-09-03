// #define DEBUG_TIMER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Utils;
using System.IO;
using System.Text;

namespace Hugula.Framework
{
    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]

    public class Timer : BehaviourSingleton<Timer>
    {
        class TimerInfo
        {
            internal float begin;
            internal float delay;
            internal bool isFrame = false;
            public int id;
            public string name = string.Empty;
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
                    return cycle <= 0 || currCycle >= cycle;
                }
            }

            public void OnDestroy()
            {
                action = null;
                arg = null;

            }
        }
        private static ObjectPool<TimerInfo> m_pool = new ObjectPool<TimerInfo>(null, ActionOnRelease);
        private static LinkedList<TimerInfo> m_Timers = new LinkedList<TimerInfo>();

        // #if PROFILER_DUMP
        private static Dictionary<int, TimerInfo> m_TimersDic = new Dictionary<int, TimerInfo>(32);
        // #endif

        private static int m_ActID = 1;

        public static int Delay(System.Action<object> action, float time, object arg)
        {
            return Add(time, action, arg);
        }

        public static int DelayFrame(System.Action<object> action, int frame, object arg)
        {
            if (action == null)
                throw new System.ArgumentNullException("action");

            var timerInfo = m_pool.Get();
            timerInfo.id = m_ActID++;
            timerInfo.action = action;
            timerInfo.arg = arg;
            timerInfo.delay = frame;
            timerInfo.isFrame = true;
            timerInfo.begin = Time.frameCount;
            timerInfo.cycle = 1;
            timerInfo.currCycle++;
            // m_Timers.Add(timerInfo);
            m_Timers.AddLast(timerInfo);
            m_TimersDic[timerInfo.id] = timerInfo;
            return timerInfo.id;
        }

        public static void StopDelay(int id)
        {
            Remove(id);
        }


        public static int Add(float delay, System.Action<object> action, object arg)
        {
            if (action == null)
                throw new System.ArgumentNullException("action");

            var timerInfo = m_pool.Get();
            timerInfo.id = m_ActID++;
            timerInfo.action = action;
            timerInfo.arg = arg;
            timerInfo.isFrame = false;
            timerInfo.delay = delay;
            timerInfo.begin = Time.unscaledTime;
            timerInfo.cycle = 1;
            timerInfo.currCycle++;
            m_Timers.AddLast(timerInfo);
            m_TimersDic[timerInfo.id] = timerInfo;
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
            if (cycle < 0) cycle = int.MaxValue;
            timerInfo.cycle = cycle;
            timerInfo.currCycle++;
            m_Timers.AddLast(timerInfo);
            m_TimersDic[timerInfo.id] = timerInfo;
            return timerInfo.id;
        }

        public static void Remove(int id)
        {
            if (m_TimersDic.TryGetValue(id, out var timerInfo))
            {
                m_Timers.Remove(timerInfo);
                m_pool.Release(timerInfo);
                m_TimersDic.Remove(id);
            }
        }

        public static bool SetTimerName(int id, string name)
        {
            if (m_TimersDic.TryGetValue(id, out var timerInfo))
            {
                timerInfo.name = name;
                return true;
            }
            return false;
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
            m_TimersDic.Clear();
        }

        static void ActionOnRelease(TimerInfo timerInfo)
        {
            timerInfo.isFrame = false;
            timerInfo.id = 0;
            timerInfo.action = null;
            timerInfo.name = string.Empty;
            timerInfo.arg = null;
            timerInfo.currCycle = 0;
            timerInfo.cycle = 1;
        }

        // Update is called once per frame
        void Update()
        {
            FrameWatcher.BeginWatch();

            var time = Time.unscaledTime;
            var frame = Time.frameCount; //当前frame
            TimerInfo timerInfo = null;
            System.Action<object> action = null;
            object arg = null;
            string name = "";
            int id = 0;

            for (var node = m_Timers.First; node != null;)
            {
                timerInfo = node.Value;
                if ((!timerInfo.isFrame && timerInfo.begin + timerInfo.delay <= time)
                    || (timerInfo.isFrame && (int)timerInfo.begin + (int)timerInfo.delay <= frame)
                 )
                {

                    action = timerInfo.action;
                    arg = timerInfo.arg;
                    name = timerInfo.name;
                    id = timerInfo.id;

                    if (timerInfo.isDone)
                    {
                        var nextNode = node.Next;
                        m_Timers.Remove(node);
                        m_TimersDic.Remove(id);
                        m_pool.Release(timerInfo);
                        node = nextNode;
                    }
                    else
                    {
                        timerInfo.currCycle++;

                        //刷新开始时间
                        if (timerInfo.isFrame)
                            timerInfo.begin = frame;
                        else
                            timerInfo.begin = time;
                    }
                    try
                    {

#if !HUGULA_RELEASE || PROFILER_DUMP
                        UnityEngine.Profiling.Profiler.BeginSample($"Timer.action:{id} {name}({arg})");
                        action(arg);
                        UnityEngine.Profiling.Profiler.EndSample();
#else
                    action(arg);
#endif
                    }
                    catch (System.Exception ex)
                    {
                        Debug.LogException(ex);
                    }
                }
                else
                {
                    node = node.Next;
                }

                if (FrameWatcher.IsTimeOver())
                {
                    break;
                }
            }
        }

        protected override void OnDestroy()
        {
            foreach (var t in m_Timers)
                t.OnDestroy();

            m_Timers.Clear();
            m_TimersDic.Clear();

            base.OnDestroy();

        }
    }
}
