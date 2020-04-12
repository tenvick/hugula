using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if USE_TMPro
using TMPro;
#endif
namespace Hugula.Databinding.Binder {

    public class TextMeshProUGUIBinder : MaskableGraphicBinder {
#if USE_TMPro

        public const string TextProperty = "text";
        public const string SupportRichTextProperty = "supportRichText";
        public const string resizeTextForBestFitProperty = "resizeTextForBestFit";
        public const string resizeTextMinSizeProperty = "resizeTextMinSize";
        public const string resizeTextMaxSizeProperty = "resizeTextMaxSize";
        public const string alignmentProperty = "alignment";
        public const string alignByGeometryProperty = "alignByGeometry";
        public const string fontSizeProperty = "fontSize";
        public const string horizontalOverflowProperty = "horizontalOverflow";
        public const string verticalOverflowProperty = "verticalOverflow";
        public const string lineSpacingProperty = "lineSpacing";
        public const string fontStyleProperty = "fontStyle";
        public const string pixelsPerUnitProperty = "pixelsPerUnit";

        TextMeshProUGUI m_target;
        TextMeshProUGUI m_Text {
            get {
                if (m_target == null)
                    m_target = GetTarget<TextMeshProUGUI> ();
                return m_target;
            }
            set {
                m_target = null;
            }
        }

        #region  重写属性
        public string text {
            get { return m_Text.text; }
            set {
                m_Text.text = value;
                OnPropertyChanged ();
            }
        }

        public float alpha {
            get { return m_Text.alpha; }
            set {
                m_Text.alpha = value;
                OnPropertyChanged ();
            }
        }

        public TMPro.TextAlignmentOptions alignment {
            get { return m_Text.alignment; }
            set {
                m_Text.alignment = value;
                OnPropertyChanged ();
            }
        }

        public float fontSize {
            get { return m_Text.fontSize; }
            set {
                m_Text.fontSize = value;
                OnPropertyChanged ();
            }
        }

        public float lineSpacing {
            get { return m_Text.lineSpacing; }
            set {
                m_Text.lineSpacing = value;
                OnPropertyChanged ();
            }
        }

        public float pixelsPerUnit {
            get { return m_Text.pixelsPerUnit; }
        }

        #endregion
   
        protected override void OnDestroy () {
            m_Text = null;
            base.OnDestroy ();
        }

#endif
    }
}