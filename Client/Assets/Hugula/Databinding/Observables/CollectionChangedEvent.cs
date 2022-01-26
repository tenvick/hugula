using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using Object = System.Object;
using Hugula.Utils;

namespace Hugula.Databinding
{
    public class CollectionChangedEvent
    {
		event NotifyCollectionChangedEventHandler PropertyChanged;

        public int count
        {
            get;
            private set;
        }

        public void Add(NotifyCollectionChangedEventHandler handler)
        {
            count ++;
            PropertyChanged += handler;
        }

        public void Remove(NotifyCollectionChangedEventHandler handler)
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

        public void Dispatch(object obj,HugulaNotifyCollectionChangedEventArgs args)
        {
            PropertyChanged?.Invoke(obj,args);
        }

        private static ObjectPool<CollectionChangedEvent> m_Pool = new ObjectPool<CollectionChangedEvent>(null,OnRelease,32);


        private static void OnRelease(CollectionChangedEvent pEvent)
        {
            pEvent.Clear();
        }

        public static CollectionChangedEvent Get()
        {
            return m_Pool.Get();
        }


    }
}