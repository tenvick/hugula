// #define DEBUG_TIMER
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Utils;
using System.Linq;

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
                    return cycle<= 0|| currCycle >= cycle;
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
#if DEBUG_TIMER
        static Dictionary<int,string> m_TimerTraceDic = new Dictionary<int, string>();
#endif
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
            m_Timers.Add(timerInfo);

#if DEBUG_TIMER
            var str = EnterLua.LuaTraceback();
            m_TimerTraceDic.Add(timerInfo.id,str);
            Debug.Log($"add DelayFrame(id={timerInfo.id},begin={timerInfo.begin},delay:{frame},frameCount:{Time.frameCount},action:{action.GetHashCode()}) arg:{arg}\r\n:{str}");
#endif
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
            m_Timers.Add(timerInfo);
#if DEBUG_TIMER
            var str = EnterLua.LuaTraceback();
            m_TimerTraceDic.Add(timerInfo.id,str);
            Debug.Log($"add Delay(id={timerInfo.id},begin={timerInfo.begin},delay:{delay},frameCount:{Time.frameCount},action:{action.GetHashCode()}) arg:{arg}\r\n:{str}");
#endif
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
#if DEBUG_TIMER
            var str = EnterLua.LuaTraceback();
            m_TimerTraceDic.Add(timerInfo.id,str);
            Debug.Log($"add Delay(id={timerInfo.id},begin={timerInfo.begin},delay:{delay},frameCount:{Time.frameCount},action:{action.GetHashCode()}) arg:{arg}\r\n:{str}");
#endif
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
            timerInfo.isFrame = false;
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
            var frame = Time.frameCount; //当前frame
            TimerInfo timerInfo = null;
            System.Action<object> action = null;
            object arg = null;
            for (int i = 0; i < m_Timers.Count;)
            {
                timerInfo = m_Timers[i];
                if((!timerInfo.isFrame && timerInfo.begin + timerInfo.delay <= time  )
                    || (timerInfo.isFrame &&  (int)timerInfo.begin + (int)timerInfo.delay <= frame )
                 )
                {

                    action = timerInfo.action;
                    arg = timerInfo.arg;

#if DEBUG_TIMER 
                    if(timerInfo.isFrame)
                        Debug.Log($"do DelayFrame(id={timerInfo.id},begin={(int)timerInfo.begin}+delay={(int)timerInfo.delay}<=frame={frame} is {(int)timerInfo.begin + (int)timerInfo.delay >= frame},currCyc={timerInfo.currCycle}>cycle={timerInfo.cycle};action={action.GetHashCode()}\r\n {m_TimerTraceDic[timerInfo.id]}");
                    else
                        Debug.Log($"do Delay(id={timerInfo.id},begin={timerInfo.begin}+delay={timerInfo.delay}<=time={time} is {timerInfo.begin + timerInfo.delay <= time },frame={frame},currCyc={timerInfo.currCycle}>cycle={timerInfo.cycle};action={action.GetHashCode()}\r\n {m_TimerTraceDic[timerInfo.id]}");
#endif

                    if (timerInfo.isDone)
                    {
                        m_Timers.RemoveAt(i);
#if DEBUG_TIMER 
                            if(timerInfo.isFrame)
                                Debug.Log($"remove DelayFrame(id={timerInfo.id},begin={timerInfo.begin},delay={timerInfo.delay},currCyc={timerInfo.currCycle}>cycle={timerInfo.cycle}; action={action.GetHashCode()}\r\n {m_TimerTraceDic[timerInfo.id]}");
                            else
                                Debug.Log($"remove Delay(id={timerInfo.id},begin={timerInfo.begin},delay={timerInfo.delay},currCyc={timerInfo.currCycle}>cycle={timerInfo.cycle}; action={action.GetHashCode()}\r\n {m_TimerTraceDic[timerInfo.id]}");

                            m_TimerTraceDic.Remove(timerInfo.id);
#endif
                        m_pool.Release(timerInfo);
                    }
                    else
                    {
                        timerInfo.currCycle++;
                    }

                    //刷新开始时间
                    if(timerInfo.isFrame)
                        timerInfo.begin = frame;
                    else
                        timerInfo.begin = time;

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
