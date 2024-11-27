using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hugula.Utils;

namespace Hugula.Databinding.Binder
{
    ///<summary>
    ///直接绑定UnityEngine.Object对象
    ///</summary>
    [RequireComponent(typeof(UnityEngine.Component))]
    [XLua.LuaCallCSharp]
    public sealed class CustomBinder : UIBehaviourBinder<Component>
    {

        protected override void InitBindings(Object arg)
        {
            base.InitBindings(target);           
        }

    }

}