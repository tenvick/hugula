﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Hugula.Databinding.Binder
{
    [XLua.LuaCallCSharp]
    public abstract class UIBehaviourBinder<T> : BindableObject where T : UnityEngine.Object
    {
        // [HideInInspector]
        [PopUpComponentsAttribute]
        [SerializeField]
        private T m_Target;

        public T target
        {
            get
            {
                return m_Target;
            }
            set
            {
                m_Target = value;
            }
        }

        protected override void Awake()
        {
            base.Awake();
            if (!m_Target) m_Target = GetComponent<T>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            m_Target = null;
        }
#if UNITY_EDITOR

        internal override void ClearBindRef()
        {
           m_Target = default(T);
           base.ClearBindRef();
        }
#endif
    }
}