using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class MaskableGraphicBinder : UIBehaviourBinder {

        public const string MaskableProperty = "maskable";
        public const string ColorProperty = "color";
        protected MaskableGraphic m_Maskable;
        #region  重写属性
        public bool maskable {
            get { return m_Maskable.maskable; }
            set {
                m_Maskable.maskable = value;
                OnPropertyChanged ();
            }
        }

        public Color color {
            get { return m_Maskable.color; }
            set {
                m_Maskable.color = value;
                OnPropertyChanged ();
            }
        }
        #endregion

        protected virtual void Awake () {
            m_Maskable = GetTarget<MaskableGraphic> ();
        }

    }
}