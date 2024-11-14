using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;

namespace Hugula.Databinding
{

    public class ObservableCollection<T> : ICollection<T>, ICollection, IList<T>, IList, IReadOnlyCollection<T>, IReadOnlyList<T>, INotifyTable, INotifyCollectionChanged, INotifyPropertyChanged
    {
        Monitor monitor = new Monitor();
        IList<T> m_Items;

        public ObservableCollection()
        {
            m_Items = new List<T>();
        }
        public ObservableCollection(IEnumerable<T> collection)
        {
            m_Items = new List<T>(collection);
        }
        public ObservableCollection(List<T> list)
        {
            m_Items = new List<T>(list);
        }

        protected IDisposable BlockReentrancy()
        {
            this.monitor.Add();
            return this.monitor;
        }

        protected void CheckReentrancy()
        {
            if (this.monitor.Busy)
            {
                if ((this.CollectionChanged != null) && (this.CollectionChanged.Count > 1))
                    throw new InvalidOperationException();
            }
        }

        public int FindIndex(System.Func<int, object, bool> filter)
        {
            for (int i = 0; i < m_Items.Count; i++)
            {
                if (filter(i, m_Items[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        #region Ilist
        object IList.this[int index]
        {
            get
            {
                return m_Items[index];
            }
            set
            {
                T val = (T)value;
                m_Items[index] = val;
            }
        }

        bool IList.IsFixedSize
        {
            get
            {
                IList list = m_Items as IList;
                if (list != null)
                {
                    return list.IsFixedSize;
                }
                return m_Items.IsReadOnly;
            }
        }
        bool IList.IsReadOnly { get { return m_Items.IsReadOnly; } }

        int IList.Add(object value)
        {
            Add((T)value);
            return m_Items.Count;
        }

        void IList.Clear()
        {
            ClearItems();
        }

        bool IList.Contains(object value)
        {
            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            Insert(index, (T)value);
        }
        void IList.Remove(object value)
        {
            Remove((T)value);
        }

        void IList.RemoveAt(int index)
        {
            RemoveItem(index);
        }
        #endregion

        #region  Icollection
        private object syncRoot;
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }

        object ICollection.SyncRoot
        {
            get
            {
                if (this.syncRoot == null)
                {
                    ICollection c = m_Items as ICollection;
                    if (c != null)
                    {
                        this.syncRoot = c.SyncRoot;
                    }
                    else
                    {
                        Interlocked.CompareExchange<Object>(ref this.syncRoot, new Object(), null);
                    }
                }
                return this.syncRoot;
            }
        }

        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (array.Rank != 1)
                throw new ArgumentException("RankMultiDimNotSupported");

            if (array.GetLowerBound(0) != 0)
                throw new ArgumentException("NonZeroLowerBound");

            if (index < 0)
                throw new ArgumentOutOfRangeException(string.Format("ArgumentOutOfRangeException:{0}", index));

            if (array.Length - index < Count)
                throw new ArgumentException("ArrayPlusOffTooSmall");

            T[] tArray = array as T[];
            if (tArray != null)
            {
                m_Items.CopyTo(tArray, index);
            }
            else
            {
                Type targetType = array.GetType().GetElementType();
                Type sourceType = typeof(T);
                if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
                    throw new ArgumentException("InvalidArrayType");

                object[] objects = array as object[];
                if (objects == null)
                    throw new ArgumentException("InvalidArrayType");

                int count = m_Items.Count;
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                        objects[index++] = m_Items[i];
                    }
                }
                catch (ArrayTypeMismatchException)
                {
                    throw new ArgumentException("InvalidArrayType");
                }
            }
        }

        #endregion

        #region  IList<T> ICollection<T>
        public int IndexOf(T item)
        {
            return m_Items.IndexOf(item);
        }

        public void Add(T item)
        {
            if (m_Items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            InsertItem(m_Items.Count, item);
        }

        public void Insert(int index, T item)
        {
            if (m_Items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            m_Items.Insert(index, item);
        }

        public bool Contains(T item)
        {
            return m_Items.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (m_Items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            m_Items.CopyTo(array, arrayIndex);
        }

        public void RemoveAt(int index)
        {
            if (m_Items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            RemoveItem(index);
        }

        public bool Remove(T item)
        {
            if (m_Items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");

            int index = m_Items.IndexOf(item);
            if (index < 0)
                return false;
            RemoveItem(index);
            return true;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerator)m_Items.GetEnumerator());
        }

        public void Clear()
        {
            if (m_Items.IsReadOnly)
                throw new NotSupportedException("ReadOnlyCollection");
            ClearItems();
        }

        public int Count
        {
            get
            {
                return m_Items.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return m_Items.IsReadOnly;
            }
        }

        public T this[int i]
        {
            get
            {
                return m_Items[i];
            }
            set
            {
                // m_Items[i] = value;
                SetItem(i, value);
            }
        }

        #endregion

        private NotifyCollectionChangedEventHandlerEvent m_CollectionChanged = NotifyCollectionChangedEventHandlerEvent.Get();// new NotifyCollectionChangedEventHandlerEvent();
        public NotifyCollectionChangedEventHandlerEvent CollectionChanged
        {
            get
            {
                return m_CollectionChanged;
            }
        }

        private PropertyChangedEventHandlerEvent m_PropertyChanged = new PropertyChangedEventHandlerEvent();
        public PropertyChangedEventHandlerEvent PropertyChanged
        {
            get
            {
                return m_PropertyChanged;
            }
        }


        protected virtual void ClearItems()
        {
            CheckReentrancy();
            m_Items.Clear();
            OnPropertyChanged("Item[]");
            // OnCollectionChanged(new HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            OnCollectionChanged(CollectionChangedEventArgsUtility.CreateCollectionArgsChangedItemIndex(NotifyCollectionChangedAction.Reset,0,0));
        }

        protected virtual void RemoveItem(int index)
        {
            CheckReentrancy();
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index");

            T removedItem = m_Items[index];

            m_Items.RemoveAt(index);

            OnPropertyChanged(string.Format("Item[{0}]", index));

            // OnCollectionChanged(new HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, removedItem, index));
            OnCollectionChanged(CollectionChangedEventArgsUtility.CreateCollectionArgsChangedItemIndex(NotifyCollectionChangedAction.Remove, 1, index));

        }

        protected virtual void InsertItem(int index, T item)
        {
            CheckReentrancy();
            if (index < 0 || index > Count + 1)
                throw new ArgumentOutOfRangeException("index");

            m_Items.Insert(index, item);

            OnPropertyChanged(string.Format("Item[{0}]", index));
            // OnCollectionChanged(new HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            OnCollectionChanged(CollectionChangedEventArgsUtility.CreateCollectionArgsChangedItemIndex(NotifyCollectionChangedAction.Add, 1, index));

        }

        protected virtual void SetItem(int index, T item)
        {
            CheckReentrancy();
            if (index < 0 || index >= Count)
                throw new ArgumentOutOfRangeException("index");
            // T originalItem = m_Items[index];

            m_Items[index] = item;

            OnPropertyChanged(string.Format("Item[{0}]", index));
            // OnCollectionChanged(new HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, originalItem, index));
            OnCollectionChanged(CollectionChangedEventArgsUtility.CreateCollectionArgsNewItemOldItemIndex(NotifyCollectionChangedAction.Replace, 1, 1, index));

        }

        protected virtual void MoveItem(int oldIndex, int newIndex)
        {
            CheckReentrancy();

            if (oldIndex < 0 || oldIndex > Count)
            {
                throw new ArgumentOutOfRangeException("oldIndex");
            }

            if (newIndex < 0 || newIndex > Count)
            {
                throw new ArgumentOutOfRangeException("oldIndex");
            }

            var oldItem = m_Items[oldIndex];
            var newItem = m_Items[newIndex];
            m_Items[oldIndex] = newItem;
            m_Items[newIndex] = oldItem;

            OnPropertyChanged(string.Format("Item[{0}]", oldIndex));
            OnPropertyChanged(string.Format("Item[{0}]", newIndex));

            // OnCollectionChanged(new HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, newItem, newIndex, oldIndex));
            OnCollectionChanged(CollectionChangedEventArgsUtility.CreateCollectionArgsChangedItemIndexOldIndex(NotifyCollectionChangedAction.Move, 1, newIndex, oldIndex));

        }

        protected virtual void OnCollectionChanged(HugulaNotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, propertyName);
        }

        public void InsertRange(int index, IEnumerable<T> range)
        {
            if (index < 0 || index > Count)
                throw new ArgumentOutOfRangeException("index");
            if (range == null)
                throw new ArgumentNullException("range");

            int originalIndex = index;

            List<T> items = range.ToList();
            foreach (T item in items)
                m_Items.Insert(index++, item);

            // OnCollectionChanged(new HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items, originalIndex));
            OnCollectionChanged(CollectionChangedEventArgsUtility.CreateCollectionArgsChangedItemsStartingIndex(NotifyCollectionChangedAction.Add, items.Count, originalIndex));

        }

        public void RemoveRange(IEnumerable<T> range)
        {
            if (range == null)
                throw new ArgumentNullException("range");

            int idxMin = m_Items.Count;
            int delCount = 0;
            List<T> items = range.ToList();
            foreach (T item in items)
            {
                int idx = m_Items.IndexOf(item);
                if (idx >= 0)
                {
                    m_Items.RemoveAt(idx);
                    delCount++;
                    if (idx < idxMin)
                        idxMin = idx;
                }
            }

            // OnCollectionChanged(new HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, items));
            if (delCount > 0)
                OnCollectionChanged(CollectionChangedEventArgsUtility.CreateCollectionArgsChangedItemIndex(NotifyCollectionChangedAction.Remove, delCount, idxMin));
        }

        public void ReplaceRange(int startIndex, IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("m_Items");

            T[] ritems = items.ToArray();

            if (startIndex < 0 || startIndex + ritems.Length > Count)
                throw new ArgumentOutOfRangeException("startIndex");

            var oldItems = new T[ritems.Length];
            for (var i = 0; i < ritems.Length; i++)
            {
                oldItems[i] = m_Items[i + startIndex];
                m_Items[i + startIndex] = ritems[i];
            }

            int repCount = oldItems.Length;

            // OnCollectionChanged(new HugulaNotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, ritems, oldItems, startIndex));
            OnCollectionChanged(CollectionChangedEventArgsUtility.CreateCollectionArgsNewItemsOldItemsStartingIndex(NotifyCollectionChangedAction.Replace, repCount, repCount, startIndex));

        }

        [Serializable]
        class Monitor : IDisposable
        {
            private int _count;
            public bool Busy { get { return _count > 0; } }

            public void Add()
            {
                ++_count;
            }

            public void Dispose()
            {
                --_count;
            }
        }
    }

}