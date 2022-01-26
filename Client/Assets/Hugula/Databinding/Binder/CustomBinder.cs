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

        protected override void InitBindings()
        {
            m_InitBindings = true;
            if (m_BindingsDic == null)
                m_BindingsDic = DictionaryPool<string, Binding>.Get();
            Binding binding = null;
            for (int i = 0; i < bindings.Count; i++)
            {
                binding = bindings[i];
                binding.target = target;
                m_BindingsDic.Add(binding.propertyName, binding);
            }
        }

    }

}