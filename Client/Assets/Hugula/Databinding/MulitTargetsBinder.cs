using System.Collections.Generic;
using UnityEngine;
using Hugula.Utils;

namespace Hugula.Databinding
{
    /// <summary>
    /// 多目标对象绑定
    /// 每个binder都单独序列化自己的Binding.target对象。
    /// </summary>
    public class MulitTargetsBinder : BindableObject
    {
        protected override void InitBindings()
        {
            if (m_BindingsDic == null)
                m_BindingsDic = DictionaryPool<string, Binding>.Get();
            Binding binding = null;
            for (int i = 0; i < bindings.Count; i++)
            {
                binding = bindings[i];
                m_BindingsDic.Add(binding.propertyName, binding);
            }
        }
    }

}