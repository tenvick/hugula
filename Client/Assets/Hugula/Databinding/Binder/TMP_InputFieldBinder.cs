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
    [RequireComponent(typeof(TMP_InputField))]
    [XLua.LuaCallCSharp]
    public class TMP_InputFieldBinder : SelectableBinder<TMP_InputField>
    {


        public const string SubmitEventCommandProperty = "submitEventCommand";
        public const string TextProperty = "text";

        #region  重写属性
        public bool shouldHideMobileInput
        {
            set
            {
                target.shouldHideMobileInput = value;
                // OnPropertyChanged();
            }
            get
            {
                return target.shouldHideMobileInput;
            }
        }

        public string text
        {
            set
            {
                target.text = value;
                // OnPropertyChanged();
            }
            get
            {
                return target.text;
            }
        }

        public bool isFocused
        {
            get
            {
                return target.isFocused;
            }
        }

        public float caretBlinkRate
        {
            set
            {
                target.caretBlinkRate = value;
                // OnPropertyChanged();
            }
            get
            {
                return target.caretBlinkRate;
            }
        }

        public int caretWidth
        {
            set
            {
                target.caretWidth = value;
                // OnPropertyChanged();
            }
            get
            {
                return target.caretWidth;
            }
        }

        public TMP_Text textComponent
        {
            set
            {
                target.textComponent = value;
                // OnPropertyChanged();
            }
            get
            {
                return target.textComponent;
            }
        }

        public Graphic placeholder
        {
            set
            {
                target.placeholder = value;
                // OnPropertyChanged();
            }
            get
            {
                return target.placeholder;
            }
        }

        public Color caretColor
        {
            set
            {
                target.caretColor = value;
                // OnPropertyChanged();
            }
            get
            {
                return target.caretColor;
            }
        }

        public bool customCaretColor
        {
            set
            {
                target.customCaretColor = value;
                // OnPropertyChanged();
            }
            get
            {
                return target.customCaretColor;
            }
        }

        public Color selectionColor
        {
            set
            {
                target.selectionColor = value;
                // OnPropertyChanged();
            }
            get
            {
                return target.selectionColor;
            }
        }

        public TMP_InputField.OnValidateInput onValidateInput
        {
            get { return target.onValidateInput; }
            set
            {
                target.onValidateInput = value;
                // OnPropertyChanged();
            }
        }

        public int characterLimit
        {
            get { return target.characterLimit; }
            set
            {
                target.characterLimit = value;
                // OnPropertyChanged();
            }
        }

        public TMP_InputField.ContentType contentType
        {
            get { return target.contentType; }
            set
            {
                target.contentType = value;
                // OnPropertyChanged();
            }
        }

        public TMP_InputField.LineType lineType
        {
            get { return target.lineType; }
            set
            {
                target.lineType = value;
                // OnPropertyChanged();
            }
        }

        public TMP_InputField.InputType inputType
        {
            get { return target.inputType; }
            set
            {
                target.inputType = value;
                // OnPropertyChanged();
            }
        }

        public TouchScreenKeyboardType keyboardType
        {
            get { return target.keyboardType; }
            set
            {
                target.keyboardType = value;
                // OnPropertyChanged();
            }
        }

        public TMP_InputField.CharacterValidation characterValidation
        {
            get { return target.characterValidation; }
            set
            {
                target.characterValidation = value;
                // OnPropertyChanged();
            }
        }

        public bool readOnly
        {
            get { return target.readOnly; }
            set
            {
                target.readOnly = value;
                // OnPropertyChanged();
            }
        }

        public bool multiLine
        {
            get { return target.multiLine; }
        }

        public char asteriskChar
        {
            get { return target.asteriskChar; }
            set
            {
                target.asteriskChar = value;
                // OnPropertyChanged();
            }
        }

        public bool wasCanceled
        {
            get { return target.wasCanceled; }
        }

        public int caretPosition
        {
            get { return target.caretPosition; }
            set
            {
                target.caretPosition = value;
                // OnPropertyChanged();
            }
        }

        public int selectionAnchorPosition
        {
            get { return target.selectionAnchorPosition; }
            set
            {
                target.selectionAnchorPosition = value;
                // OnPropertyChanged();
            }
        }

        public int selectionFocusPosition
        {
            get { return target.selectionFocusPosition; }
            set
            {
                target.selectionFocusPosition = value;
                // OnPropertyChanged();
            }
        }

        #endregion 

        private ICommand m_SubmitEventCommand;
        public ICommand submitEventCommand
        {
            get
            {
                return m_SubmitEventCommand;
            }
            set
            {
                m_SubmitEventCommand = value;
                // OnPropertyChanged();
            }
        }

        public object m_commandParameter;

        public object commandParameter
        {
            get { return m_commandParameter; }
            set
            {
                m_commandParameter = value;
            }
        }

        private IExecute m_OnValueChangedExecute;
        public IExecute onValueChangedExecute
        {
            get
            {
                return m_OnValueChangedExecute;
            }
            set
            {
                m_OnValueChangedExecute = value;
                // OnPropertyChanged();
            }
        }

        void OnSubmitEvent(string value)
        {
            OnPropertyChangedBindingApply(TextProperty);
            if (m_SubmitEventCommand != null && m_SubmitEventCommand.CanExecute(m_commandParameter))
                m_SubmitEventCommand.Execute(value);
        }

        void OnValueChanged(string value)
        {
            if (m_OnValueChangedExecute != null) //&& m_OnClickCommand.can_execute (m_commandParameter)
                m_OnValueChangedExecute.Execute(value);
        }
        protected override void Awake()
        {
            base.Awake();
            target.onEndEdit.AddListener(OnSubmitEvent);
            target.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void OnDestroy()
        {
            target.onEndEdit.RemoveListener(OnSubmitEvent);
            target.onValueChanged.RemoveListener(OnValueChanged);
            m_commandParameter = null;
            m_SubmitEventCommand = null;
            m_OnValueChangedExecute = null;
            base.OnDestroy();
        }


    }
#endif
}