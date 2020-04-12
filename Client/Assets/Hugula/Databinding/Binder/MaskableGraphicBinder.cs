using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class MaskableGraphicBinder : UIBehaviourBinder {

        public const string MaskableProperty = "maskable";
        public const string ColorProperty = "color";
        private MaskableGraphic m_target_mask;
        protected MaskableGraphic m_Maskable {
            get {
                if (m_target_mask == null)
                    m_target_mask = GetTarget<MaskableGraphic> ();

                return m_target_mask;

            }
            set {
                m_target_mask = value;
            }
        }
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

        protected override void OnDestroy () {
            m_Maskable = null;
            base.OnDestroy ();
        }
    }
}