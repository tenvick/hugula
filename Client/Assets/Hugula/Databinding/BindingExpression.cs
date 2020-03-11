using System;
using System.Collections.Generic;

namespace Hugula.Databinding {

	[XLua.LuaCallCSharp]
	[XLua.CSharpCallLua]
	public class BindingExpression : IBindingExpression {
		static readonly char[] ExpressionSplit = new [] { '.' };

		readonly List<BindingPathPart> _parts = new List<BindingPathPart> ();

		public string source {
			get {
				return m_Binding?.source;
			}
		}

		public string format {
			get {
				return m_Binding?.format;
			}
		}

		public object converter {
			get;
			private set;
		}

		public string targetProperty {
			get {
				return m_TargetProperty;
			}
		}

		public BindingMode mode {
			get {
				return m_Binding.mode;
			}
		}

		#region 
		Binding m_Binding;
		string m_Path;
		string m_TargetProperty;
		WeakReference<object> m_WeakSource;
		WeakReference<BindableObject> m_WeakTarget;
		#endregion
		public BindingExpression (Binding info, BindableObject bindable) {
			m_Binding = info;
			m_Path = info.path;
			m_TargetProperty = info.propertyName;
			ApplySource (bindable);
			ParsePath ();

		}

		public BindingExpression (Binding info) {
			m_Binding = info;
			m_Path = info.path;
			m_TargetProperty = info.propertyName;
			ParsePath ();
		}

		public void Apply (bool fromTarget = true) {
			if (m_WeakSource == null || m_WeakTarget == null)
				return;

			if (!m_WeakTarget.TryGetTarget (out BindableObject target)) {
				Unapply ();
				return;
			}

			if (m_WeakSource.TryGetTarget (out var source) && m_TargetProperty != null)
				ApplyActual (source, target, m_TargetProperty, fromTarget);
		}

		/// <summary>
		///  更新目标或者源的值
		/// </summary>
		public void Apply (object sourceObject, BindableObject target, string property, bool fromBindingContextChanged = false) {
			m_TargetProperty = property;

			if (m_WeakTarget != null && m_WeakTarget.TryGetTarget (out BindableObject prevTarget) && !Object.Equals (prevTarget, target))
				throw new InvalidOperationException (string.Format ("Binding instances can not be reused. prev={0},target={1}", prevTarget, target));

			if (m_WeakSource != null && m_WeakSource.TryGetTarget (out var previousSource) && !Object.Equals (previousSource, sourceObject))
				throw new InvalidOperationException (string.Format ("Binding instances can not be reused. prev={0},source={1}", previousSource, sourceObject));

			m_WeakSource = new WeakReference<object> (sourceObject);
			m_WeakTarget = new WeakReference<BindableObject> (target);

			ApplyActual (sourceObject, target, property);
		}

		public void Unapply (bool fromBindingContextChanged = false) {
			if (m_WeakSource != null && m_WeakSource.TryGetTarget (out var sourceObject)) {
				for (var i = 0; i < _parts.Count - 1; i++) {
					BindingPathPart part = _parts[i];

					if (!part.isSelf)
						part.TryGetValue (sourceObject, out sourceObject);

					part.Unsubscribe ();
				}
			}

			m_WeakSource = null;
			m_WeakTarget = null;
		}

		#region  protected 
		void ParsePath () {
			string p = m_Path.Trim ();

			var last = new BindingPathPart (this, ".");
			_parts.Add (last);

			if (p[0] == '.') {
				if (p.Length == 1)
					return;

				p = p.Substring (1);
			}

			string[] pathParts = p.Split (ExpressionSplit);
			for (var i = 0; i < pathParts.Length; i++) {
				string part = pathParts[i].Trim ();

				if (part == string.Empty)
					throw new FormatException ("Path contains an empty part:" + this.targetProperty);

				BindingPathPart indexer = null;
				//索引解析
				int lbIndex = part.IndexOf ('[');
				if (lbIndex != -1) {
					int rbIndex = part.LastIndexOf (']');
					if (rbIndex == -1)
						throw new FormatException ("Indexer did not contain closing [");

					int argLength = rbIndex - lbIndex - 1;
					if (argLength == 0)
						throw new FormatException ("Indexer did not contain arguments");

					string argString = part.Substring (lbIndex + 1, argLength);
					indexer = new BindingPathPart (this, argString, true);
					part = part.Substring (0, lbIndex);
					part = part.Trim ();
					indexer.indexerName = part;
				}

				//方法解析
				lbIndex = part.IndexOf ('(');
				if (lbIndex != -1) {
					int rbIndex = part.LastIndexOf (')');
					if (rbIndex == -1)
						throw new FormatException ("Method did not contain closing (");

					int argLength = rbIndex - lbIndex - 1;

					string argString = part.Substring (0, lbIndex);
					var next = new BindingPathPart (this, argString);

					if (argLength >= 1) {
						next.isSetter = true;
					} else {
						next.isGetter = true;
					}

					last.nextPart = next;
					_parts.Add (next);
					last = next;
				} else if (part.Length > 0) {
					var next = new BindingPathPart (this, part);
					last.nextPart = next;
					_parts.Add (next);
					last = next;
				}

				if (indexer != null) {
					last.nextPart = indexer;
					_parts.Add (indexer);
					last = indexer;
				}
			}

			//解析convert
			if (!string.IsNullOrEmpty (m_Binding?.converter)) {
				converter = EnterLua.luaenv.Global.GetInPath<XLua.LuaTable> ("Convert." + m_Binding?.converter);
			}
		}

