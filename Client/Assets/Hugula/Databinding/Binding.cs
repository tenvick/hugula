// Copyright (c) 2020 hugula
// direct https://github.com/tenvick/hugula
//
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Hugula.Databinding {

	[System.SerializableAttribute]
	public class Binding : IDisposable //:IBinding
	{
		internal const string SelfPath = ".";

		#region  序列化属性
		///<summary>
		/// The type of the property
		///</summary>
		public Type returnType;

		///<summary>
		/// The name of the BindableProperty
		///</summary>
		public string propertyName;

		public string path;
		public string format;
		public string converter;
		public BindingMode mode;
		public string source;

		#endregion

		#region  lua实现寻址与赋值
		// //source表达式解析的源
		public object m_Source;
		//引用的上下文
		public object context;

		private bool isApplied = false;

		///<summary>
		/// 表达式的解析与寻值
		///</summary>
		public IBindingExpression bindingExpression { get; set; }

		#endregion

		#region  表达式与寻值
		internal void Apply (bool fromTarget = true) {
			if (bindingExpression == null) bindingExpression = new BindingExpression (this);
			// Action act = () => bindingExpression.Apply (fromTarget);
			// Executor.Execute (act);
			bindingExpression.Apply (fromTarget);
			isApplied = true;
		}

		//初始化绑定
		internal void Apply (object cont, BindableObject bindObj, bool fromBindingContextChanged = false) {

			if (bindingExpression == null) {
				if (string.IsNullOrEmpty (source))
					bindingExpression = new BindingExpression (this);
				else
					bindingExpression = new BindingExpression (this, bindObj);
			}

			object bindingContext = cont;
			if (m_Source != null) bindingContext = m_Source;
			bindingExpression.Apply (bindingContext, bindObj, propertyName, fromBindingContextChanged);
			isApplied = true;

		}

		internal void Unapply (bool fromBindingContextChanged = false) {
			if (bindingExpression != null) bindingExpression.Unapply (fromBindingContextChanged);
		}

		#endregion

		public void Dispose () {
			Unapply ();
			// m_Target = null;
			m_Source = null;
			context = null;
			bindingExpression = null;
		}

	}

}