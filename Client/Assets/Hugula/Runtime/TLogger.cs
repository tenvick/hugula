using UnityEngine;
using System.Collections;
using System.IO;
using Hugula.Utils;
using System.Text;
using System;

///<summary>
///
/// 提供本地日志记录
/// 当HUGULA_NO_LOG和HUGULA_RELEASE同时开启的时候，只会写入错误日志和LogSys日志到本地文件，不会输出日志到控制台
///</summary>
[XLua.LuaCallCSharp]
public class TLogger : ILogHandler
{

    #region  ILogHander
    public void LogException(Exception exception, UnityEngine.Object context)
    {
        try
        {
            LogCallback(exception.Message, exception.StackTrace, LogType.Exception);
        }
        catch (Exception e)
        {
            LogCallback(e.InnerException.ToString(), exception.StackTrace, LogType.Exception);
        }
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        // #if !HUGULA_RELEASE 
        //         var content = string.Format(format, args);
        //         LogCallback(content, context == null ? "" : content.ToString(), logType);
        // #endif
    }


    #endregion

    static string preLogPath = "";
    static string logPath = "";
    static int line = 0;

    static bool isInit = false;
    private static string logName = "tlog.txt";
    private static string preLogName = "pre_tlog.txt";

    private static ILogger unityLogger = Debug.unityLogger;
    private static TLogger myLogger;
    static int mainThreadId = 0;
    private static object locked = new object();
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {

        if (!isInit && Application.isPlaying)
        {

            if (!Directory.Exists(CUtils.realPersistentDataPath))
                Directory.CreateDirectory(CUtils.realPersistentDataPath);

#if UNITY_EDITOR
            logPath = $"Assets/Tmp/{logName}";
            preLogPath = $"Assets/Tmp/{preLogName}";
#else
            logPath = Path.Combine(CUtils.realPersistentDataPath,logName);
            preLogPath = Path.Combine(CUtils.realPersistentDataPath,preLogName);
#endif

#if HUGULA_NO_LOG && HUGULA_RELEASE
            //不输出日志直接记录文件
            myLogger = new TLogger();
            Debug.unityLogger.logHandler = myLogger;
#else
            Application.logMessageReceivedThreaded -= LogCallback;
            Application.logMessageReceivedThreaded += LogCallback;
#endif
            mainThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId;

            OverrideLog(logPath, preLogPath);
            isInit = true;
        }
    }

    //覆盖旧的log
    static void OverrideLog(string newPath, string oldPath)
    {
        var finfo = new FileInfo(newPath);
        if (finfo.Exists) //如果本次存在
        {
            if (File.Exists(oldPath)) File.Delete(oldPath);//删除上一个.
            finfo.MoveTo(oldPath); //覆盖旧的
        }
    }

    static StringBuilder sb = new StringBuilder();
    static string[] LogTypes = { "Error", "Assert", "Warning", "Log", "Exception" };
    static void LogCallback(string msg, string stackTrace, LogType type)
    {
        if (msg == null) return;
        lock (locked)
        {
            // #if HUGULA_RELEASE
            //             if (type == LogType.Error || type == LogType.Exception)
            // #endif
            {
                sb.Clear();
                sb.Append("\r\n\r\n");
                sb.AppendLine(LogTypes[(int)type]);
                // sb.AppendLine(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
                sb.AppendLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sb.AppendLine(msg);
                sb.Append(stackTrace);
                WriteToLogFile(sb.ToString());
            }
        }
    }

    public static void LogError(string msg)
    {
        LogCallback(msg, "TLogger.LogError", LogType.Log);
#if HUGULA_NO_LOG
        Debug.LogError(BindColor("#ffff00", "err", msg));
#endif
    }

    public static void LogSys(string msg)
    {
        if (!string.IsNullOrEmpty(msg))
        {
            LogCallback(msg, "TLogger.LogSys", LogType.Log);
#if !HUGULA_NO_LOG
        Debug.Log(BindColor("#00e75c", "sys", msg));
#endif
            // BuglyAgent.PrintLog(LogSeverity.LogInfo,"sys_cs:starup:{1},frame:{2} \r\n msg:{0}",msg,Time.realtimeSinceStartup, Time.frameCount);
        }
    }

    public static void Log(string msg)
    {
#if !HUGULA_NO_LOG
        Debug.Log(BindColor("#00e75c", "Log", msg));
#endif
    }


    static void WriteToLogFile(string content)
    {
        using (var sw = new StreamWriter(logPath, true))
        {
            sw.Write(content);
        }
    }

    static string BindColor(string color1, string type, string msg)
    {

#if UNITY_EDITOR
        return string.Format("<b><color={0}>[{1}]</color></b> {2}", color1, type, msg);
#else
        return string.Format("[{0}] {1}", type, msg);
#endif
    }
}
