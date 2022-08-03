using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Hugula.Databinding
{

    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public delegate void PropertyChangedEventHandler(object sender, string propertyName);

    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public interface INotifyPropertyChanged
    {
        PropertyChangedEventHandlerEvent PropertyChanged{get;}
    }

    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public delegate void NotifyCollectionChangedEventHandler(object sender, HugulaNotifyCollectionChangedEventArgs args);

    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public interface INotifyCollectionChanged
    {
        NotifyCollectionChangedEventHandlerEvent CollectionChanged{get;}
    }

    //用于lua NotifyTable类型装换
    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public interface INotifyTable : IList, INotifyCollectionChanged, INotifyPropertyChanged
    {

    }

}