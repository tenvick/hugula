//#define MESSAGEBOX_DEBUG
using System.Collections;
using System.Collections.Generic;
using Hugula.Loader;
using Hugula.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
namespace Hugula.UI {

    [SLua.CustomLuaClass]
    public class MessageBox : MonoBehaviour {
        private const string MESSAGEBOX_ABNAME = "system_messagebox" + Common.CHECK_ASSETBUNDLE_SUFFIX;
        public Text text;
        public Text caption;
        public Button[] buttons;

        public Button buttonClose;

        /// <summary>
        /// This function is called when the MonoBehaviour will be destroyed.
        /// </summary>
        void OnDestroy () {
            Button btn;
            for (int i = 0; i < buttons.Length; i++) {
                btn = buttons[i];
                btn.onClick.RemoveAllListeners ();
            }
        }

        public void ShowContent (string text, string caption, MessageBoxButton[] btnContents) {
#if MESSAGEBOX_DEBUG 
            Debug.LogFormat ("ShowContent {0}  ", gameObject);
#endif
            if (!gameObject.activeSelf) gameObject.SetActive (true);

            this.text.text = text;
            this.caption.text = caption;

            var btns = buttons;
            Button btn;

            if (btnContents != null) {
                for (int i = 0; i < btns.Length; i++) {
                    btn = btns[i];
                    if (i < btnContents.Length) {
                        btn.gameObject.SetActive (true);
                        btn.onClick.RemoveAllListeners ();
                        var msgBoxBtn = btnContents[i];
                        if (msgBoxBtn.onClick != null) btn.onClick.AddListener (msgBoxBtn.onClick);
                        if (!string.IsNullOrEmpty (msgBoxBtn.btnText)) {
                            var txt = btn.GetComponentInChildren<Text> (true);
                            if (txt) txt.text = msgBoxBtn.btnText;
                        }

                    } else {
                        btn.gameObject.SetActive (false);
                    }
                }
            }
        }

        public void ShowContent (string text, string caption, string btn1Text, UnityAction btn1OnClick, string btn2Text = "", UnityAction btn2OnClick = null) {
            List<MessageBoxButton> msgBoxBtns = new List<MessageBoxButton> ();
            var msgInfo1 = new MessageBoxButton { btnText = btn1Text, onClick = btn1OnClick };
            msgBoxBtns.Add (msgInfo1);
            if (btn2OnClick != null) {
                var msgInfo2 = new MessageBoxButton { btnText = btn2Text, onClick = btn2OnClick };
                msgBoxBtns.Add (msgInfo2);
            }

            ShowContent (text, caption, msgBoxBtns.ToArray ());
        }

        public void Hide () {
            this.gameObject.SetActive (false);
        }

        private static IEnumerator LoadMessageBox (MessageBoxInfo m_messageBoxInfo) {
#if MESSAGEBOX_DEBUG 
            Debug.LogFormat ("LoadMessageBox .LoadAssetCoroutine {0} ,frame={1}  ", m_messageBoxInfo,Time.frameCount);
#endif
            var req = ResourcesLoader.LoadAssetCoroutine (CUtils.GetRightFileName (MESSAGEBOX_ABNAME), CUtils.GetAssetName (MESSAGEBOX_ABNAME), typeof (GameObject), int.MaxValue);
            yield return req;
#if MESSAGEBOX_DEBUG 
            Debug.LogFormat ("LoadMessageBox LoadAssetCoroutine is done {0},frame={1}  ", req,Time.frameCount);
#endif
            var obj = (GameObject)req.data;
#if MESSAGEBOX_DEBUG 
            Debug.LogFormat ("LoadMessageBox obj req.GetAsset<GameObject> {0} ,frame={1}  ", obj,Time.frameCount);
#endif
            var ins = GameObject.Instantiate (obj);
            m_messageBox = ins.GetComponent<MessageBox> ();
            m_isloading = false;
            DontDestroyOnLoad (ins);
            m_messageBox.ShowContent (m_messageBoxInfo.text, m_messageBoxInfo.caption, m_messageBoxInfo.btns);
        }

        public static void Show (string text, string caption, string btn1Text, UnityAction btn1OnClick) {
            var msgInfo = new MessageBoxButton { btnText = btn1Text, onClick = btn1OnClick };
            Show (text, caption, new MessageBoxButton[] { msgInfo });
        }

        public static void Show (string text, string caption, string btn1Text, UnityAction btn1OnClick, string btn2Text, UnityAction btn2OnClick) {
            var msgInfo1 = new MessageBoxButton { btnText = btn1Text, onClick = btn1OnClick };
            var msgInfo2 = new MessageBoxButton { btnText = btn2Text, onClick = btn2OnClick };
            Show (text, caption, new MessageBoxButton[] { msgInfo1, msgInfo2 });
        }

        public static void Show (string text, string caption, MessageBoxButton[] btns) {
#if MESSAGEBOX_DEBUG 
            Debug.LogFormat ("MessageBox.Show {0} frame={1}", text,Time.frameCount);
#endif
            if (m_messageBox != null) {
                m_messageBox.gameObject.SetActive (true);
                m_messageBox.ShowContent (text, caption, btns);
#if MESSAGEBOX_DEBUG 
                Debug.LogFormat ("m_messageBox.ShowContent {0} frame={1} ", text,Time.frameCount);
#endif
            } else if (!m_isloading) {
#if MESSAGEBOX_DEBUG 
                Debug.LogFormat ("Show {0} load asset frame={1}", text,Time.frameCount);
#endif
                m_isloading = true;
                var m_messageBoxInfo = new MessageBoxInfo ();
                m_messageBoxInfo.text = text;
                m_messageBoxInfo.caption = caption;
                m_messageBoxInfo.btns = btns;
                ResourcesLoader.instance.StartCoroutine (LoadMessageBox (m_messageBoxInfo));

            }
        }

        public static void Destroy () {
            if (m_messageBox != null) {
                GameObject.Destroy (m_messageBox.gameObject);
            }
            m_messageBox = null;
        }

        public static void Close () {
            if (m_messageBox != null)
                m_messageBox.gameObject.SetActive (false);
        }

        // private static MessageBoxInfo m_messageBoxInfo = new MessageBoxInfo ();
        private static MessageBox m_messageBox;

        private static bool m_isloading;
    }

    [SLua.CustomLuaClass]
    public class MessageBoxButton {
        public string btnText;
        public UnityAction onClick;
    }

    [SLua.CustomLuaClass]
    public class MessageBoxInfo {
        public string text;
        public string caption;
        public MessageBoxButton[] btns;
    }
}