// Copyright (c) 2015 hugula
// direct https://github.com/tenvick/hugula
//
using UnityEngine;
using System.Collections;
using System.Text;

namespace Hugula.Utils
{
    public class ProfilerPanel : MonoBehaviour
    {
        const float m_KBSize = 1024.0f * 1024.0f;
		private string memory,framerate;
		private StringBuilder memory1;

        private float updateInterval = 1F;
        private float accum = 0; // FPS accumulated over the interval
        private int frames = 0; // Frames drawn over the interval
        private float timeleft; // Left time for current interval

        void Start()
        {
            timeleft = updateInterval;
			memory1 = new StringBuilder ();
        }

        void Update()
        {
            UpdateGameInfo();
        }

        void OnGUI()
        {
            GUILayout.Label("fps:" + framerate);
			GUILayout.Label(memory);

        }
        /// <summary>
        /// 更新游戏信息
        /// </summary>
        void UpdateGameInfo()
        {
            float totalMemory = (float)(Profiler.GetTotalAllocatedMemory() / m_KBSize);
            float totalReservedMemory = (float)(Profiler.GetTotalReservedMemory() / m_KBSize);
            float totalUnusedReservedMemory = (float)(Profiler.GetTotalUnusedReservedMemory() / m_KBSize);
            float monoHeapSize = (float)(Profiler.GetMonoHeapSize() / m_KBSize);
            float monoUsedSize = (float)(Profiler.GetMonoUsedSize() / m_KBSize);

            timeleft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            ++frames;

            // Interval ended - update GUI text and start new interval
            if (timeleft <= 0.0)
            {
                // display two fractional digits (f2 format)
                float fps = accum / frames;
                framerate = Mathf.RoundToInt(fps).ToString();// ioo.f("fps:{0:F2}", fps);
                timeleft = updateInterval;
                accum = 0.0F;
                frames = 0;
            }

            memory = string.Format("TotalAllocatedMemory:{0}MB\n" + //TotalAllocatedMemory
                                        "TotalReservedMemory:{1}MB\n" +
                                        "TotalUnusedReservedMemory:{2}MB\n" +
                                        "MonoHeapSize:{3}MB\nMonoUsedSize:{4}MB",
										totalMemory.ToString(), totalReservedMemory.ToString(),
										totalUnusedReservedMemory.ToString(), monoHeapSize.ToString(), monoUsedSize.ToString());
        }
    }
}