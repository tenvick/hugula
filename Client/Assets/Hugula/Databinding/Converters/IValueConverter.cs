using System;

namespace Hugula.Databinding {
	[XLua.LuaCallCSharp]
	[XLua.CSharpCallLua]
	public interface IValueConverter { //: IValueConverter<object, object> {
		object Convert (object value, Type targetType);
		object ConvertBack (object value, Type targetType);
	}

	public interface IValueConverter<S, T> : IValueConverter {
		T Convert (S value, Type targetType);
		S ConvertBack (T value, Type targetType);
	}
}