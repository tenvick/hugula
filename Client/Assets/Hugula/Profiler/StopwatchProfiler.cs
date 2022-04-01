using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Hugula.Profiler
{
    [XLua.LuaCallCSharp]
    public class StopwatchProfiler : System.IDisposable
    {
        #region Static
        private List<StopwatchProfiler> m_ChildrenList;
        public List<StopwatchProfiler> childrenList
        {
            get
            {
                if (m_ChildrenList == null) m_ChildrenList = new List<StopwatchProfiler>();
                return m_ChildrenList;
            }
        }

        /// <summary>
        /// Initializes static profiler stack with a sentry null, allows for peeking when stack is empty.
        /// </summary>
        static StopwatchProfiler()
        {
            // ProfilerStack.Push(null);
        }

        #endregion Static

        #region Fields
        public string stopName = "";
        private Stopwatch stopWatch = new Stopwatch();

        private int nestingLevel = 0;

        private double maxSingleElapsedTime = 0f;
        private double maxSingleElapsedTimeSelf = 0f;

        private double childrenElapsedMilliseconds = 0f;

        private double lastElapsedMilliseconds = 0f;

        public StopwatchProfiler()
        {
            NumberOfCalls = 0;
        }

        #endregion Fields

        #region Properties
        //上一帧整体耗时
        public double lastFrameTime = 0;
        public double MaxSingleFrameTimeInMsSelf
        {
            get { return maxSingleElapsedTimeSelf; }
        }

        public double MaxSingleFrameTimeInMs
        {
            get { return maxSingleElapsedTime; }
        }

        double m_ElapsedMilliseconds = 0;
        public double ElapsedMilliseconds
        {
            get { return stopWatch.Elapsed.TotalMilliseconds; }
        }

        public double ElapsedMillisecondsSelf
        {
            get { return ElapsedMilliseconds - childrenElapsedMilliseconds; }
        }

        public int NumberOfCalls { get; private set; }

        public int NumberOfCallsGreaterThan3ms { get; private set; }

        #endregion Properties

        public void AddParent(StopwatchProfiler parent)
        {
            if (parent != null)
            {
#if UNITY_EDITOR
                // UnityEngine.Debug.Log($"StopwatchProfiler:({parent.stopName}) addChild({this.stopName})");
#endif
                parent.childrenList.Add(this);
            }
        }

        public void Start()
        {
#if UWATEST || UWA_SDK_ENABLE
            UWAEngine.PushSample (stopName);
#endif
            nestingLevel++;
            NumberOfCalls++;

            if (nestingLevel == 1)
            {
#if UNITY_EDITOR
                // UnityEngine.Debug.Log($"StopwatchProfiler:({this.stopName}) Start({nestingLevel},{NumberOfCalls})");
#endif
                stopWatch.Start();
            }
        }

        public void Stop()
        {


#if UWATEST || UWA_SDK_ENABLE
            UWAEngine.PopSample ();
#endif
            if (nestingLevel == 1)
            {
#if UNITY_EDITOR
                // UnityEngine.Debug.Log($"StopwatchProfiler:({this.stopName}) Stop({nestingLevel},{NumberOfCalls})");
#endif
                stopWatch.Stop();

                childrenElapsedMilliseconds = 0; //所有
                double childrenFrameElapsedMilliseconds = 0;
                foreach (var watch in childrenList)
                {
                    childrenElapsedMilliseconds += watch.ElapsedMilliseconds;
                    childrenFrameElapsedMilliseconds += watch.lastFrameTime;
                }

                lastFrameTime = ElapsedMilliseconds - lastElapsedMilliseconds; //当前帧总体消耗
                lastElapsedMilliseconds = ElapsedMilliseconds;

                double lastFrameTimeSelf = lastFrameTime - childrenFrameElapsedMilliseconds; //当前帧自身消耗

                if (lastFrameTimeSelf > maxSingleElapsedTimeSelf)
                    maxSingleElapsedTimeSelf = lastFrameTimeSelf;

                if (lastFrameTime > 3.0f)
                {
                    NumberOfCallsGreaterThan3ms++;
                }
                if (lastFrameTime > maxSingleElapsedTime)
                    maxSingleElapsedTime = lastFrameTime;

                childrenList.Clear();
            }
            nestingLevel--;
        }

        public void ForceStop()
        {
            stopWatch.Stop();
            nestingLevel = 0;
        }

        public void Reset()
        {
            stopWatch.Reset();
            nestingLevel = 0;
            NumberOfCalls = 0;
            NumberOfCallsGreaterThan3ms = 0;
            maxSingleElapsedTime = 0f;
            childrenElapsedMilliseconds = 0f;
            lastElapsedMilliseconds = 0f;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}