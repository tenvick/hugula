using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using Object = System.Object;
using Hugula.Utils;

namespace Hugula.Databinding
{
    public class PropertyChangedEvent
    {
		event PropertyChangedEventHandler PropertyChanged;

        public int count
        {
            get;
            private set;
        }

        public void Add(PropertyChangedEventHandler handler)
        {
            count ++;
            PropertyChanged += handler;
        }

        public void Remove(PropertyChangedEventHandler handler)
        {
            count --;
            PropertyChanged -= handler;
        }

        public void Clear()
        {
            count = 0;
            PropertyChanged = null;
        }

        public void Release()
        {
            m_Pool.Release(this);
        }

        public void Dispatch(object obj,string propertyName)
        {
            PropertyChanged?.Invoke(obj,propertyName);
        }

        private static ObjectPool<PropertyChangedEvent> m_Pool = new ObjectPool<PropertyChangedEvent>(null,OnRelease,32);


        private static void OnRelease(PropertyChangedEvent pEvent)
        {
            pEvent.Clear();
        }

        public static PropertyChangedEvent Get()
        {
            return m_Pool.Get();
        }


    }
}