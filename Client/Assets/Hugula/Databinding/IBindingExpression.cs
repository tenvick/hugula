using System;

namespace Hugula.Databinding {


	[XLua.LuaCallCSharp]
	[XLua.CSharpCallLua]
	public interface IBindingExpression {
		void Apply(bool fromTarget);
		void Unapply(bool fromBindingContextChanged);
	}
}