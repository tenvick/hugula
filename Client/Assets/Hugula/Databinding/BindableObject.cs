// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula
//
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Hugula.Utils;

namespace Hugula.Databinding
{

    public abstract class BindableObject : MonoBehaviour, INotifyPropertyChanged, IClearBingding
    {
        public const string ContextProperty = "context";

        #region  重写属性
        public bool activeSelf
        {
            get { return gameObject.activeSelf; }
            set
            {
                gameObject.SetActive(value);
                OnPropertyChanged();
            }
        }
        public new bool enabled
        {
            get { return base.enabled; }
            set
            {
                base.enabled = value;
                OnPropertyChanged();
            }
        }

        public new string tag
        {
            get { return base.tag; }
            set
            {
                base.tag = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region  databinding
        protected object m_Context;
        protected object m_InheritedContext;
        private PropertyChangedEventHandlerEvent m_PropertyChanged;// = new PropertyChangedEventHandlerEvent();

        public PropertyChangedEventHandlerEvent PropertyChanged
        {
            get
            {
                if (m_PropertyChanged == null)
                    m_PropertyChanged = PropertyChangedEventHandlerEvent.Get();
                return m_PropertyChanged;
            }
        }

        internal virtual bool forceContextChanged
        {
            get;
            set;
        }

        /// <summary>
        /// 绑定上下文
        /// </summary>
        public object context
        {
            get { return m_InheritedContext ?? m_Context; }
            set
            {
                if (!Object.Equals(m_Context, value) || forceContextChanged)
                {
                    forceContextChanged = false;
                    m_InheritedContext = null;
                    if (m_BindingsDic == null) InitBindings();
                    OnBindingContextChanging();
                    SetProperty<object>(ref m_Context, value);
                    OnBindingContextChanged();
                }
            }
        }

        /// <summary>
        /// 继承的绑定上下文
        /// </summary>
        public object inheritedContext
        {
            get { return m_InheritedContext; }
            set
            {
                if (!Object.Equals(m_InheritedContext, value))
                {
                    SetProperty<object>(ref m_InheritedContext, value);
                    // OnInheritedContextChanged();
                }
            }
        }


        ///<summary>
        /// 绑定表达式
        ///<summary>
        [HideInInspector]
        [SerializeField]
        // [BindingsAttribute]
        protected List<Binding> bindings = new List<Binding>();

        // protected bool m_InitBindings = false;
        protected Dictionary<string, Binding> m_BindingsDic;// = new Dictionary<string, Binding>();
        protected virtual void InitBindings()
        {
            // m_InitBindings = true;
            Binding binding = null;
            if (m_BindingsDic == null)
                m_BindingsDic = DictionaryPool<string, Binding>.Get();
            for (int i = 0; i < bindings.Count; i++)
            {
                binding = bindings[i];
                binding.target = this;
                m_BindingsDic.Add(binding.propertyName, binding);
            }
        }

        public Binding GetBinding(string property)
        {
            if (m_BindingsDic == null) InitBindings();
            Binding binding = null;
            m_BindingsDic.TryGetValue(property, out binding);
            return binding;
        }


        public void SetBinding(string sourcePath, UnityEngine.Object target, string property, BindingMode mode, string format, string converter)
        {

            if (target == null) target = this;
            Binding binding = null;
            if (m_BindingsDic == null) InitBindings();
            if (m_BindingsDic.TryGetValue(property, out binding))
            {
                binding.Dispose();
                m_BindingsDic.Remove(property);
                bindings.Remove(binding);
                Debug.LogWarningFormat(" target({0}).{1} has already bound.", target, property);
            }

            binding = new Binding(sourcePath, target, property, mode, format, converter);
            bindings.Add(binding);
            m_BindingsDic.Add(property, binding);

        }

        // public void SetBinding(string sourcePath, object target, string property, BindingMode mode)
        // {
        //     SetBinding(sourcePath, target, property, mode, string.Empty, string.Empty);
        // }

        // public void SetBinding(string sourcePath, string property, BindingMode mode)
        // {
        //     SetBinding(sourcePath, this, property, mode, string.Empty, string.Empty);
        // }

        ///<summary>
        /// 销毁之前清理所有绑定表达式
        ///<summary>
        public virtual void ClearBinding()
        {
            foreach (var binding in bindings)
                binding?.Dispose();

            bindings?.Clear();

        }
        ///<summary>
        /// 解绑源绑定，不销毁对象
        ///<summary>
        public virtual void Unapply()
        {
            foreach (var binding in bindings)
                binding?.Unapply();
        }

        ///<summary>
        /// 清理m_Context和m_InheritedContext引用对象
        ///<summary>
        internal void ClearContextRef()
        {
            m_Context = null;
            m_InheritedContext = null;
        }

        // protected virtual void OnInheritedContextChanged()
        // {

        // }

        internal virtual void SetInheritedContext(object value, bool force)
        {
            if (m_BindingsDic == null) InitBindings();
            if (!Object.Equals(m_InheritedContext, value) || force)
            {
                m_InheritedContext = value;
                OnBindingContextChanging();
                var contextBinding = GetBinding(ContextProperty);
                if (contextBinding != null && contextBinding.path != Binding.SelfPath)
                {
                    contextBinding.target = this;
                    // contextBinding.ApplyContext(context);
                    // #if HUGULA_ASYNC_APPLY
                    //                     Executor.Execute(contextBinding.Apply);
                    // #else
                    contextBinding.Apply(context);
                    // #endif
                }
                else
                    OnBindingContextChanged();
            }
        }
        protected virtual void OnBindingContextChanging()
        {

        }

        protected virtual void OnBindingContextChanged()
        {
            var bindingContext = context;
            for (int i = 0; i < bindings.Count; i++)
            {
                var binding = bindings[i];
                if (!ContextProperty.Equals(binding.propertyName))
                { //context需要触发自己，由inherited触发
                    // binding.ApplyContext(context);
                    binding.Apply(bindingContext);
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, propertyName);
        }

        //变化的同时更新目标源
        protected virtual void OnPropertyChangedBindingApply([CallerMemberName] string propertyName = null)
        {
            Binding binding = GetBinding(propertyName);
            if (binding != null && (binding.mode == BindingMode.TwoWay || binding.mode == BindingMode.OneWayToSource))
            {
                binding.UpdateSource();
            }
            PropertyChanged?.Invoke(this, propertyName);
        }

        protected bool SetProperty<T1>(ref T1 storage, T1 value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        #endregion

        protected virtual void Awake()
        {
        }

        protected virtual void OnDestroy()
        {
            ClearBinding();
            if(m_PropertyChanged!=null)
                PropertyChangedEventHandlerEvent.Release(m_PropertyChanged);
            if (m_BindingsDic != null)
                DictionaryPool<string, Binding>.Release(m_BindingsDic);
            m_Context = null;
            m_InheritedContext = null;
            m_PropertyChanged = null;
        }

#if UNITY_EDITOR
        [XLua.DoNotGen]
        public void AddBinding(Binding expression)
        {
            bindings.Add(expression);
        }

        [XLua.DoNotGen]
        public List<Binding> GetBindings()
        {
            return bindings;
        }

        //清理序列化的引用
        [XLua.DoNotGen]
        internal virtual void ClearBindRef()
        {
            foreach (var b in bindings)
            {
                b.source = null;
            }
        }
#endif
    }


}