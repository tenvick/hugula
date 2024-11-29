// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula
//
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Hugula.Utils;

namespace Hugula.Databinding
{

    public abstract class BindableObject : MonoBehaviour, INotifyPropertyChanged, IClearBingding//gc alloc 88B  +List<Binding> 40B   Binding96B*n + BindingPathPart65B*n
    {
        public const string ContextProperty = "context";
        /// <summary>
        /// 超过BindingsDicCapacity个绑定时使用字典查询
        /// </summary>
        internal const int BindingsDicCapacity = 8;

        /// <summary>
        /// 空绑定
        /// </summary>
        static readonly Binding EmptyBinding = new Binding("", null, "", BindingMode.OneWay, null);

        #region  重写属性


        public new string tag
        {
            get { return base.tag; }
            set
            {
                base.tag = value;
                // OnPropertyChanged();
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
                    if (!m_InitBindings) InitBindings(this);
                    OnBindingContextChanging();
                    SetProperty<object>(ref m_Context, value);
                    OnBindingContextChanged();
                }
            }
        }

        ///<summary>
        /// 绑定表达式
        ///<summary>
        [HideInInspector]
        [SerializeField]
        // [BindingsAttribute]
        protected List<Binding> bindings ;//= new List<Binding>(); //gc alloc 40B
        /// <summary>
        /// 缓存的上下文绑定
        /// </summary>
        protected Binding m_ContextBinding;

        protected Dictionary<string, Binding> m_BindingsDic;// = new Dictionary<string, Binding>();



        protected bool m_InitBindings = false;

        internal virtual bool forceContextChanged
        {
            get;
            set;
        }

        public bool activeSelf
        {
            get { return gameObject.activeSelf; }
            set
            {
                gameObject.SetActive(value);
            }
        }
        public new bool enabled
        {
            get { return base.enabled; }
            set
            {
                base.enabled = value;
            }
        }

        protected virtual void InitBindings(Object target)
        {
            if (m_InitBindings) return;

            m_InitBindings = true;
            m_ContextBinding = null;
            
            if(bindings == null) return;
            bool needDic = bindings.Count >= BindingsDicCapacity;

            if (m_BindingsDic == null && needDic)
                m_BindingsDic = DictionaryPool<string, Binding>.Get();

            Binding binding = null;
            for (int i = 0; i < bindings.Count; i++)
            {
                binding = bindings[i];
                binding.next = null;

                if (binding.target == null)
                    binding.target = target;

                if (needDic)
                {
                    if (m_BindingsDic.TryGetValue(binding.propertyName, out Binding existingBinding))
                    {
                        // Find the last binding in the chain
                        while (existingBinding.next != null)
                            existingBinding = existingBinding.next;

                        if (existingBinding != binding) //不能是自己
                            existingBinding.next = binding;
                    }
                    else
                    {
                        m_BindingsDic.Add(binding.propertyName, binding);
                        if (binding.propertyName == ContextProperty && binding.target == target) //如果是当前对象的上下文
                            m_ContextBinding = binding;
                    }
                }
                else
                {
                    if (binding.propertyName == ContextProperty) //如果是上下文
                    {
                        if (binding.target == target)
                        {
                            m_ContextBinding = binding;
                        }
                    }

                }
            }       

        }

        /// <summary>
        /// 获取上下文绑定
        /// </summary>
        /// <returns></returns>
        public Binding GetContextBinding()
        {
            if (!m_InitBindings) InitBindings(this);

            if (m_ContextBinding == null) m_ContextBinding = EmptyBinding; //标记已经查找过

            if (m_ContextBinding == EmptyBinding)
                return null;
            else
                return m_ContextBinding;
        }

        /// <summary>
        /// 返回的是第一个绑定表达式重复的放在next链表中
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public Binding GetBinding(string property)
        {
            if (!m_InitBindings) InitBindings(this);
            Binding firstBinding = null;
            bool needDic = bindings.Count >= BindingsDicCapacity;
            if (needDic && m_BindingsDic != null)
                m_BindingsDic.TryGetValue(property, out firstBinding);
            else
            {
                Binding curr = null;
                Binding lastBinding = null;
                for (int i = 0; i < bindings.Count; i++)
                {
                    curr = bindings[i];
                    if (curr.propertyName == property)
                    {
                        if (firstBinding == null) //fisrt
                        {
                            firstBinding = curr;
                            if (firstBinding.m_IsProcessed) break; //已经遍历过了，无需再次遍历
                            lastBinding = firstBinding;
                        }
                        else
                        {
                            lastBinding.next = curr;
                            lastBinding = curr;
                        }

                        curr.m_IsProcessed = true; // 标记已处理
                    }
                }

                //确保最后一个next为null
                if (lastBinding != null) lastBinding.next = null;
            }

            return firstBinding;
        }

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


        /// <summary>
        /// 设置继承的上下文
        /// 如果有context绑定则调用此方法
        /// </summary>
        /// <param name="value"></param>
        internal virtual void SetInheritedContext(object value)
        {
            var contextBinding = GetContextBinding();  //;
            if (contextBinding != null && contextBinding.path != Binding.SelfPath)
            {
                m_InheritedContext = value;
                contextBinding.target = this;
                contextBinding.Apply(value);
            }
            else
            {
                context = value;
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
                if (binding != m_ContextBinding ) // !ContextProperty.Equals(binding.propertyName)
                {
                    //已经绑定过context不需要再次绑定
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
            while (binding != null)
            {
                if (binding.mode == BindingMode.TwoWay || binding.mode == BindingMode.OneWayToSource)
                {
                    binding.UpdateSource();
                }
                binding = binding.next;
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
            if (m_PropertyChanged != null)
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
            if(bindings == null) bindings = new List<Binding>();
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