// Copyright (c) 2014 hugula
// direct https://github.com/Hugulor/Hugula
//
using UnityEngine;
using System.Collections;

public class ProfilerPanel : MonoBehaviour {
    const float m_KBSize = 1024.0f * 1024.0f;
    private string memory, framerate;

    private float updateInterval = 0.2F;
    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval
    private float timeleft; // Left time for current interval

    void Start() {
        timeleft = updateInterval;
        //memory = Util.Get<UILabel>(gameObject, "MemoryInfo");
        //framerate = Util.Get<UILabel>(gameObject, "FrameRate");
    }

    void Update() {
        UpdateGameInfo();
    }

    void OnGUI()
    {
        GUILayout.Label("fps:" + framerate);
        GUILayout.Label("" + memory);

    }
    /// <summary>
    /// 更新游戏信息
    /// </summary>
    void UpdateGameInfo() {
        float totalMemory = (float)(Profiler.GetTotalAllocatedMemory() / m_KBSize);
        float totalReservedMemory = (float)(Profiler.GetTotalReservedMemory() / m_KBSize);
        float totalUnusedReservedMemory = (float)(Profiler.GetTotalUnusedReservedMemory() / m_KBSize);
        float monoHeapSize = (float)(Profiler.GetMonoHeapSize() / m_KBSize);
        float monoUsedSize = (float)(Profiler.GetMonoUsedSize() / m_KBSize);

        timeleft -= Time.deltaTime;
        accum += Time.timeScale / Time.deltaTime;
        ++frames;

        // Interval ended - update GUI text and start new interval
        if (timeleft <= 0.0) {
            // display two fractional digits (f2 format)
            float fps = accum / frames;
            framerate = fps.ToString();// ioo.f("fps:{0:F2}", fps);

            //if (fps < 30) {
            //    framerate.color = Color.yellow;
            //} else {
            //    if (fps < 10) {
            //        framerate.color = Color.red;
            //    } else {
            //        framerate.color = Color.green;
            //    }
            //}
            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
        memory = string.Format("TotalAllocatedMemory：{0}MB\n"+
                                    "TotalReservedMemory：{1}MB\n"+
                                    "TotalUnusedReservedMemory:{2}MB\n"+
                                    "MonoHeapSize:{3}MB\nMonoUsedSize:{4}MB", 
                                    totalMemory, totalReservedMemory, 
                                    totalUnusedReservedMemory, monoHeapSize, monoUsedSize);
    }
}
