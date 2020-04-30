using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{

    [RequireComponent(typeof(Text))]
    public class TextBinder : MaskableGraphicBinder<Text>
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
                OnPropertyChanged();
            }
        }

        public bool supportRichText
        {
            get { return target.supportRichText; }
            set
            {
                target.supportRichText = value;
                OnPropertyChanged();
            }
        }

        public bool resizeTextForBestFit
        {
            get { return target.resizeTextForBestFit; }
            set
            {
                target.resizeTextForBestFit = value;
                OnPropertyChanged();
            }
        }

        public int resizeTextMinSize
        {
            get { return target.resizeTextMinSize; }
            set
            {
                target.resizeTextMinSize = value;
                OnPropertyChanged();
            }
        }

        public int resizeTextMaxSize
        {
            get { return target.resizeTextMaxSize; }
            set
            {
                target.resizeTextMaxSize = value;
                OnPropertyChanged();
            }
        }

        public TextAnchor alignment
        {
            get { return target.alignment; }
            set
            {
                target.alignment = value;
                OnPropertyChanged();
            }
        }

        public bool alignByGeometry
        {
            get { return target.alignByGeometry; }
            set
            {
                target.alignByGeometry = value;
                OnPropertyChanged();
            }
        }

        public int fontSize
        {
            get { return target.fontSize; }
            set
            {
                target.fontSize = value;
                OnPropertyChanged();
            }
        }

        public HorizontalWrapMode horizontalOverflow
        {
            get { return target.horizontalOverflow; }
            set
            {
                target.horizontalOverflow = value;
                OnPropertyChanged();
            }
        }

        public VerticalWrapMode verticalOverflow
        {
            get { return target.verticalOverflow; }
            set
            {
                target.verticalOverflow = value;
                OnPropertyChanged();
            }
        }

        public float lineSpacing
        {
            get { return target.lineSpacing; }
            set
            {
                target.lineSpacing = value;
                OnPropertyChanged();
            }
        }

        public FontStyle fontStyle
        {
            get { return target.fontStyle; }
            set
            {
                target.fontStyle = value;
                OnPropertyChanged();
            }
        }

        public float pixelsPerUnit
        {
            get { return target.pixelsPerUnit; }
        }

        #endregion

    }
}