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

		//表示没有参数的方法
		public bool isGetter { get; set; }

		//表示有参数的方法
		public bool isSetter { get; set; }

		public BindingExpression expression {
			get {
				return m_Expression;
			}
		}
		readonly BindingExpression m_Expression;
		readonly PropertyChangedEventHandler m_ChangeHandler;
		readonly WeakPropertyChangedProxy _listener;

		public BindingPathPart (BindingExpression expression, string path, bool isIndexer = false) {
			m_Expression = expression;
			isSelf = path == Binding.SelfPath;
			this.path = path;
			this.isIndexer = isIndexer;
			isGetter = false;
			isSetter = false;

			m_ChangeHandler = PropertyChanged;
			_listener = new WeakPropertyChangedProxy ();
		}

		public void Subscribe (INotifyPropertyChanged handler) {
			if (_listener != null && Object.Equals (handler, _listener.Source))
				return;

			Unsubscribe ();

			_listener.SubscribeTo (handler, m_ChangeHandler);
		}

		public void Unsubscribe () {
			var listener = _listener;
			if (listener != null) {
				listener.Unsubscribe ();
			}
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

			m_Expression.Apply (false);

		}

		public bool TryGetValue (object source, out object value) {
			value = source;
			if (value != null) {
				if (isSetter || isGetter)
					value = ExpressionUtility.InvokeSourceMethod (source, this.path);
				else
					value = ExpressionUtility.GetSourcePropertyValue (source, this.path);
				return true;
			}
			return false;
		}

		public override string ToString () {
			return string.Format ("BindingPathPart(path={0},isIndexer={1},isSelf={5},isSetter={2},isGetter={3},indexerName={4})", this.path, isIndexer, isSetter, isGetter, indexerName, isSelf);
		}
	}

	internal class WeakPropertyChangedProxy {
		INotifyPropertyChanged m_Source;
		readonly WeakReference<PropertyChangedEventHandler> m_Listener = new WeakReference<PropertyChangedEventHandler> (null);
		readonly PropertyChangedEventHandler m_Handler;
		internal INotifyPropertyChanged Source { get { return m_Source; } }

		public WeakPropertyChangedProxy () {
			m_Handler = new PropertyChangedEventHandler (OnPropertyChanged);
		}

		public void SubscribeTo (INotifyPropertyChanged source, PropertyChangedEventHandler listener) {
			source.PropertyChanged += m_Handler;
			var bo = source as BindableObject;

			m_Source = source;
			m_Listener.SetTarget (listener);
		}

		public void Unsubscribe () {
			if (m_Source != null) {
				m_Source.PropertyChanged -= m_Handler;
			}
			m_Source = null;
			m_Listener.SetTarget (null);
		}

		void OnPropertyChanged (object sender, string e) {
			if (m_Listener.TryGetTarget (out var handler) && handler != null)
				handler (sender, e);
			else
				Unsubscribe ();
		}
	}
}