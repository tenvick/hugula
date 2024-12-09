using System;
using Hugula.Utils;
using UnityEngine;

namespace Hugula.Databinding
{
    [System.SerializableAttribute]
    public struct BindingPathPartConfig
    {
        [NonSerialized]
        internal PropertyChangedEventHandlerEvent m_NotifyPropertyChanged;

        public string path;
        public BindingPathPartFlags _flags;

        public bool isSelf
        {
            get => (_flags & BindingPathPartFlags.IsSelf) != 0;
            set
            {
                if (value)
                    _flags |= BindingPathPartFlags.IsSelf;
                else
                    _flags &= ~BindingPathPartFlags.IsSelf;
            }
        }

        public bool isIndexer
        {
            get => (_flags & BindingPathPartFlags.IsIndexer) != 0;
            set
            {
                if (value)
                    _flags |= BindingPathPartFlags.IsIndexer;
                else
                    _flags &= ~BindingPathPartFlags.IsIndexer;
            }
        }

        public bool isMethod
        {
            get => (_flags & BindingPathPartFlags.IsMethod) != 0;
            set
            {
                if (value)
                    _flags |= BindingPathPartFlags.IsMethod;
                else
                    _flags &= ~BindingPathPartFlags.IsMethod;
            }
        }

        public bool isExpSetter
        {
            get => (_flags & BindingPathPartFlags.IsExpSetter) != 0;
            set
            {
                if (value)
                    _flags |= BindingPathPartFlags.IsExpSetter;
                else
                    _flags &= ~BindingPathPartFlags.IsExpSetter;
            }
        }

        public bool isExpGetter
        {
            get => (_flags & BindingPathPartFlags.IsExpGetter) != 0;
            set
            {
                if (value)
                    _flags |= BindingPathPartFlags.IsExpGetter;
                else
                    _flags &= ~BindingPathPartFlags.IsExpGetter;
            }
        }

        public bool isGlobal
        {
            get => (_flags & BindingPathPartFlags.IsGlobal) != 0;
            set
            {
                if (value)
                    _flags |= BindingPathPartFlags.IsGlobal;
                else
                    _flags &= ~BindingPathPartFlags.IsGlobal;
            }
        }

        /// <summary>
        /// 是否是表达式
        /// </summary>
        public bool isExpression
        {
            get => (_flags & BindingPathPartFlags.IsExpSetter) != 0 || (_flags & BindingPathPartFlags.IsExpGetter) != 0;
        }

#if UNITY_EDITOR
        public override string ToString()
        {
            return string.Format("BindingPathPartConfig(path={0},isSelf={1},isIndexer={2},isMethod={3},isExpSetter={4},isExpGetter={5},) m_NotifyPropertyChanged={6}", this.path, isSelf, isIndexer, isMethod, isExpSetter, isExpGetter, m_NotifyPropertyChanged);
        }
#endif

        public void Dispose()
        {
            path = null;
            m_NotifyPropertyChanged = null;
            isIndexer = false;
            isExpSetter = false;
            isSelf = false;
        }
    }

