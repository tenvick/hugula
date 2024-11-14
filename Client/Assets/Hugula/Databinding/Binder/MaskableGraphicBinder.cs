using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{

    [XLua.LuaCallCSharp]
    public abstract class MaskableGraphicBinder<T> : UIBehaviourBinder<T> where T : UnityEngine.UI.MaskableGraphic
    {

        // public const string MaskableProperty = "maskable";
        // public const string ColorProperty = "color";

        #region  重写属性
        public bool maskable
        {
            get { return target.maskable; }
            set
            {
                target.maskable = value;
                OnPropertyChanged();
            }
        }

        public Color color
        {
            get { return target.color; }
            set
            {
                target.color = value;
                OnPropertyChanged();
            }
        }
        #endregion

    }
}