// 倒计时组件
// author pu
// data 2020.4.10
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using Hugula.Databinding;
///<summary>
/// 剩余时间 单位秒
///</summary>

namespace Hugula.UIComponents
{
    /// <summary>
    /// 使用指定的格式显示倒计时
    ///
    /// 
    /// </summary>
    public class CoolDown : BindableObject
    {
        public Text text;

        public string prefix = string.Empty;
        private int m_RemainTime;
        ///<summary>
        /// 剩余时间 单位秒
        ///</summary>
        public int remainTime
        {
            get { return m_RemainTime; }
            set
            {
                m_RemainTime = value;
                SetText(m_RemainTime);
                BeginCD();
                OnPropertyChanged();
            }
        }

        private ICommand m_OnCompleted;

        public ICommand onCompleted
        {
            get { return m_OnCompleted; }
            set { m_OnCompleted = value; }
        }

        public object onCompletedParameter { get; set; }

        public enum TimeStyle
        {
            ///<summary>
            /// 以00:00:00 格式显示倒计时
            ///</summary>
            HHMMSS
        }

        public TimeStyle timeStyle;

        //private
        private bool m_BeginCD = false;
        private float m_Dt = 0;
        private float m_PassedTime = 0;
        private StringBuilder sb = new StringBuilder();

        // Start is called before the first frame update
        void Start()
        {
            if (text == null) text = this.GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            if (m_BeginCD)
            {
                m_Dt += Time.deltaTime;
                m_PassedTime += Time.deltaTime;
                int remain = m_RemainTime - (int)m_PassedTime;
                if (m_Dt >= 1)
                {
                    SetText(remain);
                }

                if (remain <= 0)
                {
                    m_BeginCD = false;
                    SetText(0);
                    if (onCompleted != null)
                    {
                        if (onCompleted.CanExecute(onCompletedParameter))
                            onCompleted.Execute(onCompletedParameter);
                    }
                }
            }
        }

        void BeginCD()
        {
            m_BeginCD = true;
            m_Dt = 0;
            m_PassedTime = 0;
        }

        void SetText(int remainseconds)
        {
            if (text)
            {
                text.text = prefix + TimeFormatHHMMSS(remainseconds);
            }
        }

        string TimeFormatHHMMSS(int remainseconds)
        {

            int h = remainseconds / 3600;
            int m = (remainseconds - h * 3600) / 60;
            int s = remainseconds - h * 3600 - m * 60;
            sb.Clear();
            if (h <= 9)
            {
                sb.Append("0");
                sb.Append(h);
            }
            else
                sb.Append(h);

            sb.Append(":");

            if (m <= 9)
            {
                sb.Append("0");
                sb.Append(m);
            }
            else
            {
                sb.Append(m);
            }

            sb.Append(":");

            if (s <= 9)
            {
                sb.Append("0");
                sb.Append(s);
            }
            else
            {
                sb.Append(s);
            }
            return sb.ToString();
        }
    }
}
