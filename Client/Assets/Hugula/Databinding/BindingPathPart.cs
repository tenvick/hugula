using System;
using XLua;

namespace Hugula.Databinding {

    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public class BindingPathPart {
        public BindingPathPart nextPart { get; set; }

        public string path { get; internal set; }

        public string indexerName { get; set; }

        public bool isIndexer { get; internal set; }

        public bool isSelf { get; }

        //表示方法
        public bool isMethod { get; set; }

        // private WeakReference<object> m_Source = new WeakReference<object> (null);
        object m_Source;
        //当前节点的源对象
        public object source {
            get {
                // object real = null;
                // m_Source.TryGetTarget (out real);
                return m_Source;
            }
        }

        INotifyPropertyChanged m_NotifyPropertyChanged;

        readonly Binding m_Binding;
        readonly PropertyChangedEventHandler m_ChangeHandler;

        public BindingPathPart (Binding binding, string path, bool isIndexer = false) {
            this.m_Binding = binding;
            isSelf = path == Binding.SelfPath;
            this.path = path;
            this.isIndexer = isIndexer;
            isMethod = false;

            m_ChangeHandler = PropertyChanged;
        }

        public void SetSource (object current) {
            m_Source = current;
            // if (m_Source.TryGetTarget (out var source) && Object.Equals (source, current))
            //     return;

            // m_Source.SetTarget (current);
        }

        public void Subscribe (INotifyPropertyChanged source) {
            if (Object.Equals (source, m_NotifyPropertyChanged))
                return;

            Unsubscribe ();

            source.PropertyChanged += m_ChangeHandler;
            m_NotifyPropertyChanged = source;
        }

        public void Unsubscribe () {

            if (m_NotifyPropertyChanged != null) {
                m_NotifyPropertyChanged.PropertyChanged -= m_ChangeHandler;
            }
            m_NotifyPropertyChanged = null;
        }

        public Type SetterType { get; set; }

        public void PropertyChanged (object sender, string propertyName) {
            BindingPathPart part = nextPart ?? this;

            string name = propertyName;

            if (!string.IsNullOrEmpty (name)) {
                if (part.isIndexer) {
                    if (name.Contains ("[")) {
                        if (name != string.Format ("{0}[{1}]", part.indexerName, part.path))
                            return;
                    } else if (name != part.indexerName)
                        return;
                }
                if (name != part.path) {
                    return;
                }
            }

            // m_Binding.UpdateTarget ();
            m_Binding.OnSourceChanged (this);

        }

        public bool TryGetValue (bool needSubscribe, out object value) {
            value = source;
            if (value != null) {
                value = ExpressionUtility.GetSourcePropertyValue (value, this, needSubscribe);
                return true;
            }
            return false;
        }

        public override string ToString () {
            return string.Format ("BindingPathPart(path={0},isIndexer={1},isMethod={2},indexerName={2},isSelf={4},soure={5})", this.path, isIndexer, isMethod, indexerName, isSelf, source);
        }
    }
}