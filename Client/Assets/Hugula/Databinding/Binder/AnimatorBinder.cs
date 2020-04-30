using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{

    [RequireComponent(typeof(Animator))]
    public class AnimatorBinder : UIBehaviourBinder<Animator>
    {

        //要设定的参数名
        public string parameterName;

        public int IntegerValue
        {
            get
            {
                return target.GetInteger(parameterName);
            }
            set
            {
                target.SetInteger(parameterName, value);
            }
        }

        public float FloatValue
        {
            get
            {
                return target.GetFloat(parameterName);
            }
            set
            {
                target.SetFloat(parameterName, value);
            }
        }

        public bool BoolValue
        {
            get
            {
                return target.GetBool(parameterName);
            }
            set
            {
                target.SetBool(parameterName, value);
            }
        }

        public string SetTrigger
        {
            get
            {
                return parameterName;

            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    target.SetTrigger(value);
                else if (!string.IsNullOrEmpty(parameterName))
                    target.SetTrigger(parameterName);
            }
        }


    }
}