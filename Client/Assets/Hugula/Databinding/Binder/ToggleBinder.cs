using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{
    public class ToggleBinder : SelectableBinder<Toggle>
    {
        private const string IsOnProperty = "isOn";

        #region  重写属性
        public Toggle.ToggleTransition toggleTransition
        {
            get
            {
                return target.toggleTransition;
            }
            set
            {
                target.toggleTransition = value;
            }
        }

        public Graphic graphic
        {
            get
            {
                return target.graphic;
            }
            set
            {
                target.graphic = value;
            }
        }

        public bool isOn
        {
            get
            {
                return target.isOn;
            }
            set
            {
                target.isOn = value;
                OnPropertyChanged();
            }
        }
        public ToggleGroup group { get; set; }
        #endregion
        private ICommand m_Command;
        public ICommand onValueChangedCommand
        {
            get
            {
                return m_Command;
            }
            set
            {
                m_Command = value;
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

        void OnValueChanged(bool changed)
        {
            OnPropertyChangedBindingApply(IsOnProperty);
            if (m_Command != null && m_Command.CanExecute(m_commandParameter))
                m_Command.Execute(m_commandParameter);
        }

        protected override void Awake()
        {
            base.Awake();
            target.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void OnDestroy()
        {
            target.onValueChanged.RemoveListener(OnValueChanged);
            m_commandParameter = null;
            m_Command = null;
            base.OnDestroy();
        }
    }
}