using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
{
    public class ListPool<T>
    {
        private static Stack<List<T>> m_CacheStack;

        public static List<T> Allocate()
        {
            List<T> result;
            if (m_CacheStack == null || m_CacheStack.Count == 0)
            {
                result = new List<T>();
            }
            else
            {
                result = m_CacheStack.Pop();
            }

            return result;
        }

        public static void Recycle(List<T> t)
        {
            if (t == null)
            {
                return;
            }

            t.Clear();

            if (m_CacheStack == null)
            {
                m_CacheStack = new Stack<List<T>>();
            }

            m_CacheStack.Push(t);
        }
    }
}
