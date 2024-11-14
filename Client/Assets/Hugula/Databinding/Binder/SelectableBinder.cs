using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{

    [XLua.LuaCallCSharp]
    public abstract class SelectableBinder<T> : UIBehaviourBinder<T> where T : UnityEngine.UI.Selectable
    {

        // public const string MaskableProperty = "maskable";
        // public const string ColorProperty = "color";

        #region  重写属性
        public Navigation navigation
        {
            get { return target.navigation; }
            set
            {
                target.navigation = value;
                OnPropertyChanged();
            }
        }

        public Selectable.Transition transition
        {
            get { return target.transition; }
            set
            {
                target.transition = value;
                OnPropertyChanged();
            }
        }

        public ColorBlock colors
        {
            get { return target.colors; }
            set
            {
                target.colors = value;
                OnPropertyChanged();
            }
        }

        public SpriteState spriteState
        {
            get { return target.spriteState; }
            set
            {
                target.spriteState = value;
                OnPropertyChanged();
            }
        }

        public AnimationTriggers animationTriggers
        {
            get { return target.animationTriggers; }
            set
            {
                target.animationTriggers = value;
                OnPropertyChanged();
            }
        }

        public Graphic targetGraphic
        {
            get { return target.targetGraphic; }
            set
            {
                target.targetGraphic = value;
                OnPropertyChanged();
            }
        }

        public bool interactable
        {
            get { return target.interactable; }
            set
            {
                target.interactable = value;
                OnPropertyChanged();
            }
        }

        public Image image
        {
            get { return target.image; }
            set
            {
                target.image = value;
                OnPropertyChanged();
            }
        }

        public Animator animator
        {
            get { return target.animator; }
        }

        #endregion

    }
}