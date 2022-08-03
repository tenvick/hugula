using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using Object = System.Object;
namespace Hugula.Databinding
{
    public class ObservableObject : INotifyPropertyChanged
    {
        private PropertyChangedEventHandlerEvent m_PropertyChanged = new PropertyChangedEventHandlerEvent();
        public PropertyChangedEventHandlerEvent PropertyChanged
        {
            get
            {
                return m_PropertyChanged;
            }
        }

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

    }
}