    /***
    public class BindingPathPart  //65B
    {


        public BindingPathPart nextPart { get; set; }

        public string path;//{ get; internal set; }

        private BindingPathPartFlags _flags = BindingPathPartFlags.None;


        // public string indexerName { get; set; } //indexerName可以删除

        // public bool isIndexer { get; internal set; }

        // public bool isSelf { get; private set; }

        // //表示方法
        // public bool isMethod { get; internal set; }
        public bool isIndexer
        {
            get => (_flags & BindingPathPartFlags.IsIndexer) != 0;
            internal set
            {
                if (value)
                    _flags |= BindingPathPartFlags.IsIndexer;
                else
                    _flags &= ~BindingPathPartFlags.IsIndexer;
            }
        }

        public bool isSelf
        {
            get => (_flags & BindingPathPartFlags.IsSelf) != 0;
            private set
            {
                if (value)
                    _flags |= BindingPathPartFlags.IsSelf;
                else
                    _flags &= ~BindingPathPartFlags.IsSelf;
            }
        }

        public bool isMethod
        {
            get => (_flags & BindingPathPartFlags.IsMethod) != 0;
            internal set
            {
                if (value)
                    _flags |= BindingPathPartFlags.IsMethod;
                else
                    _flags &= ~BindingPathPartFlags.IsMethod;
            }
        }

        // WeakReference<object> m_SourceWeak = new WeakReference<object>(null);

        //当前节点source对象缓存结果加快访问
        public object source;
        // {
        //     get
        //     {
        //         if (m_SourceWeak.TryGetTarget(out var target))
        //             return target;
        //         return null;
        //     }
        //     set
        //     {
        //         m_SourceWeak.SetTarget(value);
        //     }
        // }

        //INotifyPropertyChanged or PropertyChangedEventHandlerEvent
        // WeakReference<object> m_NotifyPropertyChangedWeak = new WeakReference<object>(null);

        //监听的事件
        PropertyChangedEventHandlerEvent m_NotifyPropertyChanged;
        // {
        //     get
        //     {
        //         if (m_NotifyPropertyChangedWeak.TryGetTarget(out var target))
        //             return target;
        //         return null;
        //     }
        //     set
        //     {
        //         m_NotifyPropertyChangedWeak.SetTarget(value);
        //     }
        // }

        internal Binding m_Binding;

        // readonly PropertyChangedEventHandler m_ChangeHandler;

        ~BindingPathPart()
        {

        }

        /// <summary>
        /// for pool
        /// </summary>
        public BindingPathPart()
        {
            // m_ChangeHandler = PropertyChanged;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize(Binding binding, string path, bool isIndexer = false, bool isMethod = false)
        {
            this.m_Binding = binding;
            isSelf = path == Binding.SelfPath;
            this.path = path;
            this.isIndexer = isIndexer;
            this.isMethod = isMethod;

            // m_ChangeHandler = PropertyChanged;
        }

        public void Initialize(Binding binding, BindingPathPartConfig config)
        {
            this.m_Binding = binding;
            this.isSelf = config.isSelf;
            this.path = config.path;
            this.isIndexer = config.isIndexer;
            this.isMethod = config.isMethod;
        }


        /// <summary>
        /// 订阅 hander必须是 PropertyChangedEventHandlerEvent 
        /// </summary>
        /// <param name="handler"></param>
        public void Subscribe(PropertyChangedEventHandlerEvent handler)
        {
            if (ReferenceEquals(m_NotifyPropertyChanged, handler))
                return;

            Unsubscribe(false);

            BindingPathPart part = this;
            var propertName = part.path;
            handler.Add(PropertyChanged, propertName);
            m_NotifyPropertyChanged = handler;
        }


        public void Unsubscribe(bool sourceRelease = true)
        {
            if (m_NotifyPropertyChanged == null) return;

            BindingPathPart part = this;
            var propertName = part.path;
            m_NotifyPropertyChanged.Remove(PropertyChanged, propertName);
            m_NotifyPropertyChanged = null;//

            if (sourceRelease)
                source = null;
        }

        public void PropertyChanged(object sender, string propertyName)
        {
            if (m_Binding == null)
                return;

            BindingPathPart part = this;
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (propertyName != part.path)
                {
                    return;
                }
            }
            // m_Binding.OnSourceChanged(this);

        }

        public bool TryGetValue(object source, out object value)
        {
            value = source;
            if (value != null)
            {
                if (source is XLua.LuaTable lua)
                {
                    lua.TryGet<string, object>(path, out value);
                }
                else
                {
                    value = ExpressionUtility.GetSourceInvoke(value, this);
                }

                return true;
            }
            return false;
        }

        public void Dispose()
        {
            Unsubscribe();
            // indexerName = null;
            isIndexer = false;
            isMethod = false;
            isSelf = false;
            path = null;

            source = null;
            nextPart = null;
            m_Binding = null;
            m_NotifyPropertyChanged = null;

        }

        public void ReleaseToPool()
        {
            m_PoolBindingPathPart.Release(this);
        }

        public override string ToString()
        {
            return string.Format("BindingPathPart(path={0},isIndexer={1},isMethod={2},isSelf={3},source={4})", this.path, isIndexer, isMethod, isSelf, source);
        }

        #region  pool

        public static BindingPathPart Get()
        {
            return m_PoolBindingPathPart.Get();
        }

        const int capacity = 4096;
        const int initial = 2048;

        private static ObjectPool<BindingPathPart> m_PoolBindingPathPart = new ObjectPool<BindingPathPart>(null, ActionOnRelease, capacity, initial);

        private static void ActionOnRelease(BindingPathPart bpPart)
        {
            bpPart.Dispose();
        }

        #endregion
    }

        ***/
    [Flags]
    public enum BindingPathPartFlags : byte
    {
        None = 0,
        IsSelf = 1 << 0,
        IsIndexer = 1 << 1,
        IsMethod = 1 << 2,
        IsExpSetter = 1 << 3,

        IsExpGetter = 1 << 4,
        /// <summary>
        /// 表达式上下文是否全局
        /// </summary>
        IsGlobal = 1 << 5,

    }

}