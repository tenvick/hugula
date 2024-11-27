using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if USE_TMPro
using TMPro;
#endif
namespace Hugula.Databinding.Binder
{

#if USE_TMPro
        [RequireComponent(typeof(TextMeshProUGUI))]
        public class TextMeshProUGUIBinder : MaskableGraphicBinder<TextMeshProUGUI>
        {
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

    #region  重写属性
        public string text
        {
            get { return target.text; }
            set
            {
                target.text = value;
                // OnPropertyChanged();
            }
        }

        public float alpha
        {
            get { return target.alpha; }
            set
            {
                target.alpha = value;
                // OnPropertyChanged();
            }
        }

        public TMPro.TextAlignmentOptions alignment
        {
            get { return target.alignment; }
            set
            {
                target.alignment = value;
                // OnPropertyChanged();
            }
        }

        public float fontSize
        {
            get { return target.fontSize; }
            set
            {
                target.fontSize = value;
                // OnPropertyChanged();
            }
        }

        public float lineSpacing
        {
            get { return target.lineSpacing; }
            set
            {
                target.lineSpacing = value;
                // OnPropertyChanged();
            }
        }

        public float pixelsPerUnit
        {
            get { return target.pixelsPerUnit; }
        }

    #endregion

    }
#endif
}