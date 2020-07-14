using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{
    [RequireComponent(typeof(Slider))]
    public class SliderBinder : SelectableBinder<Slider>
    {

        public const string OnClickExcuteProperty = "onValueChangedExcute";
        public const string ValueProperty = "value";


        #region  重写属性
        public Slider.Direction direction
        {
            get { return target.direction; }
            set
            {
                if (target.direction != value)
                {
                    target.direction = value;
                    OnPropertyChanged();
                }
            }
        }

        public float minValue
        {
            get { return target.minValue; }
            set
            {
                target.minValue = value;
                OnPropertyChanged();
            }
        }

        public float maxValue
        {
            get { return target.maxValue; }
            set
            {
                target.maxValue = value;
                OnPropertyChanged();
            }
        }

        public bool wholeNumbers
        {
            get { return target.wholeNumbers; }
            set
            {
                target.wholeNumbers = value;
                OnPropertyChanged();
            }
        }

        public float value
        {
            get { return target.value; }
            set
            {
                target.value = value;
                OnPropertyChanged();
            }
        }

        public float normalizedValue
        {
            get { return target.normalizedValue; }
            set
            {
                target.normalizedValue = value;
                OnPropertyChanged();
            }
        }

        #endregion 
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
                OnPropertyChanged();
            }
        }

        void OnValueChanged(float value)
        {
            OnPropertyChangedBindingApply(ValueProperty);
            if (m_OnValueChangedExecute != null) //&& m_OnClickCommand.can_execute (m_commandParameter)
                m_OnValueChangedExecute.Execute(value);
        }

        protected override void Awake()
        {
            base.Awake();
            target.onValueChanged.AddListener(OnValueChanged);
        }

        protected override void OnDestroy()
        {
            target.onValueChanged.RemoveListener(OnValueChanged);
            m_OnValueChangedExecute = null;
            base.OnDestroy();
        }

    }
}