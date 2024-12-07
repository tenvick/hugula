using System;
using Hugula.Utils;

namespace Hugula.Databinding
{

    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public class BindingPathPart  //65B
    {
        public BindingPathPart nextPart { get; set; }

        public string path { get; internal set; }

        // public string indexerName { get; set; } //indexerName可以删除

        public bool isIndexer { get; internal set; }

        public bool isSelf { get; private set; }

        //表示方法
        public bool isMethod { get; internal set; }

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

        PropertyChangedEventHandler m_ChangeHandler;

        ~BindingPathPart()
        {

        }

        /// <summary>
        /// for pool
        /// </summary>
        public BindingPathPart()
        {
            m_ChangeHandler = PropertyChanged;
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

            m_ChangeHandler = PropertyChanged;
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
            handler.Add(m_ChangeHandler,propertName);    
            m_NotifyPropertyChanged = handler;
        }


        public void Unsubscribe(bool sourceRelease = true)
        {
            if (m_NotifyPropertyChanged == null) return;

            BindingPathPart part = this;
            var propertName = part.path;
            m_NotifyPropertyChanged.Remove(m_ChangeHandler,propertName);
            m_NotifyPropertyChanged = null;//

            if (sourceRelease)
                source = null;
        }

        public void PropertyChanged(object sender, string propertyName)
        {
            if (m_Binding == null)
                return;

            // if (!m_Binding.NeedsGetter()) return;

            BindingPathPart part = this;
            if (!string.IsNullOrEmpty(propertyName))
            {
                if (propertyName != part.path)
                {
                    return;
                }
                // if (part.isIndexer)
                // {
                //     if (name.Contains("["))
                //     {
                //         if (name != string.Format("{0}[{1}]", part.indexerName, part.path))
                //             return;
                //     }
                //     else if (name != part.indexerName)
                //         return;
                // }
                // if (name != part.path)
                // {
                //     return;
                // }
            }
            m_Binding.OnSourceChanged(this);

        }

        public bool TryGetValue(object source, out object value)
        {
            value = source;
            if (value != null)
            {
                if(source is XLua.LuaTable lua)
                {
                    lua.TryGet<string,object>(path,out value);
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
            return string.Format("BindingPathPart(path={0},isIndexer={1},isMethod={2},isSelf={3},source={4})", this.path, isIndexer, isMethod, isSelf,source);
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

}