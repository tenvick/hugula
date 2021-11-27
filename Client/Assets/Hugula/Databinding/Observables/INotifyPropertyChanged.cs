using System;
using System.Collections;
using System.Collections.Specialized;

namespace Hugula.Databinding {

	[XLua.LuaCallCSharp]
	[XLua.CSharpCallLua]
	public delegate void PropertyChangedEventHandler (object sender, string propertyName);

	[XLua.LuaCallCSharp]
	[XLua.CSharpCallLua]
	public interface INotifyPropertyChanged {
		event PropertyChangedEventHandler PropertyChanged;
	}

	[XLua.LuaCallCSharp]
	[XLua.CSharpCallLua]
	public delegate void NotifyCollectionChangedEventHandler (object sender, HugulaNotifyCollectionChangedEventArgs args);

	[XLua.LuaCallCSharp]
	[XLua.CSharpCallLua]
	public interface INotifyCollectionChanged {
		event NotifyCollectionChangedEventHandler CollectionChanged;
	}

	//用于lua NotifyTable类型装换
	[XLua.LuaCallCSharp]
	[XLua.CSharpCallLua]
	public interface INotifyTable:IList,INotifyCollectionChanged,INotifyPropertyChanged
	{

	}

}