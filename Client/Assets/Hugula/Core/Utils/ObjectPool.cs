using UnityEngine;
using System.Collections.Generic;
using System;

namespace Hugula.Utils
{
    /// <summary>
    /// this copy from https://bitbucket.org/Unity-Technologies/ui/src/b5f9aae6ff7c2c63a521a1cb8b3e3da6939b191b/UnityEngine.UI/UI/Core/Utility/ObjectPool.cs?at=5.3&fileviewer=file-view-default
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPool<T> where T : new()
    {
        private readonly Stack<T> m_Stack = new Stack<T>();
        private readonly System.Action<T> m_ActionOnGet;
        private readonly System.Action<T> m_ActionOnRelease;

        public int countAll { get; private set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public ObjectPool(Action<T> actionOnGet, Action<T> actionOnRelease)
        {
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
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
        private static readonly ObjectPool<List<T>> s_ListPool = new ObjectPool<List<T>>(null, l => l.Clear());

        public static List<T> Get()
        {
            return s_ListPool.Get();
        }

        public static void Release(List<T> toRelease)
        {
            s_ListPool.Release(toRelease);
        }
    }

    public interface IReleaseToPool
    {
        void ReleaseToPool();
    }
}