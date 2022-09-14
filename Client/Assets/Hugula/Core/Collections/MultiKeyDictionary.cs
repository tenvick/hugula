using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Hugula.Collections
{
    public class Dictionary<TKey1, TKey2, TValue> : Dictionary<Tuple<TKey1, TKey2>, TValue>, IDictionary<Tuple<TKey1, TKey2>, TValue>
    {
        public TValue this[TKey1 key1, TKey2 key2]
        {
            get { return base[Tuple.Create(key1, key2)]; }
            set { base[Tuple.Create(key1, key2)] = value; }
        }

        public void Add(TKey1 key1, TKey2 key2, TValue value)
        {
            base.Add(Tuple.Create(key1, key2), value);
        }

        public void Remove(TKey1 key1, TKey2 key2)
        {
            base.Remove(Tuple.Create(key1, key2));
        }

        /// <summary>
        /// Tries the get value.
        /// </summary>
        /// <param name="key1">The key1.</param>
        /// <param name="key2">The key2.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public bool TryGetValue(TKey1 key1, TKey2 key2, out TValue value)
        {
            return base.TryGetValue(Tuple.Create(key1, key2), out value);
        }

        public bool ContainsKey(TKey1 key1, TKey2 key2)
        {
            return base.ContainsKey(Tuple.Create(key1, key2));
        }

    }
}
