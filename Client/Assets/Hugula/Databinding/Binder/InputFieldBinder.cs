using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class InputFieldBinder : SelectableBinder {

        public const string SubmitEventCommandProperty = "submitEventCommand";
        public const string TextProperty = "text";

        InputField m_target;
        InputField m_InputField {
            get {
                if (m_target == null)
                    m_target = GetTarget<InputField> ();
                return m_target;
            }
            set {
                m_target = null;
            }
        }

        #region  重写属性
        public bool shouldHideMobileInput {
            set {
                m_InputField.shouldHideMobileInput = value;
                OnPropertyChanged ();
            }
            get {
                return m_InputField.shouldHideMobileInput;
            }
        }

        public string text {
            set {
                m_InputField.text = value;
                OnPropertyChanged ();
            }
            get {
                return m_InputField.text;
            }
        }

        public bool isFocused {
            get {
                return m_InputField.isFocused;
            }
        }

        public float caretBlinkRate {
            set {
                m_InputField.caretBlinkRate = value;
                OnPropertyChanged ();
            }
            get {
                return m_InputField.caretBlinkRate;
            }
        }

        public int caretWidth {
            set {
                m_InputField.caretWidth = value;
                OnPropertyChanged ();
            }
            get {
                return m_InputField.caretWidth;
            }
        }

        public Text textComponent {
            set {
                m_InputField.textComponent = value;
                OnPropertyChanged ();
            }
            get {
                return m_InputField.textComponent;
            }
        }

        public Graphic placeholder {
            set {
                m_InputField.placeholder = value;
                OnPropertyChanged ();
            }
            get {
                return m_InputField.placeholder;
            }
        }

        public Color caretColor {
            set {
                m_InputField.caretColor = value;
                OnPropertyChanged ();
            }
            get {
                return m_InputField.caretColor;
            }
        }

        public bool customCaretColor {
            set {
                m_InputField.customCaretColor = value;
                OnPropertyChanged ();
            }
            get {
                return m_InputField.customCaretColor;
            }
        }

        public Color selectionColor {
            set {
                m_InputField.selectionColor = value;
                OnPropertyChanged ();
            }
            get {
                return m_InputField.selectionColor;
            }
        }

        public InputField.OnValidateInput onValidateInput {
            get { return m_InputField.onValidateInput; }
            set {
                m_InputField.onValidateInput = value;
                OnPropertyChanged ();
            }
        }

        public int characterLimit {
            get { return m_InputField.characterLimit; }
            set {
                m_InputField.characterLimit = value;
                OnPropertyChanged ();
            }
        }

        public InputField.ContentType contentType {
            get { return m_InputField.contentType; }
            set {
                m_InputField.contentType = value;
                OnPropertyChanged ();
            }
        }

        public InputField.LineType lineType {
            get { return m_InputField.lineType; }
            set {
                m_InputField.lineType = value;
                OnPropertyChanged ();
            }
        }

        public InputField.InputType inputType {
            get { return m_InputField.inputType; }
            set {
                m_InputField.inputType = value;
                OnPropertyChanged ();
            }
        }

        public TouchScreenKeyboard touchScreenKeyboard {
            get { return m_InputField.touchScreenKeyboard; }
        }

        public TouchScreenKeyboardType keyboardType {
            get { return m_InputField.keyboardType; }
            set {
                m_InputField.keyboardType = value;
                OnPropertyChanged ();
            }
        }

        public InputField.CharacterValidation characterValidation {
            get { return m_InputField.characterValidation; }
            set {
                m_InputField.characterValidation = value;
                OnPropertyChanged ();
            }
        }

        public bool readOnly {
            get { return m_InputField.readOnly; }
            set {
                m_InputField.readOnly = value;
                OnPropertyChanged ();
            }
        }

        public bool multiLine {
            get { return m_InputField.multiLine; }
        }

        public char asteriskChar {
            get { return m_InputField.asteriskChar; }
            set {
                m_InputField.asteriskChar = value;
                OnPropertyChanged ();
            }
        }

        public bool wasCanceled {
            get { return m_InputField.wasCanceled; }
        }

        public int caretPosition {
            get { return m_InputField.caretPosition; }
            set {
                m_InputField.caretPosition = value;
                OnPropertyChanged ();
            }
        }

        public int selectionAnchorPosition {
            get { return m_InputField.selectionAnchorPosition; }
            set {
                m_InputField.selectionAnchorPosition = value;
                OnPropertyChanged ();
            }
        }

        public int selectionFocusPosition {
            get { return m_InputField.selectionFocusPosition; }
            set {
                m_InputField.selectionFocusPosition = value;
                OnPropertyChanged ();
            }
        }

        #endregion 

        private ICommand m_SubmitEventCommand;
        public ICommand submitEventCommand {
            get {
                return m_SubmitEventCommand;
            }
            set {
                m_SubmitEventCommand = value;
                OnPropertyChanged ();
            }
        }

        public object m_commandParameter;

        public object commandParameter {
            get { return m_commandParameter; }
            set {
                m_commandParameter = value;
            }
        }

        private IExecute m_OnValueChangedExecute;
        public IExecute onValueChangedExecute {
            get {
                return m_OnValueChangedExecute;
            }
            set {
                m_OnValueChangedExecute = value;
                OnPropertyChanged ();
            }
        }

        void OnSubmitEvent (string value) {
            OnPropertyChangedBindingApply (TextProperty);
            if (m_SubmitEventCommand != null && m_SubmitEventCommand.CanExecute (m_commandParameter))
                m_SubmitEventCommand.Execute (value);
        }

        void OnValueChanged (string value) {
            if (m_OnValueChangedExecute != null) //&& m_OnClickCommand.can_execute (m_commandParameter)
                m_OnValueChangedExecute.Execute (value);
        }
        protected void Awake () {
            m_InputField.onEndEdit.AddListener (OnSubmitEvent);
            m_InputField.onValueChanged.AddListener (OnValueChanged);
        }

        protected override void OnDestroy () {
            m_InputField.onEndEdit.RemoveListener (OnSubmitEvent);
            m_InputField.onValueChanged.RemoveListener (OnValueChanged);
            m_commandParameter = null;
            m_SubmitEventCommand = null;
            m_OnValueChangedExecute = null;
            m_InputField = null;
            base.OnDestroy ();
        }

    }
}