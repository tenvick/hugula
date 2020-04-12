using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class SliderBinder : SelectableBinder {

        public const string OnClickExcuteProperty = "onValueChangedExcute";
        public const string ValueProperty = "value";

        Slider m_target_slider;
        Slider m_Slider {
            get {
                if (m_target_slider == null)
                    m_target_slider = GetTarget<Slider> ();
                return m_target_slider;
            }
            set {
                m_target_slider = null;
            }
        }

        #region  重写属性
        public Slider.Direction direction {
            get { return m_Slider.direction; }
            set {
                if (m_Slider.direction != value) {
                    m_Slider.direction = value;
                    OnPropertyChanged ();
                }
            }
        }

        public float minValue {
            get { return m_Slider.minValue; }
            set {
                m_Slider.minValue = value;
                OnPropertyChanged ();
            }
        }

        public float maxValue {
            get { return m_Slider.maxValue; }
            set {
                m_Slider.maxValue = value;
                OnPropertyChanged ();
            }
        }

        public bool wholeNumbers {
            get { return m_Slider.wholeNumbers; }
            set {
                m_Slider.wholeNumbers = value;
                OnPropertyChanged ();
            }
        }

        public float value {
            get { return m_Slider.value; }
            set {
                m_Slider.value = value;
                OnPropertyChanged ();
            }
        }

        public float normalizedValue {
            get { return m_Slider.normalizedValue; }
            set {
                m_Slider.normalizedValue = value;
                OnPropertyChanged ();
            }
        }

        #endregion 
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

        void OnValueChanged (float value) {
            OnPropertyChangedBindingApply (ValueProperty);
            if (m_OnValueChangedExecute != null) //&& m_OnClickCommand.can_execute (m_commandParameter)
                m_OnValueChangedExecute.Execute (value);
        }

        void Awake () {
            m_Slider.onValueChanged.AddListener (OnValueChanged);
        }

        void OnDestory () {
            m_Slider.onValueChanged.RemoveListener (OnValueChanged);
            m_OnValueChangedExecute = null;
            m_Slider = null;
            base.OnDestroy ();
        }

    }
}