using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder {

    public class AnimatorBinder : BindableObject {
        Animator m_target;
        Animator m_Animator {
            get {
                if (m_target == null)
                    m_target = GetTarget<Animator> ();
                return m_target;
            }
            set {
                m_target = null;
            }
        }

        //要设定的参数名
        public string parameterName;

        public int IntegerValue {
            get {
                return m_Animator.GetInteger (parameterName);
            }
            set {
                m_Animator.SetInteger (parameterName, value);
            }
        }

        public float FloatValue {
            get {
                return m_Animator.GetFloat (parameterName);
            }
            set {
                m_Animator.SetFloat (parameterName, value);
            }
        }

        public bool BoolValue {
            get {
                return m_Animator.GetBool (parameterName);
            }
            set {
                m_Animator.SetBool (parameterName, value);
            }
        }

        public string SetTrigger {
            get {
                return parameterName;

            }
            set {
                if (!string.IsNullOrEmpty (value))
                    m_Animator.SetTrigger (value);
                else if (!string.IsNullOrEmpty (parameterName))
                    m_Animator.SetTrigger (parameterName);
            }
        }

        protected override void OnDestroy () {
            m_Animator = null;
            base.OnDestroy ();
        }

    }
}