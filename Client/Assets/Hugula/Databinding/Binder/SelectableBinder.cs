using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class SelectableBinder : UIBehaviourBinder {

        // public const string MaskableProperty = "maskable";
        // public const string ColorProperty = "color";
        Selectable m_target_selectable;
        Selectable m_Selectable {
            get {
                if (m_target_selectable == null)
                    m_target_selectable = GetTarget<Selectable> ();
                return m_target_selectable;
            }
            set {
                m_target_selectable = null;
            }
        }

        #region  重写属性
        public Navigation navigation {
            get { return m_Selectable.navigation; }
            set {
                m_Selectable.navigation = value;
                OnPropertyChanged ();
            }
        }

        public Selectable.Transition transition {
            get { return m_Selectable.transition; }
            set {
                m_Selectable.transition = value;
                OnPropertyChanged ();
            }
        }

        public ColorBlock colors {
            get { return m_Selectable.colors; }
            set {
                m_Selectable.colors = value;
                OnPropertyChanged ();
            }
        }

        public SpriteState spriteState {
            get { return m_Selectable.spriteState; }
            set {
                m_Selectable.spriteState = value;
                OnPropertyChanged ();
            }
        }

        public AnimationTriggers animationTriggers {
            get { return m_Selectable.animationTriggers; }
            set {
                m_Selectable.animationTriggers = value;
                OnPropertyChanged ();
            }
        }

        public Graphic targetGraphic {
            get { return m_Selectable.targetGraphic; }
            set {
                m_Selectable.targetGraphic = value;
                OnPropertyChanged ();
            }
        }

        public bool interactable {
            get { return m_Selectable.interactable; }
            set {
                m_Selectable.interactable = value;
                OnPropertyChanged ();
            }
        }

        public Image image {
            get { return m_Selectable.image; }
            set {
                m_Selectable.image = value;
                OnPropertyChanged ();
            }
        }

        public Animator animator {
            get { return m_Selectable.animator; }
        }

        #endregion

        protected override void OnDestroy () {
            m_Selectable = null;
            base.OnDestroy ();
        }

    }
}