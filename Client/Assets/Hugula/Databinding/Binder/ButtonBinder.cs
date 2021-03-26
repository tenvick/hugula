using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{

    [RequireComponent(typeof(Button))]
    public class ButtonBinder : SelectableBinder<Button>
    {

        public const string OnClickCommandProperty = "onClickCommand";

        #region  重写属性
        #endregion
        private ICommand m_OnClickCommand;
        public ICommand onClickCommand
        {
            get
            {
                return m_OnClickCommand;
            }
            set
            {
                m_OnClickCommand = value;
                OnPropertyChanged();
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

        protected void OnClick()
        {
            if (m_OnClickCommand != null && m_OnClickCommand.CanExecute(m_commandParameter))
                m_OnClickCommand.Execute(m_commandParameter);
        }

        protected override void Awake()
        {
            base.Awake();
            target.onClick.AddListener(OnClick);
        }

        protected override void OnDestroy()
        {
            target.onClick.RemoveListener(OnClick);
            m_commandParameter = null;
            m_OnClickCommand = null;
            base.OnDestroy();
        }

    }
}