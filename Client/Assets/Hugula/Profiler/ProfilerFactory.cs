using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Hugula.Profiler
{

    /// <summary>
    /// Usage:
    /// using(ProfilerFactory.GetProfiler("profilerName"))
    /// {
    ///		ExpensiveMethod();
    ///		// more code that you want to profile quickly
    /// }
    /// 
    /// 
    /// Add a call to ProfilerFactory.DumpProfilerInfo(); to log all measured data.
    /// </summary>
    [XLua.LuaCallCSharp]
    public static class ProfilerFactory
    {
        #region Constants

        private const string TitleFormat = "\nProfiler|Total|Calls|Max Single Call|Avg Except Max Call|Number of SingleCallTime>3ms|first frame|first end frame|max frame|max end frame|asset count|nestingLevel";
        private const string LogFormat = "\n{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}";

        #endregion

        #region Static

        private static Dictionary<string, StopwatchProfiler> profilers = new Dictionary<string, StopwatchProfiler>();
#if PROFILER_NO_DUMP
        public static readonly bool DoNotProfile = true;
#elif PROFILER_DUMP
        public static readonly bool DoNotProfile = false;
#else
        public static readonly bool DoNotProfile = true;
#endif

        #endregion Static

        #region Methods

        public static StopwatchProfiler GetProfiler(string name, string parentName = null)
        {
            if (DoNotProfile) return null;

            StopwatchProfiler profiler;
            profilers.TryGetValue(name, out profiler);

            if (profiler == null)
            {
                profiler = new StopwatchProfiler();
                profiler.stopName = name;
                profilers.Add(name, profiler);
            }

            if (!string.IsNullOrEmpty(parentName) && profilers.TryGetValue(parentName, out var parent))
            {
                profiler.AddParent(parent);
            };

            return profiler;
        }

        public static StopwatchProfiler GetAndStartComponentProfiler(Component target, string args = "", bool needUnityProfiler = false)
        {
            if (DoNotProfile) return null;
            string fullPath = " :" + args ?? "";
            if (target != null)
            {
                // fullPath = Hugula.Utils.CUtils.GetGameObjectFullPath(((UnityEngine.Component)target).gameObject) + fullPath;
                fullPath = target.name + fullPath;
            }
            StopwatchProfiler profiler = GetProfiler(fullPath);
            profiler.Start(needUnityProfiler);
            return profiler;
        }

        public static StopwatchProfiler GetAndStartProfiler(string name, string args = "", string parentName = null, bool needUnityProfiler = false)
        {
            if (DoNotProfile) return null;
            StopwatchProfiler profiler = GetProfiler(string.IsNullOrEmpty(args) ? name : name + args, parentName);
            profiler.Start(needUnityProfiler);
            return profiler;
        }
        private static System.Text.UTF8Encoding uniEncoding = new System.Text.UTF8Encoding();

        private static void WriteToStream(Stream stream, string msg)
        {
            if (stream != null)
            {
                stream.Write(uniEncoding.GetBytes(msg),
                    0, uniEncoding.GetByteCount(msg));
            }
        }

        /// <summary>
        /// Dumps all profiler data to a log file.
        /// </summary>
        static string LogDirName
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE
                var kDirName = Path.Combine(Application.dataPath, "../Logs/");
#else
                var kDirName = Hugula.Utils.CUtils.realPersistentDataPath;
#endif
                return kDirName;
            }
        }

        public static List<string> GetProfilerNames()
        {
            var dir = new DirectoryInfo(LogDirName);
            var files = dir.GetFiles("ProfilerInfoLog_*.txt");
            var list = new List<string>();
            foreach (var item in files)
            {
                list.Add(item.FullName);
            }
            return list;
        }


        static string m_LogFileName;
        static string LogFileFullName
        {
            get
            {
                if (string.IsNullOrEmpty(m_LogFileName))
                {
#if UNITY_EDITOR || UNITY_STANDALONE
                    var kDirName = Path.Combine(Application.dataPath, "../Logs/");
#else
                var kDirName = Hugula.Utils.CUtils.realPersistentDataPath;
#endif

                    if (File.Exists(kDirName) == false)
                    {
                        Directory.CreateDirectory(kDirName);
                    }

                    m_LogFileName = Path.Combine(kDirName, $"ProfilerInfoLog_{System.DateTime.Now.ToString("MM_dd HH_mm_ss ")}.txt");
                }

                return m_LogFileName;
            }
        }

        public static void DumpProfilerInfo(float timeThresholdToLog = 0f, bool resetProfilers = true, bool bWriteToFile = false)
        {
            FileStream kFile = null;
            if (bWriteToFile)
            {

                string kLogFileName = LogFileFullName;

                if (File.Exists(kLogFileName))
                {
                    kFile = File.Open(kLogFileName, FileMode.Append);
                }
                else
                {
                    kFile = File.Create(kLogFileName);
                }
                WriteToStream(kFile, string.Format("\r\n\r\n\r\n\r\n------------------------------------------  {0}[{1};{2}] - {3}  ms   ------------------------------------------------------------\r\n", UnityEngine.SystemInfo.deviceModel, UnityEngine.SystemInfo.deviceType, SystemInfo.deviceUniqueIdentifier, DateTime.Now.ToString()));
               
                using (var memoryInfoPlugin = new MemoryInfo.MemoryInfoPlugin())
                {
                    var memoryInfo = memoryInfoPlugin.GetMemoryInfo();
                    WriteToStream(kFile,$"------------------------------------------ memoryInfo:TotalSize={memoryInfo.TotalSize},UsedSize={memoryInfo.UsedSize},AvailMem={memoryInfo.AvailMem}------------------------------------------------------------\r\n");
                }

                WriteToStream(kFile, TitleFormat);
                Debug.LogFormat("\r\nDumpProfilerInfo in path = {0} ", kLogFileName);
            }

            foreach (var pair in profilers)
            {
                pair.Value.ForceStop();
            }

            var list = new List<KeyValuePair<string, StopwatchProfiler>>();

            foreach (var pair in profilers)
            {
                list.Add(pair);
            }

            list.Sort(delegate (KeyValuePair<string, StopwatchProfiler> a, KeyValuePair<string, StopwatchProfiler> b)
            {
                return
                    // a.Value.firstTime.CompareTo(b.Value.firstTime);
                    b.Value.firstFrameCount.CompareTo(a.Value.firstFrameCount);
                //a.Value.ElapsedMilliseconds.CompareTo(b.Value.ElapsedMilliseconds);

            });

            double fullTime = 0;
            foreach (var pair in list)
            {
                double time = pair.Value.ElapsedMilliseconds;
                if (time < 0)
                {
                    UnityEngine.Debug.LogWarning($"found negative value, check profiler code {pair.Key};ElapsedMillisecondsSelf={time}");
                    time = 1;
                }
                fullTime += time;
                if (pair.Value.NumberOfCalls > 0 && time > timeThresholdToLog)
                {
                    var itemValue = pair.Value;
                    var avgExceptMaxMilliseconds = 0f;
                    if (itemValue.NumberOfCalls > 1)
                    {
                        avgExceptMaxMilliseconds = (float)(itemValue.ElapsedMilliseconds - itemValue.MaxSingleFrameTimeInMs) / (float)(itemValue.NumberOfCalls - 1);
                    }

                    string kLine = string.Format(LogFormat, pair.Key, itemValue.ElapsedMilliseconds.ToString("0.0000"), itemValue.NumberOfCalls, itemValue.MaxSingleFrameTimeInMs.ToString("0.0000"), avgExceptMaxMilliseconds.ToString("0.0000"), pair.Value.NumberOfCallsGreaterThan3ms, itemValue.firstFrameCount,itemValue.firstEndFrameCount,itemValue.maxFrameCount ,itemValue.maxEndFrameCount,itemValue.tips,itemValue.getNestingLevel());
                    if (bWriteToFile)
                    {
                        WriteToStream(kFile, kLine);
                    }
                    // UnityEngine.Debug.Log(kLine);
                }
                if (resetProfilers) pair.Value.Reset();
            }

            if (fullTime > timeThresholdToLog)
            {
                string kLine = "\r\nProfiler Full stopwatch measured(ms): " + fullTime;
                UnityEngine.Debug.Log(kLine);
                if (bWriteToFile)
                {
                    WriteToStream(kFile, kLine);
                }
            }
            if (kFile != null)
            {
                kFile.Flush();
                kFile.Close();
            }

        }

        public static void BeginSample(string name, string arg = "")
        {
#if PROFILER_DUMP
            if (!string.IsNullOrEmpty(arg))
            {
                name = name + ":" + arg;
            }
#if UWATEST || UWA_SDK_ENABLE
            UWAEngine.PushSample (name);
#endif
            UnityEngine.Profiling.Profiler.BeginSample(name);
#endif
        }

        public static void EndSample()
        {
#if PROFILER_DUMP
            UnityEngine.Profiling.Profiler.EndSample();
#if UWATEST || UWA_SDK_ENABLE
            UWAEngine.PopSample ();
#endif
#endif
        }

        #endregion Methods
    }
}