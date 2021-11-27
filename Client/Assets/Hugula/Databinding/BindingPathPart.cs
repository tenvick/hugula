using System;
using Hugula.Utils;

namespace Hugula.Databinding
{

    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public class BindingPathPart
    {
        public BindingPathPart nextPart { get; set; }

        public string path { get; internal set; }

        public string indexerName { get; set; }

        public bool isIndexer { get; internal set; }

        public bool isSelf { get; private set; }

        //表示方法
        public bool isMethod { get; internal set; }

        object m_Source;
        //当前节点的源对象
        public object source
        {
            get
            {
                return m_Source;
            }
        }

        INotifyPropertyChanged m_NotifyPropertyChanged;

        Binding m_Binding;
        PropertyChangedEventHandler m_ChangeHandler;

        /// <summary>
        /// for pool
        /// </summary>
        public BindingPathPart()
        {

        }

        // public BindingPathPart(Binding binding, string path, bool isIndexer = false, bool isMethod = false)
        // {
        //     this.m_Binding = binding;
        //     isSelf = path == Binding.SelfPath;
        //     this.path = path;
        //     this.isIndexer = isIndexer;
        //     this.isMethod = isMethod;

        //     m_ChangeHandler = PropertyChanged;
        // }

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

        public void SetSource(object current)
        {
            m_Source = current;
        }

        public void Subscribe(INotifyPropertyChanged source)
        {
            if (Object.Equals(source, m_NotifyPropertyChanged))
                return;

            Unsubscribe();

            source.PropertyChanged += m_ChangeHandler;
            m_NotifyPropertyChanged = source;
        }

        public void Unsubscribe()
        {

            if (m_NotifyPropertyChanged != null)
            {
                m_NotifyPropertyChanged.PropertyChanged -= m_ChangeHandler;
            }
            m_NotifyPropertyChanged = null;
            m_Source = null;
        }

        // public Type SetterType { get; set; }

        public void PropertyChanged(object sender, string propertyName)
        {
            BindingPathPart part = nextPart ?? this;

            string name = propertyName;

            if (!string.IsNullOrEmpty(name))
            {
                if (part.isIndexer)
                {
                    if (name.Contains("["))
                    {
                        if (name != string.Format("{0}[{1}]", part.indexerName, part.path))
                            return;
                    }
                    else if (name != part.indexerName)
                        return;
                }
                if (name != part.path)
                {
                    return;
                }
            }

            m_Binding?.OnSourceChanged(this);

        }

        public bool TryGetValue(bool needSubscribe, out object value)
        {
            value = source;
            if (value != null)
            {
                value = ExpressionUtility.GetSourcePropertyValue(value, this, needSubscribe);
                return true;
            }
            return false;
        }

        public void Dispose()
        {
            Unsubscribe();
            indexerName = null;
            isIndexer = false;
            isMethod = false;
            isSelf = false;
            path = null;

            m_Source = null;
            nextPart = null;
            m_Binding = null;
        }

        public void ReleaseToPool()
        {
            m_PoolBindingPathPart.Release(this);
        }

        public override string ToString()
        {
            return string.Format("BindingPathPart(path={0},isIndexer={1},isMethod={2},indexerName={2},isSelf={4},soure={5})", this.path, isIndexer, isMethod, indexerName, isSelf, source);
        }

        #region  pool

        public static BindingPathPart Get()
        {
            return m_PoolBindingPathPart.Get();
        }

        const int capacity = 2048;
        const int initial = 1024;

        private static ObjectPool<BindingPathPart> m_PoolBindingPathPart = new ObjectPool<BindingPathPart>(null, ActionOnRelease, capacity, initial);

        private static void ActionOnRelease(BindingPathPart bpPart)
        {
            bpPart.Dispose();
        }

        #endregion
    }
}