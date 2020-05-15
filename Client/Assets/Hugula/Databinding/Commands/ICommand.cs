using System;
namespace Hugula.Databinding
{
    public interface ICommand
    {
        bool CanExecute(object parameter);
        void Execute(object parameter);
    }

    public interface IExecute
    {
        void Execute(object parameter);
    }

}