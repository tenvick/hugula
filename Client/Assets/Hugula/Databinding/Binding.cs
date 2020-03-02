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
		#region  序列化属性
		///<summary>
		/// The type of the property
		///</summary>
		public Type returnType;

		///<summary>
		/// The name of the BindableProperty
		///</summary>
		public string propertyName;

		// ///<summary>
		// /// propertyName Is Method
		// ///</summary>
		// public bool isMethod;

		// ///<summary>
		// /// {path=".",format=""}
		// ///</summary>
		// public string expression; //表达式{}

		public string path;
		public string format;
		public string converter;
		public string mode;
		public string source;

		BindableObject m_Target;

		//绑定的目标Bindable
		public BindableObject target {
			get { return m_Target; }
			set { m_Target = value; }
		}

		#endregion

		#region  lua实现寻址与赋值
		// //source表达式解析的源
		public object m_Source;
		//引用的上下文
		public object context;

		private bool isApplied = false;
		private bool isInitialize = false;

		///<summary>
		/// 表达式的解析与寻值
		///</summary>
		public IBindingExpression bindingExpression { get; set; }

		public void Initialize () {
			bindingExpression = BindingUtility.NewExpression (this);
			isInitialize = true;
		}

		#endregion

		#region  表达式与寻值
		internal void Apply (bool fromTarget = true) {
			if (!isInitialize) Initialize ();
			bindingExpression.Apply (fromTarget);
			isApplied = true;
		}

		internal void Unapply () {
			if (bindingExpression != null) bindingExpression.Unapply (true);
		}
		#endregion

		public void Dispose () {
			Unapply ();
			m_Target = null;
			m_Source = null;
			context = null;
			bindingExpression = null;
		}

	}

}