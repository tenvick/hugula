using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class ButtonBinder : SelectableBinder {

        public const string OnClickCommandProperty = "onClickCommand";

        Button m_target;
        Button m_Button {
            get {
                if (m_target == null)
                    m_target = GetTarget<Button> ();
                return m_target;
            }
            set {
                m_target = null;
            }
        }

        #region  重写属性
        #endregion
        private ICommand m_OnClickCommand;
        public ICommand onClickCommand {
            get {
                return m_OnClickCommand;
            }
            set {
                m_OnClickCommand = value;
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

        void OnClick () {
            if (m_OnClickCommand != null && m_OnClickCommand.CanExecute (m_commandParameter))
                m_OnClickCommand.Execute (m_commandParameter);
        }

        protected void Awake () {
            m_Button.onClick.AddListener (OnClick);
        }

        protected override void OnDestroy () {
            m_Button.onClick.RemoveListener (OnClick);
            m_commandParameter = null;
            m_OnClickCommand = null;
            m_Button = null;
            base.OnDestroy ();
        }

    }
}