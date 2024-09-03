using System;
using System.Collections.Generic;
using UnityEngine;

namespace Hugula.Utils
{
    /// <summary>
    /// this copy from https://bitbucket.org/Unity-Technologies/ui/src/b5f9aae6ff7c2c63a521a1cb8b3e3da6939b191b/UnityEngine.UI/UI/Core/Utility/ObjectPool.cs?at=5.3&fileviewer=file-view-default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> m_Stack ;
        private readonly System.Action<T> m_ActionOnGet;
        private readonly System.Action<T> m_ActionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public ObjectPool(Action<T> actionOnGet, Action<T> actionOnRelease)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
            m_Stack = new Stack<T>();
        }

        public ObjectPool(Action<T> actionOnGet, Action<T> actionOnRelease, int capacity, int initial = 0)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
            if (capacity <= 0)
                m_Stack = new Stack<T>();
            else
                m_Stack = new Stack<T>(capacity);

            for (int i = 0; i < initial; i++)
            {
                m_Stack.Push(new T());
            }
        }


        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = new T();
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
            }
            if (m_ActionOnGet != null)
                m_ActionOnGet(element);
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            if (m_ActionOnRelease != null)
                m_ActionOnRelease(element);
            m_Stack.Push(element);
        }
    }

    /// <summary>
    /// this copy from https://bitbucket.org/Unity-Technologies/ui/src/b5f9aae6ff7c2c63a521a1cb8b3e3da6939b191b/UnityEngine.UI/UI/Core/Utility/ListPool.cs?at=5.3&fileviewer=file-view-default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class ListPool<T>
    {
        // Object pool to avoid allocations.
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, DefaultRelease,512);
        private static void DefaultRelease(List<T> toRelease)
        {
            toRelease.Clear();
        }
        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }

    /// <summary>
    /// pool dictionary
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public static class DictionaryPool<K, V>
    {
        private static readonly ObjectPool<Dictionary<K, V>> s_ListPool = new ObjectPool<Dictionary<K, V>>(null, DefaultRelease,1024);

        private static void DefaultRelease(Dictionary<K, V> toRelease)
        {
            toRelease.Clear();
        }

        public static Dictionary<K, V> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(Dictionary<K, V> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }

    public interface IReleaseToPool
    {
        void ReleaseToPool();
    }

    public interface IReset
    {
        void Reset();
    }

    public class GameObjectPool<T> where T : Component
    {

        private Dictionary<int, Stack<T>> m_StackDic = new Dictionary<int, Stack<T>>();
        private System.Action<T> m_ActionOnGet;
        private System.Action<T> m_ActionOnRelease;
        public Dictionary<int, T> m_Source = new Dictionary<int, T>();

        // public int countAll { get; private set; }
        // public int countActive { get { return countAll - countInactive; } }
        // public int countInactive { get { return m_StackDic.Count; } }
        public GameObjectPool() { }
        public GameObjectPool(System.Action<T> actionOnGet, System.Action<T> actionOnRelease)
        {
            this.m_ActionOnGet = actionOnGet;
            this.m_ActionOnRelease = actionOnRelease;
        }
        public void Clear(int key)
        {
            m_Source.Remove(key);
            m_StackDic.Remove(key);
        }

        public bool Contains(int key)
        {
            return m_Source.ContainsKey(key) && m_StackDic.ContainsKey(key);
        }

        public void Add(int key, T comp)
        {
            if (m_Source.ContainsKey(key)) m_Source.Remove(key);
            if (m_StackDic.ContainsKey(key)) m_StackDic.Remove(key);
            m_Source.Add(key, comp);
            m_StackDic.Add(key, new Stack<T>());
        }

        public T Get(int key, Transform parent, out bool isNew)
        {
            isNew = false;
            Stack<T> m_Stack;
            if (!m_StackDic.TryGetValue(key, out m_Stack)) return null;

            T element = null;

            if (m_Stack.Count == 0)
            {
                T source = null;
                if (m_Source.TryGetValue(key, out source))
                {
                    isNew = true;
                    var obj = GameObject.Instantiate(source.gameObject, parent);
                    element = obj.GetComponent<T>();
                }
                // countAll++;
            }
            else
            {
                element = m_Stack.Pop();
            }
            if (m_ActionOnGet != null)
                m_ActionOnGet(element);
            return element;
        }

        // public T Get(int key, Transform parent = null)
        // {
        //     Stack<T> m_Stack;
        //     if (!m_StackDic.TryGetValue(key, out m_Stack)) return null;

        //     T element = null;

        //     if (m_Stack.Count == 0)
        //     {
        //         T source = null;
        //         if (m_Source.TryGetValue(key, out source))
        //         {
        //             var obj = GameObject.Instantiate(source.gameObject, parent);
        //             element = obj.GetComponent<T>();
        //         }
        //         // countAll++;
        //     }
        //     else
        //     {
        //         element = m_Stack.Pop();
        //     }
        //     if (m_ActionOnGet != null)
        //         m_ActionOnGet(element);
        //     element.gameObject.SetActive(true);
        //     return element;
        // }

        public void Release(int key, T element)
        {
            if (element == null) return;
            element.gameObject.SetActive(false);
            Stack<T> m_Stack;
            if (!m_StackDic.TryGetValue(key, out m_Stack)) return;

            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError($"Internal error. Trying to destroy GameObject {element}  that was already released to pool.");
            if (m_ActionOnRelease != null)
                m_ActionOnRelease(element);
            m_Stack.Push(element);
        }

        public void Dispose()
        {
            foreach (var kv in m_StackDic)
            {
                // foreach (var gobj in kv.Value)
                // {
                //     GameObject.Destroy(gobj?.gameObject, 0.01f);
                // }
                kv.Value.Clear();
            }

            m_StackDic.Clear();
            m_Source.Clear();
            m_ActionOnGet = null;
            m_ActionOnRelease = null;

        }
    }
}