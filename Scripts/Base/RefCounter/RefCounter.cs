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
    public interface IRefCounter
    {
        int refCount
        {
            get;
        }

        void AddRef();
        void SubRef();
    }

    public class RefCounter : IRefCounter
    {
        private int m_RefCount = 0;

        public int refCount
        {
            get { return m_RefCount; }
        }

        public void AddRef() { ++m_RefCount; }
        public void SubRef()
        {
            --m_RefCount;
            if (m_RefCount == 0)
            {
                OnZeroRef();
            }
        }

        protected virtual void OnZeroRef()
        {

        }
    }
}
