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
        unityLogger?.LogException(exception,context);
        try
        {
            LogCallback(exception);
        }
        catch (Exception e)
        {
            LogCallback(e.InnerException.ToString(), exception.StackTrace, LogType.Exception);
        }
    }

    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {

#if HUGULA_NO_LOG
        if (!(logType == LogType.Log || logType == LogType.Warning)) //warning和log不上传
#endif
        {
            unityLogger?.LogFormat(logType,context,format,args);
            var content = string.Format(format, args);
            LogCallback(content, context == null ? "" : content.ToString(), logType);
        }
    }


    #endregion

    static string preLogPath = "";
    static string logPath = "";
    static int line = 0;

    static bool isInit = false;
    private static string logName = CUtils.platform + "_tlog.txt";
    private static string preLogName = CUtils.platform + "_pre_tlog.txt";

    private static ILogHandler unityLogger = Debug.unityLogger.logHandler;
    private static TLogger myLogger;
    private static object locked = new object();

    static StreamWriter logStreamWriter;
    [RuntimeInitializeOnLoadMethod]
    public static void Init()
    {

        if (!isInit && Application.isPlaying)
        {

            if (!Directory.Exists(CUtils.realPersistentDataPath))
                Directory.CreateDirectory(CUtils.realPersistentDataPath);


#if UNITY_EDITOR
            var path = Path.Combine(Application.dataPath, "../Logs");
            logPath = Path.Combine(path, logName);
            preLogPath = Path.Combine(path, preLogName);
             if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
#elif UNITY_STANDALONE
            var path = Path.Combine(Application.dataPath, "../Logs");
            logPath = Path.Combine(path, System.DateTime.Now.ToString("MM_dd HH_mm_ss ")+logName);
            preLogPath = Path.Combine(path, preLogName);
             if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
#else
            logPath = Path.Combine(CUtils.realPersistentDataPath,logName);
            preLogPath = Path.Combine(CUtils.realPersistentDataPath,preLogName);
#endif

            myLogger = new TLogger();
            Debug.unityLogger.logHandler = myLogger;

            OverrideLog(logPath, preLogPath);
            if (logStreamWriter != null) logStreamWriter.Close();
            logStreamWriter = new StreamWriter(logPath, true);
            isInit = true;
            LogSysInfo();
        }
    }

    public static void Close()
    {
        if (isInit)
        {
            lock (locked)
            {
                if (logStreamWriter != null) logStreamWriter.Close();
            }
        }

    }

    //覆盖旧的log
    static void OverrideLog(string newPath, string oldPath)
    {
        var finfo = new FileInfo(newPath);
        if (finfo.Exists) //如果本次存在
        {
            if (File.Exists(oldPath))
            {
                File.Delete(oldPath);//删除上一个.
            }
            finfo.MoveTo(oldPath); //覆盖旧的
        }
    }

    static StringBuilder sb = new StringBuilder();
    static string[] LogTypes = { "Error", "Assert", "Warning", "Log", "Exception" };

    //报错提示
    internal static void LogCallback(Exception exception)
    {
        lock (locked)
        {
            sb.Clear();
            sb.Append("\r\n\r\n");
            sb.Append(LogTypes[4]);
            // sb.AppendLine(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
            sb.Append(System.DateTime.Now.ToString(" yyyy-MM-dd HH:mm:ss | "));
            sb.Append(exception.Message);
            sb.Append("\r\n");
            sb.Append(exception.StackTrace);
            WriteToLogFile(sb.ToString());
#if HUGULA_RELEASE && (UNITY_ANDROID || UNTIY_IOS) && !UNITY_EDITOR
        Firebase.Crashlytics.Crashlytics.LogException(exception);
#endif
        }
    }

    static void LogCallback(string msg, string stackTrace, LogType logType)
    {
        if (msg == null) return;
        lock (locked)
        {
#if HUGULA_NO_LOG
            if (!(logType == LogType.Log || logType == LogType.Warning)) //warning和log不上传
#endif
            {

                sb.Clear();
                sb.Append("\r\n\r\n");
                sb.Append(LogTypes[(int)logType]);
                // sb.AppendLine(System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
                sb.Append(System.DateTime.Now.ToString(" yyyy-MM-dd HH:mm:ss | "));
                sb.Append(msg);
                sb.Append("\r\n");
                sb.Append(stackTrace);
                var txt = sb.ToString();
                WriteToLogFile(txt);
#if HUGULA_RELEASE && (UNITY_ANDROID || UNTIY_IOS) && !UNITY_EDITOR
                Firebase.Crashlytics.Crashlytics.Log(txt);
#endif
            }
        }
    }

    public static void LogError(string msg)
    {
#if !HUGULA_NO_LOG
        Debug.LogError(BindColor("#ffff00", "err", msg));
#else
        LogCallback(msg, "TLogger.LogError", LogType.Error);
#endif
    }

    public static void LogSys(string msg)
    {
        if (!string.IsNullOrEmpty(msg))
        {
#if !HUGULA_NO_LOG
            Debug.Log(BindColor("#00e75c", "sys", msg));
#else
            LogCallback(msg, "TLogger.LogSys", LogType.Assert);
#endif
        }
    }

    public static void Log(string msg)
    {
#if !HUGULA_NO_LOG
        Debug.Log(BindColor("#00e75c", "Log", msg));
#endif
    }

    internal static void LogSysInfo()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"platform:{Application.platform.ToString()}");
        sb.Append($"\r\n deviceUniqueIdentifier={SystemInfo.deviceUniqueIdentifier}");
        sb.Append($"\r\n identifier={Application.identifier}");
        sb.Append($"\r\n deviceName={SystemInfo.deviceName}");
        sb.Append($"\r\n date={System.DateTime.Now.ToString()}");
        sb.Append($"\r\n systemMemorySize={SystemInfo.systemMemorySize};");
        sb.Append($"\r\n processorFrequency= {SystemInfo.processorFrequency}");
        sb.Append($"\r\n processorCount= {SystemInfo.processorCount}");
        sb.Append($"\r\n graphicsMemorySize= {SystemInfo.graphicsMemorySize}");
        sb.Append($"\r\n internetReachability= {Application.internetReachability}");
        sb.Append($"\r\n deviceModel= {SystemInfo.deviceModel}");
        sb.Append($"\r\n version={Application.version};");
        sb.Append($"\r\n unityVersion={Application.unityVersion}");
        sb.Append($"\r\n systemLanguage={Application.systemLanguage.ToString()};");
#if HUGULA_RELEASE && (UNITY_ANDROID || UNTIY_IOS) && !UNITY_EDITOR
        Firebase.Crashlytics.Crashlytics.Log(sb.ToString());
#endif

#if !HUGULA_RELEASE
        // var objs = Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object));
        // sb.AppendLine("total objs length = " + objs.Length + "\n");
        // foreach (var obj in objs)
        // {
        //     System.Type type = obj.GetType();
        //     sb.AppendFormat("name = {0},type = {1} \n", obj.name, type);
        // }
#endif
        LogSys(sb.ToString());
    }

    static void WriteToLogFile(string content)
    {
        if (isInit)
        {
            lock (locked)
            {
                logStreamWriter?.Write(content);
                logStreamWriter.Flush();
            }
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
