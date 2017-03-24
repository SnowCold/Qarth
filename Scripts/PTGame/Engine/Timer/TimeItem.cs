using System;
using System.Collections.Generic;
using UnityEngine;


namespace PTGame.Framework
{
    public class TimeItem : IBinaryHeapElement
    {
        /*
         * tick:当前第几次
         * 
         */

        private float                   m_DelayTime;
        private bool                    m_IsEnable = true;
        private int                     m_RepeatCount;
        private float                   m_SortScore;
        private Run<int>                m_Callback;
        private int                     m_CallbackTick;
        private int                     m_HeapIndex;

        public TimeItem(Run<int> callback, float delayTime, int repeatCount = 1)
        {
            m_CallbackTick = 0;
            m_Callback = callback;
            m_DelayTime = delayTime;
            m_RepeatCount = repeatCount;
        }

        public void OnTimeTick()
        {
            if (m_Callback != null)
            {
                m_Callback(++m_CallbackTick);
            }

            if (m_RepeatCount > 0)
            {
                --m_RepeatCount;
            }
        }

        public Run<int> callback
        {
            get { return m_Callback; }
        }

        public float sortScore
        {
            get { return m_SortScore; }
            set { m_SortScore = value; }
        }

        public int heapIndex
        {
            get { return m_HeapIndex; }
            set { m_HeapIndex = value; }
        }

        public bool enable
        {
            get { return m_IsEnable; }
            set 
            {
                if (value != m_IsEnable)
                {
                    m_IsEnable = value;
                    if (!m_IsEnable)
                    {
                        m_Callback = null;
                    }
                }
            }
        }

        public bool NeedRepeat()
        {
            if (m_RepeatCount == 0)
            {
                return false;
            }
            return true;
        }

        public float DelayTime()
        {
            return m_DelayTime;
        }

        public void RebuildHeap<T>(BinaryHeap<T> heap) where T : IBinaryHeapElement
        {
            heap.RebuildAtIndex(m_HeapIndex);
        }
    }
}