		//解析source表达式
		void ApplySource (BindableObject target) {
			var sourcePath = m_Binding.source;
			var parent = target.parent;
			Object relative_source = null;
			if (parent != null) {
				if (sourcePath.Length >= 7 && "parent.".Equals (sourcePath.Substring (0, 7))) {
					m_Path = sourcePath + "." + m_Binding.path;
					relative_source = parent;
				} else if (parent is BindableContainer) {
					var children = ((BindableContainer) parent).children;
					var len = children.Count;
					BindableObject child = null;
					for (int i = 0; i < len; i++) {
						child = children[i];
						if (child.targetName == sourcePath) {
							relative_source = child;
							break;
						}
					}

				}

				if (relative_source != null)
					this.m_Binding.m_Source = relative_source;
			}

		}

		void ApplyActual (object sourceObject, BindableObject target, string property, bool fromTarget = false) {
			var mode = m_Binding.mode;
			if ((mode == BindingMode.OneWay || mode == BindingMode.OneTime) && fromTarget)
				return;

			bool needsGetter = (mode == BindingMode.TwoWay && !fromTarget) || mode == BindingMode.OneWay || mode == BindingMode.OneTime;
			bool needsSetter = !needsGetter && ((mode == BindingMode.TwoWay && fromTarget) || mode == BindingMode.OneWayToSource);
			bool needSubscribe = mode == BindingMode.OneWay || mode == BindingMode.TwoWay;

			ExpressionUtility.ApplyLuaTable (sourceObject, target, property, _parts, needsGetter, needsSetter, needSubscribe);
			// object current = sourceObject;
			// 	BindingPathPart part = null;
			// 	for (var i = 0; i < _parts.Count; i++) {
			// 		part = _parts[i];

			// 		if (!part.isSelf && current != null) {

			// 			if (i < _parts.Count - 1)
			// 				part.TryGetValue (current, out current);

			// 		}

			// 		UnityEngine.Debug.LogFormat ("ApplyActual.current={0},property={1},m_Path={2},parts.Count={3},part={4},i={5},part.isSelf={6}", current, property, m_Path, _parts.Count, part, i, part.isSelf);
			// 		if (!part.isSelf && current == null)
			// 			break;

			// 		if (part.nextPart != null && needSubscribe) {
			// 			if (current is INotifyPropertyChanged) {
			// 				// UnityEngine.Debug.LogFormat ("current = {0}", current);
			// 				part.Subscribe ((INotifyPropertyChanged) current);
			// 			} else if (current is XLua.LuaTable && ((XLua.LuaTable) current).ContainsKey<string> ("PropertyChanged")) {
			// 				UnityEngine.Debug.LogFormat ("注册lua监听 current lua = {0}", current);
			// 				part.Subscribe ((XLua.LuaTable) current);
			// 			}
			// 		}
			// 	}

			// 	if (part == null)
			// 		throw new NullReferenceException ("the last part is null. path = " + this.m_Path);
			// 	// UnityEngine.Debug.LogFormat ("ApplyActual.needsGetter={0},needsSetter={1}", needsGetter, needsSetter);

			// 	if (needsGetter && current != null) {
			// 		UnityEngine.Debug.LogFormat ("needsGetter.target={0},current={1},part={2}", target, current, part.path);
			// 		ExpressionUtility.SetTargetBindingValue (target, sourceObject, current, part);
			// 	} else if (needsSetter && current != null) {
			// 		UnityEngine.Debug.LogFormat ("needsSetter.target={0},current={1},part={2}", target, current, part);
			// 		ExpressionUtility.SetSourceBindingValue (target, sourceObject, current, part);
			// 	}

		}
		#endregion
	}

	public enum BindingMode {
		OneWay,
		TwoWay,
		OneWayToSource,
		OneTime,
	}
}