//----------------------------------------------
//            NGUI: Next-Gen UI kit
// Copyright © 2011-2014 Tasharen Entertainment
//----------------------------------------------
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Simple script that lets you localize a UIWidget.
/// </summary>
namespace Hugula {

    [ExecuteInEditMode]
    [RequireComponent (typeof (MaskableGraphic))]
    
    public class UGUILocalize : MonoBehaviour {
        /// <summary>
        /// Localization key.
        /// </summary>

        public string key;

        [Tooltip("从Luafunction中获取值")]
        public bool luaFunction = true;

        public static System.Func<string,string> LocaltionFun;

        /// <summary>
        /// Manually change the value of whatever the localization component is attached to.
        /// </summary>

        public string value {
            set {
                if (!string.IsNullOrEmpty (value)) {
                    MaskableGraphic w = GetComponent<MaskableGraphic> ();
                    //Image 
                    Text lbl = w as Text;
                    Image sp = w as Image;

                    if (lbl != null) {
                        // If this is a label used by input, we should localize its default value instead
                        //UIInput input = NGUITools.FindInParents<UIInput>(lbl.gameObject);
                        //if (input != null && input.label == lbl) input.defaultText = value;
                        //else 
                        lbl.text = value;

                    } else if (sp != null) {
                        sp.sprite.name = value;

                    }
                }
            }
        }

        bool mStarted = false;

        /// <summary>
        /// Localize the widget on enable, but only if it has been started already.
        /// </summary>

        void OnEnable () {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            if (mStarted) OnLocalize ();
        }

        /// <summary>
        /// Localize the widget on start.
        /// </summary>

        void Start () {
#if UNITY_EDITOR
            if (!Application.isPlaying) return;
#endif
            mStarted = true;
            OnLocalize ();
        }

        /// <summary>
        /// This function is called by the Localization manager via a broadcast SendMessage.
        /// </summary>

        void OnLocalize () {
            // If no localization key has been specified, use the label's text as the key
            if (string.IsNullOrEmpty (key)) {
                Text lbl = GetComponent<Text> ();
                if (lbl != null) key = lbl.text;
            }

            // If we still don't have a key, leave the value as blank
            if (!string.IsNullOrEmpty (key))
            {

               if(luaFunction && LocaltionFun!=null)
               {
                    value = LocaltionFun.Invoke(key);
               }
               else
               {
                    value = Localization.Get (key);
               }
            } 
        }
    }
}