using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class TextBinder : MaskableGraphicBinder {

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

        Text m_Text;

        #region  重写属性
        public string text {
            get { return m_Text.text; }
            set {
                m_Text.text = value;
                OnPropertyChanged ();
            }
        }

        public bool supportRichText {
            get { return m_Text.supportRichText; }
            set {
                m_Text.supportRichText = value;
                OnPropertyChanged ();
            }
        }

        public bool resizeTextForBestFit {
            get { return m_Text.resizeTextForBestFit; }
            set {
                m_Text.resizeTextForBestFit = value;
                OnPropertyChanged ();
            }
        }

        public int resizeTextMinSize {
            get { return m_Text.resizeTextMinSize; }
            set {
                m_Text.resizeTextMinSize = value;
                OnPropertyChanged ();
            }
        }

        public int resizeTextMaxSize {
            get { return m_Text.resizeTextMaxSize; }
            set {
                m_Text.resizeTextMaxSize = value;
                OnPropertyChanged ();
            }
        }

        public TextAnchor alignment {
            get { return m_Text.alignment; }
            set {
                m_Text.alignment = value;
                OnPropertyChanged ();
            }
        }

        public bool alignByGeometry {
            get { return m_Text.alignByGeometry; }
            set {
                m_Text.alignByGeometry = value;
                OnPropertyChanged ();
            }
        }

        public int fontSize {
            get { return m_Text.fontSize; }
            set {
                m_Text.fontSize = value;
                OnPropertyChanged ();
            }
        }

        public HorizontalWrapMode horizontalOverflow {
            get { return m_Text.horizontalOverflow; }
            set {
                m_Text.horizontalOverflow = value;
                OnPropertyChanged ();
            }
        }

        public VerticalWrapMode verticalOverflow {
            get { return m_Text.verticalOverflow; }
            set {
                m_Text.verticalOverflow = value;
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

        public FontStyle fontStyle {
            get { return m_Text.fontStyle; }
            set {
                m_Text.fontStyle = value;
                OnPropertyChanged ();
            }
        }

        public float pixelsPerUnit {
            get { return m_Text.pixelsPerUnit; }
        }

        #endregion

        void Awake () {
            m_Text = GetTarget<Text> ();
            base.Awake ();
        }

    }
}