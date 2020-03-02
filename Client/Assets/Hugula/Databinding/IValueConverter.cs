using System;

namespace Hugula.Databinding
{
    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public interface IValueConverter
	{
		object Convert(object value, Type targetType, object parameter);
		object ConvertBack(object value, Type targetType, object parameter);
	}
}