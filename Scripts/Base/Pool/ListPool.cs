//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Qarth
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
