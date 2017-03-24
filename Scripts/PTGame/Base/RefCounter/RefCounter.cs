using System;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace PTGame.Framework
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
