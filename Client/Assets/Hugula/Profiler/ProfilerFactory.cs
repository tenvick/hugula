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

        private const string LogFormat = "Profiler {0} - Total: {1}ms | Self - {2}ms | Number of calls - {3} | Max Single Call - {4}ms | Number of SingleCallTime>3ms - {5}\n";

        #endregion

        #region Static

        private static Dictionary<string, StopwatchProfiler> profilers = new Dictionary<string, StopwatchProfiler>();

#if HUGULA_RELEASE
		public static readonly bool DoNotProfile = true;
#elif PROFILER_NO_DUMP
        public static readonly bool DoNotProfile = true;
#else
        public static readonly bool DoNotProfile = false;
#endif

        #endregion Static

        #region Methods

        public static StopwatchProfiler GetProfiler(string name)
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

            return profiler;
        }

        public static StopwatchProfiler GetAndStartProfiler(string name, string args = "")
        {
            if (DoNotProfile) return null;
            StopwatchProfiler profiler = GetProfiler(string.IsNullOrEmpty(args) ? name : name + args);
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
                WriteToStream(kFile, string.Format("\n------------------------------------------  {0}    ------------------------------------------------------------\n", DateTime.Now.ToString()));
                Debug.LogFormat("DumpProfilerInfo in path = {0} ", kLogFileName);
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
                    UnityEngine.Debug.LogWarning("found negative value, check profiler code " + pair.Key);
                }
                fullTime += time;
                if (pair.Value.NumberOfCalls > 0 && time > timeThresholdToLog)
                {
                    string kLine = string.Format(LogFormat, pair.Key, pair.Value.ElapsedMilliseconds, pair.Value.ElapsedMillisecondsSelf.ToString("0.00"), pair.Value.NumberOfCalls, pair.Value.MaxSingleFrameTimeInMs.ToString("0.00"), pair.Value.NumberOfCallsGreaterThan3ms);
                    if (bWriteToFile)
                    {
                        WriteToStream(kFile, kLine);
                    }
                    UnityEngine.Debug.Log(kLine);
                }
                if (resetProfilers) pair.Value.Reset();
            }

            if (fullTime > timeThresholdToLog)
            {
                string kLine = "Profiler Full stopwatch measured(ms): " + fullTime;
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