using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using Object = System.Object;
namespace Hugula.Databinding
{
    public class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, propertyName);
        }

        protected bool SetProperty<T1>(ref T1 storage, T1 value, [CallerMemberName] string propertyName = null)
        {
            if (Object.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        // public bool SetProperty()

        // public void OnPropertyChanged(string propertyName)
        // {

        // }
    }
}