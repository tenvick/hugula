using System;
namespace Hugula.Databinding
{

    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public interface ICommand
    {
        bool CanExecute(object parameter);
        void Execute(object parameter);
    }

    [XLua.LuaCallCSharp]
    [XLua.CSharpCallLua]
    public interface IExecute
    {
        void Execute(object parameter);
    }

}