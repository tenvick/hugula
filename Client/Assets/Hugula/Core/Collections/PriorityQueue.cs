using System;
using System.Collections;
using System.Collections.Generic;

namespace Hugula.Collections
{
	[Serializable]
	public class PriorityQueue<T> : IEnumerable<T>, ICollection
	{
		private readonly List<T> _list;
		private readonly IComparer<T> _comparer;

		bool ICollection.IsSynchronized
		{
			get { return ((ICollection)_list).IsSynchronized; }
		}

		object ICollection.SyncRoot
		{
			get { return ((ICollection)_list).SyncRoot; }
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((ICollection)_list).CopyTo(array, index);
		}

		public PriorityQueue() : this(Comparer<T>.Default) { }
		public PriorityQueue(IEnumerable<T> collection) : this(collection, Comparer<T>.Default) { }
		public PriorityQueue(int capacity) : this(capacity, Comparer<T>.Default) { }

		public PriorityQueue(IComparer<T> comparer)
		{
			_list = new List<T>();
			_comparer = comparer;
		}

		public PriorityQueue(IEnumerable<T> collection, IComparer<T> comparer)
		{
			_list = new List<T>(collection);
			_comparer = comparer;
			_list.Sort(_comparer);
		}

		public PriorityQueue(int capacity, IComparer<T> comparer)
		{
			_list = new List<T>(capacity);
			_comparer = comparer;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_list).GetEnumerator();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		public int Count
		{
			get { return _list.Count; }
		}

		public T this[int index]
		{
			get
			{
				return _list[index];
			}
		}

		public bool Empty
		{
			get { return _list.Count == 0; }
		}

		public void CopyTo(T[] array)
		{
			_list.CopyTo(array);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public void CopyTo(int index, T[] array, int arrayIndex, int count)
		{
			_list.CopyTo(index, array, arrayIndex, count);
		}

		public T[] ToArray()
		{
			return _list.ToArray();
		}

		public void TrimExcess()
		{
			_list.TrimExcess();
		}

		public T Peek()
		{
			try
			{
				return _list[0];
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(e.Message);
			}
		}

		public void Clear()
		{
			_list.Clear();
		}

		public void Push(T item)
		{
			int n = _list.Count;
			_list.Add(item);
			while (n > 0)
			{
				int m = (n - 1) / 2;
				if (_comparer.Compare(_list[m], _list[n]) <= 0)
					break;
				var tmp = _list[m];
				_list[m] = _list[n];
				_list[n] = tmp;
				n = m;
			}
		}

		public T Pop()
		{
			try
			{
				T result = _list[0];
				remove(0);
				return result;
			}
			catch (Exception e)
			{
				throw new InvalidOperationException(e.Message);
			}
		}

		public bool Remove(T item)
		{
			return _list.Remove(item);
		}

		public void RemoveAt(int i)
		{
			_list.RemoveAt(i);
		}

		private void remove(int n)
		{
			int length = _list.Count - 1;
			_list[0] = _list[length];
			_list.RemoveAt(length);
			while (n * 2 + 1 < length)
			{
				int m = n * 2 + 1;
				if (m + 1 < length && _comparer.Compare(_list[m + 1], _list[m]) < 0)
					++m;
				if (_comparer.Compare(_list[n], _list[m]) <= 0)
					break;
				var tmp = _list[m];
				_list[m] = _list[n];
				_list[n] = tmp;
				n = m;
			}
		}
	}
}