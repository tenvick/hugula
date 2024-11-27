using System.Collections.Generic;
using UnityEngine;
using Hugula.Utils;

namespace Hugula.Databinding
{
    /// <summary>
    /// 共享上下文绑定
    /// 每个binder都单独序列化自己的Binding.target对象并共享上下文。
    /// </summary>
    public sealed class SharedContextBinder : BindableObject, ISharedContext
    {
        /// <summary>
        /// 当前gameobject.selfActive为true的时候才绑定children
        /// </summary>
        [Tooltip("当前gameobject.selfActive为true的时候才绑定children")]
        [SerializeField] bool m_BindingOnEnable = true;
        // private bool m_CheckEnable = false;
        private bool m_NeedDelayBinding = false;

        // protected override void Awake()
        // {
        //     base.Awake();
        //     m_CheckEnable = true;
        // }


        void OnEnable()
        {
            if (m_NeedDelayBinding)
            {
                m_NeedDelayBinding = false;
                OnBindingContextChanged();
            }
        }

        protected override void OnBindingContextChanged()
        {
            if (!m_BindingOnEnable || gameObject.activeInHierarchy)
            {
                base.OnBindingContextChanged();
            }
            else
                m_NeedDelayBinding = true;
        }
    }

}