using System;

namespace Hugula.Databinding {

	[XLua.LuaCallCSharp]
	[XLua.CSharpCallLua]
	public interface IBindingExpression {
		void Apply (bool fromTarget);
		void Apply (object context, BindableObject bindObj, string targetProperty, bool fromBindingContextChanged);
		void Unapply (bool fromBindingContextChanged);
	}
}