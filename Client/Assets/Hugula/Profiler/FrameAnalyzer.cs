using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using UnityEngine;
namespace Hugula.Profiler
{

    public class FrameAnalyzer : MonoBehaviour
    {
        struct FrameData
        {
            public float fFrameTime;
            public uint nFPSRaw;
        }
        NativeArray<FrameData> m_FrameData;

        uint m_frameCount = 0;

        // Estimate the next frame time and update the resolution scale if necessary.
        private void DetermineResolution()
        {
            ++m_frameCount;

            FrameData kFrame = new FrameData();
            kFrame.fFrameTime = Time.unscaledDeltaTime;
            kFrame.nFPSRaw = (uint)(1f / Time.unscaledDeltaTime);
            if (m_frameCount > m_FrameData.Length - 1)
            {
                m_frameCount = 0;
            }
            m_FrameData[(int)m_frameCount] = kFrame;
        }

        // Start is called before the first frame update
        void Start()
        {
            if (ProfilerFactory.DoNotProfile == true)
                return;
            m_FrameData = new NativeArray<FrameData>(10000, Allocator.Persistent);
        }

        // Update is called once per frame
        void Update()
        {
            if (ProfilerFactory.DoNotProfile == true)
                return;
            DetermineResolution();
        }

        private void OnDestroy()
        {
            if (ProfilerFactory.DoNotProfile == true)
                return;
            DumpFrameDataToLog();
            m_FrameData.Dispose();
        }

        private int m_nLogCount = 0;
        public void ResetFrame()
        {
            m_frameCount = 0;
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
        public void DumpFrameDataToLog()
        {
            if (ProfilerFactory.DoNotProfile == true)
                return;
            string kDirName = Application.persistentDataPath + "/Log/";
            if (File.Exists(kDirName) == false)
            {
                Directory.CreateDirectory(kDirName);
            }
            string kLogFileName = kDirName + "FrameData" + m_nLogCount++ + ".txt";
            if (File.Exists(kLogFileName))
            {
                File.Delete(kLogFileName);
            }
            FileStream kFile = File.Create(kLogFileName);
            foreach (FrameData kFrame in m_FrameData)
            {
                if (kFrame.nFPSRaw == 0)
                    break;
                string kLine = kFrame.fFrameTime + "," + kFrame.nFPSRaw + "\n";
                WriteToStream(kFile, kLine);
            }
            kFile.Flush();
            Debug.LogFormat("FrameData.txt in path {0}", kLogFileName);
        }
    }
}