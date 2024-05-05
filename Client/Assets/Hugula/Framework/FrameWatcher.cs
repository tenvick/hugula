#define SHARE_FRAME_TIME
using UnityEngine;

/// <summary>
/// 帧管理器 监控该帧运行的时间
/// </summary>
[XLua.LuaCallCSharp]
public class FrameWatcher
{
    private static int frameCount = -1;
    private static System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
    private static float beginTime = 0;
    /// <summary>
    /// 每帧最长分帧时长
    /// </summary>
    public static int FRAME_MAX_DURA = 500;
    /// <summary>
    /// 开始监听
    /// </summary>
    public static float BeginWatch()
    {
#if SHARE_FRAME_TIME
        if (Time.frameCount != frameCount)
        {
            frameCount = Time.frameCount;
            watch.Restart();
        }

        beginTime = watch.ElapsedMilliseconds;
        return beginTime;
#else
        watch.Restart();
        return watch.ElapsedMilliseconds;
#endif
    }
    /// <summary>
    /// 是否已经超时
    /// </summary>
    /// <param name="dura"> 本次监听最多时长 毫秒 </param>
    public static bool IsTimeOver(float dura = 100)
    {
#if SHARE_FRAME_TIME
        var cur = watch.ElapsedMilliseconds;
        if (cur >= FRAME_MAX_DURA || cur > beginTime + dura)
            return true;
        return false;
#else
        if (watch.ElapsedMilliseconds > dura)
            return true;
        return false;
#endif
    }

    internal static float ElapsedMilliseconds
    {
        get{
            return watch.ElapsedMilliseconds;
        }
    }
}