using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Hugula.Loader;
using Hugula.Utils;
using UnityEngine.UI;

namespace Hugula.UI
{

    [SLua.CustomLuaClass]
    public class MessageBox : MonoBehaviour
    {
        private const string MESSAGEBOX_ABNAME = "system_messagebox" + Common.CHECK_ASSETBUNDLE_SUFFIX;
        public Text text;
        public Text caption;
        public Button[] buttons;

        public Button buttonClose;

        // Use this for initialization
        void Start()
        {
            m_messageBox = this;
            m_isloading = false; 
            DontDestroyOnLoad(this.gameObject);
            if (buttonClose) buttonClose.onClick.AddListener(Close);
        }

        private static IEnumerator LoadMessageBox()
        {
            var req = ResourcesLoader.LoadAssetCoroutine(CUtils.GetRightFileName(MESSAGEBOX_ABNAME), CUtils.GetAssetName(MESSAGEBOX_ABNAME), typeof(GameObject), int.MaxValue);
            yield return req;
            var obj = req.GetAsset<GameObject>();
            var ins = GameObject.Instantiate(obj);
            var msgbox = ins.GetComponent<MessageBox>();
            SetContent(msgbox, m_messageBoxInfo);
        }

        private static void SetContent(MessageBox messageBox, MessageBoxInfo messageBoxInfo)
        {
            messageBox.text.text = messageBoxInfo.text;
            messageBox.caption.text = messageBoxInfo.caption;

            var btns = messageBox.buttons;
            var btnContents = messageBoxInfo.btns;
            Button btn;

            if (btnContents != null)
            {
                for (int i = 0; i < btns.Length; i++)
                {
                    btn = btns[i];
                    if (i < btnContents.Length)
                    {
                        btn.gameObject.SetActive(true);
                        btn.onClick.RemoveAllListeners();
                        if (btnContents[i].onClick != null) btn.onClick.AddListener(btnContents[i].onClick);
                    }
                    else
                    {
                        btn.gameObject.SetActive(false);
                    }
                }
            }
        }


        public static void Show(string text, string caption, string btn1Text, UnityAction btn1OnClick)
        {
            var msgInfo = new MessageBoxButton { btnText = btn1Text, onClick = btn1OnClick };
            Show(text, caption, new MessageBoxButton[] { msgInfo });
        }


        public static void Show(string text, string caption, string btn1Text, UnityAction btn1OnClick, string btn2Text, UnityAction btn2OnClick)
        {
            var msgInfo1 = new MessageBoxButton { btnText = btn1Text, onClick = btn1OnClick };
            var msgInfo2 = new MessageBoxButton { btnText = btn2Text, onClick = btn2OnClick };
            Show(text, caption, new MessageBoxButton[] { msgInfo1, msgInfo2 });
        }

        public static void Show(string text, string caption, MessageBoxButton[] btns)
        {
            m_messageBoxInfo.text = text;
            m_messageBoxInfo.caption = caption;
            m_messageBoxInfo.btns = btns;

            if (m_messageBox != null)
            {
                m_messageBox.gameObject.SetActive(true);
                SetContent(m_messageBox, m_messageBoxInfo);
            }
            else if (!m_isloading)
            {
                m_isloading = true;
                ResourcesLoader.instance.StartCoroutine(LoadMessageBox());
            }
        }

        public static void Destroy()
        {
            if (m_messageBox != null)
            {
                GameObject.Destroy(m_messageBox.gameObject);
            }
            m_messageBox = null;
        }

        public static void Close()
        {
            if (m_messageBox != null)
                m_messageBox.gameObject.SetActive(false);
        }

        private static MessageBoxInfo m_messageBoxInfo = new MessageBoxInfo();
        private static MessageBox m_messageBox;

        private static bool m_isloading;
    }

    [SLua.CustomLuaClass]
    public class MessageBoxButton
    {
        public string btnText;
        public UnityAction onClick;
    }

    [SLua.CustomLuaClass]
    public class MessageBoxInfo
    {
        public string text;
        public string caption;
        public MessageBoxButton[] btns;
    }
}