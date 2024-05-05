using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Hugula.Profiler
{
    [XLua.LuaCallCSharp]
    public class StopwatchProfiler : System.IDisposable
    {
        // #region Static
        // private List<StopwatchProfiler> m_ChildrenList;
        // public List<StopwatchProfiler> childrenList
        // {
        //     get
        //     {
        //         if (m_ChildrenList == null) m_ChildrenList = new List<StopwatchProfiler>();
        //         return m_ChildrenList;
        //     }
        // }

        /// <summary>
        /// Initializes static profiler stack with a sentry null, allows for peeking when stack is empty.
        /// </summary>
        static StopwatchProfiler()
        {
            // ProfilerStack.Push(null);
        }

        // #endregion Static

        #region Fields
        public string stopName = "";
        private Stopwatch stopWatch = new Stopwatch();

        private int nestingLevel = 0;

        internal int getNestingLevel()
        {
            return nestingLevel;
        }

        private double maxSingleElapsedTime = 0f;
        private double maxSingleElapsedTimeSelf = 0f; 

        public StopwatchProfiler()
        {
            NumberOfCalls = 0;
        }

        private bool m_NeedUnityProfiler = false;
        #endregion Fields

        #region Properties

        int startFrameTime = 0;
        internal int firstFrameCount = 0;
        internal int firstEndFrameCount = 0;
        /// <summary>
        /// 最大耗时开始帧
        /// </summary>
        internal int maxFrameCount = 0;
        /// <summary>
        ///  最大耗时结束帧
        /// </summary>
        internal int maxEndFrameCount = 0;

        internal int currentLoadingAssetCount =0;

        internal string tips = string.Empty;
    
        /// <summary>
        /// 总共单帧最大耗时
        /// </summary>
        internal double MaxSingleFrameTimeInMs
        {
            get { return maxSingleElapsedTime; }
        }

        double m_ElapsedMilliseconds = 0;
        /// <summary>
        /// 总共耗时
        /// </summary>
        internal double ElapsedMilliseconds
        {
            get { return m_ElapsedMilliseconds; }
        }

        internal int NumberOfCalls { get; private set; }

        internal int NumberOfCallsGreaterThan3ms { get; private set; }

        #endregion Properties

        public void AddParent(StopwatchProfiler parent)
        {
            //             if (parent != null)
            //             {
            // #if UNITY_EDITOR
            //                 // UnityEngine.Debug.Log($"StopwatchProfiler:({parent.stopName}) addChild({this.stopName})");
            // #endif
            //                 parent.childrenList.Add(this);
            //             }
        }

        public void Start(bool needUnityProfiler = false)
        {
            startFrameTime = Time.frameCount;

            if (firstFrameCount == 0)
            {
                firstFrameCount = startFrameTime;
                firstEndFrameCount = startFrameTime;
            }
            nestingLevel++;
            NumberOfCalls++;
            
            currentLoadingAssetCount = ResLoader.CurrentLoadingAssetCount;

            if (nestingLevel == 1)
            {
#if UNITY_EDITOR
                // UnityEngine.Debug.Log($"StopwatchProfiler:({this.stopName}) Start({nestingLevel},{NumberOfCalls})");
#endif
                stopWatch.Restart();
            }

            if (needUnityProfiler)
            {
                m_NeedUnityProfiler = needUnityProfiler;
#if UWATEST || UWA_SDK_ENABLE
            UWAEngine.PushSample (stopName);
#endif
                UnityEngine.Profiling.Profiler.BeginSample(stopName);
            }
        }

        public void Stop()
        {
            if (m_NeedUnityProfiler)
            {
                m_NeedUnityProfiler = false;
                UnityEngine.Profiling.Profiler.EndSample();
#if UWATEST || UWA_SDK_ENABLE
            UWAEngine.PopSample();
#endif
            }
            if (nestingLevel == 1)
            {
                stopWatch.Stop();
                var frameCount = Time.frameCount;

                if (firstEndFrameCount == firstFrameCount) //记录第一次耗时end frameCount
                {
                    firstEndFrameCount = frameCount;
                }

                var currWatchTime = stopWatch.Elapsed.TotalMilliseconds;

                m_ElapsedMilliseconds += currWatchTime; //统计总体耗时

                if (currWatchTime > maxSingleElapsedTimeSelf)
                {
                    maxSingleElapsedTimeSelf = currWatchTime;
                    maxFrameCount = startFrameTime;
                    maxEndFrameCount = frameCount;
                    tips =$"{currentLoadingAssetCount} , {ResLoader.CurrentLoadingAssetCount}";
                }

                if (currWatchTime > 3.0f)
                {
                    NumberOfCallsGreaterThan3ms++;
                }
                if (currWatchTime > maxSingleElapsedTime)
                    maxSingleElapsedTime = currWatchTime;

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
            maxSingleElapsedTimeSelf = 0f;
            m_ElapsedMilliseconds = 0;
            firstFrameCount = 0;
            firstEndFrameCount = 0;
            maxFrameCount = 0;
            maxEndFrameCount = 0;
        }

        public void Dispose()
        {
            Stop();
        }
    }
}