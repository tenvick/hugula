using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Hugula
{

    public class FPS : MonoBehaviour
    {
        /// <summary>
        /// 协程实现循环计算，省去了update里边的代码；用OnGUI来实现解耦合
        /// </summary>

        public static int guiFontSize = 24;
        public string FpsCountLabel = string.Empty;
        private GUIStyle guiStyle = new GUIStyle();
        public static float FPSCount = 30f;


        public static bool IsSuperLowDevice = false;

        //电影帧当前帧率小于24帧每秒，也就是frame time >40ms人眼就感觉卡顿
        public const float MOVIE_FRAMETIME1 = 0.04167f;//.34f;//41.67f*2;
        public const float MOVIE_FRAMETIME2 = 0.125f;//41.67f*3;

        public static int Value = 30;
        public float RefreshTime = 0.1f;//1.0f;

        private int mFrameCounter = 0;
        private float mTimeCounter = 0;
        internal static FpsSample fpsSample = new FpsSample();

        #region Fps Sample
        public class FpsSample
        {
            //总帧数
            private int mTotal = 0;
            private int mAvg = 0;
            //总时间
            private float mTotalTime = 0;
            private System.Action<int> mCb = null;
            private float mMinTime = 0;
            private float mMaxTime = 0;
            private bool mSampling = false;
            //记录三帧耗时
            private float[] mThreeFrameTimes = new float[3];
            private float mDisplayFrameTime = 0.01667f;// 60fps
            public void Begin(System.Action<int> cb)
            {
                Reset();

                mCb = cb;
                mSampling = true;
                // if (CurQuality == QualityEnum.LOW)
                //     mDisplayFrameTime = 0.03333f;//30fps
                // else
                //     mDisplayFrameTime = 0.01667f;//60fps
            }

            public void End()
            {
                // Reset();
                mAvg = (int)(mTotal / mTotalTime + 0.5f);
                mSampling = false;
            }

            public void Flush()
            {
                if (mSampling)
                {
                    Reset();
                }
            }

            private void Reset()
            {
                Jank1 = 0;
                Jank2 = 0;
                mTotal = 0;
                mAvg = 0;
                for (int i = 0; i < mThreeFrameTimes.Length; i++)
                    mThreeFrameTimes[i] = 0;
                mMinTime = float.MaxValue;
                mMaxTime = float.MinValue;
                mTotalTime = 0;
                mCb = null;
            }

            public bool IsSampling() { return mSampling; }

            public int AvgFps { get { return mAvg; } }
            public float MaxFpsTime { get { return mMaxTime; } }
            public float MinFpsTime { get { return mMinTime; } }
            public int TotalFrame { get { return mTotal; } }
            public float TotalTime { get { return mTotalTime; } }
            public int Jank0 { get; private set; }
            public int Jank1 { get; private set; }
            public int Jank2 { get; private set; }

            public void Update(float frameDeltaTime)
            {
                mTotalTime += frameDeltaTime;
                mMinTime = frameDeltaTime < mMinTime ? frameDeltaTime : mMinTime;
                mMaxTime = frameDeltaTime > mMaxTime ? frameDeltaTime : mMaxTime;


                //同时满足两条件，则认为是一次卡顿Jank.
                //①Display FrameTime>前三帧平均耗时2倍。
                //②Display FrameTime>两帧电影帧耗时 (1000ms/24*2=84ms)。
                float threeTotal3 = ThreeFrameTotal();
                float avg = 1.5f * (threeTotal3 / 3f); //考虑视觉连贯性
                if (frameDeltaTime >= avg && mTotal > 3)// (frameDeltaTime > mDisplayFrameTime || 
                {
                    if (frameDeltaTime >= MOVIE_FRAMETIME2) //严重卡顿
                        Jank2++;
                    else if (frameDeltaTime >= MOVIE_FRAMETIME1)//卡顿
                        Jank1++;
                    else
                        Jank0++; //
                }
                // 同时满足两条件，则认为是一次严重卡顿BigJank.
                // ①Display FrameTime >前三帧平均耗时2倍。
                // ②Display FrameTime >三帧电影帧耗时(1000ms/24*3=125ms)。    
                mThreeFrameTimes[mTotal % 3] = frameDeltaTime;//记录上一帧
                mTotal++;

            }

            private float ThreeFrameTotal()
            {
                float t = 0;
                for (int i = 0; i < mThreeFrameTimes.Length; i++)
                    t += mThreeFrameTimes[i];

                return t;
            }
        }

        public static void BeginSample(System.Action<int> Cb = null)
        {
            fpsSample.Begin(Cb);
        }

        public static void EndSample()
        {
            fpsSample.End();
        }

        public static void FlushSample()
        {
            fpsSample.Flush();
        }

        public static void SetSampleInterval(int seconds)
        {
            // fpsSample.MaxCount = seconds;
        }
        #endregion

        void Update()
        {
            if (mTimeCounter > RefreshTime)
            {
                Value = (int)(mFrameCounter / mTimeCounter + 0.5f);
                mFrameCounter = 0;
                mTimeCounter = 0;
            }
            else
            {
                mFrameCounter++;
                mTimeCounter += Time.deltaTime;
            }

            if (fpsSample.IsSampling())
            {
                fpsSample.Update(Time.deltaTime);
            }
        }

#if !HUGULA_RELEASE
        void OnGUI()
        {
            // GUILayout.Label("fps:" + Value);
            //  UnityEngine.Experimental.LowLevel.PlayerLoopSystem player = UnityEngine.Experimental.LowLevel.PlayerLoop.GetDefaultPlayerLoop();
            // player.subSystemList
            GUI.Label(new Rect(0,50,200,200),"fps:"+Value);

        }
#endif
    }
}
