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

        private const string TitleFormat = "\nProfiler|Total|Self|Calls|Max Single Call|Max Self Single Call|Avg Except Max Call|Number of SingleCallTime>3ms";
        private const string LogFormat = "\n{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}";

        #endregion

        #region Static

        private static Dictionary<string, StopwatchProfiler> profilers = new Dictionary<string, StopwatchProfiler>();
#if PROFILER_DUMP
        public static readonly bool DoNotProfile = false;
#elif HUGULA_RELEASE
        public static readonly bool DoNotProfile = true;
#else
        public static readonly bool DoNotProfile = false;
#endif

        #endregion Static

        #region Methods

        public static StopwatchProfiler GetProfiler(string name, string parentName=null)
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

        public static StopwatchProfiler GetAndStartComponentProfiler(Component target, string args = "")
        {
            if (DoNotProfile) return null;
            string fullPath = " :" + args ?? "";
            if (target != null)
            {
                fullPath = Hugula.Utils.CUtils.GetGameObjectFullPath(((UnityEngine.Component)target).gameObject) + fullPath;
            }
            StopwatchProfiler profiler = GetProfiler(fullPath);
            profiler.Start();
            return profiler;
        }

        public static StopwatchProfiler GetAndStartProfiler(string name, string args = "", string parentName = null)
        {
            if (DoNotProfile) return null;
            StopwatchProfiler profiler = GetProfiler(string.IsNullOrEmpty(args) ? name : name + args, parentName);
            profiler.Start();
            return profiler;
        }
        private static System.Text.UnicodeEncoding uniEncoding = new System.Text.UnicodeEncoding();

        private static void WriteToStream(Stream stream, string msg)
        {
            if (stream != null)
            {
                stream.Write(uniEncoding.GetBytes(msg),
                    0, uniEncoding.GetByteCount(msg));
            }
        }
        public static void DumpProfilerInfo(float timeThresholdToLog = 0f, bool resetProfilers = true, bool bWriteToFile = false)
        {
            FileStream kFile = null;
            if (bWriteToFile)
            {
                string kDirName = Application.persistentDataPath + "/Log/";
                if (File.Exists(kDirName) == false)
                {
                    Directory.CreateDirectory(kDirName);
                }
                string kLogFileName = kDirName + "ProfilerInfoLog.txt";

                if (File.Exists(kLogFileName))
                {
                    kFile = File.Open(kLogFileName, FileMode.Append);
                }
                else
                {
                    kFile = File.Create(kLogFileName);
                }
                WriteToStream(kFile, string.Format("\r\n\r\n\r\n\r\n------------------------------------------  {0}[{1};{2}] - {3}  ms   ------------------------------------------------------------\r\n", UnityEngine.SystemInfo.deviceModel, UnityEngine.SystemInfo.deviceType, SystemInfo.deviceUniqueIdentifier, DateTime.Now.ToString()));
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

            list.Sort(delegate (KeyValuePair<string, StopwatchProfiler> a, KeyValuePair<string, StopwatchProfiler> b) { return a.Value.ElapsedMilliseconds.CompareTo(b.Value.ElapsedMilliseconds); });

            double fullTime = 0;
            foreach (var pair in list)
            {
                double time = pair.Value.ElapsedMillisecondsSelf;
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
                    string kLine = string.Format(LogFormat, pair.Key, itemValue.ElapsedMilliseconds.ToString("0.0000"), itemValue.ElapsedMillisecondsSelf.ToString("0.0000"), itemValue.NumberOfCalls, itemValue.MaxSingleFrameTimeInMs.ToString("0.0000"), itemValue.MaxSingleFrameTimeInMsSelf.ToString("0.0000"), avgExceptMaxMilliseconds.ToString("0.0000"), pair.Value.NumberOfCallsGreaterThan3ms);
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

        // public static void BeginSample(string name)
        // {
        //     UnityEngine.Profiling.Profiler.BeginSample(string.Format("{0} -- {1}", name, Time.frameCount));
        // }

        // public static void EndSample()
        // {
        //     UnityEngine.Profiling.Profiler.EndSample();
        // }

        #endregion Methods
    }
}