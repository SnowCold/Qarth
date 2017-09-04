//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;
using System.Collections.Generic;
using UnityEngine;


namespace Qarth
{
    public partial class Timer
    {
        public class TimeItem : IBinaryHeapElement, ICacheAble, ICacheType
        {
            /*
             * tick:当前第几次
             * 
             */
            private static int s_NextID;
            private static Dictionary<int, TimeItem> s_TimeItemMap = new Dictionary<int, TimeItem>();
            private float m_DelayTime;
            private bool m_IsEnable = true;
            private int m_RepeatCount;
            private float m_SortScore;
            private Run<int> m_Callback;
            private int m_CallbackTick;
            private int m_HeapIndex;
            private bool m_IsCache;

            private int m_ID = -1;

            public int id
            {
                get { return m_ID; }
                private set { m_ID = value; }
            }

            public static TimeItem Allocate(Run<int> callback, float delayTime, int repeatCount = 1)
            {
                TimeItem item = ObjectPool<TimeItem>.S.Allocate();
                item.Set(callback, delayTime, repeatCount);
                return item;
            }

            public void Set(Run<int> callback, float delayTime, int repeatCount)
            {
                m_CallbackTick = 0;
                m_Callback = callback;
                m_DelayTime = delayTime;
                m_RepeatCount = repeatCount;
                RegisterActiveTimeItem(this);
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

            public bool isEnable
            {
                get { return m_IsEnable; }
            }

            public bool cacheFlag
            {
                get
                {
                    return m_IsCache;
                }

                set
                {
                    m_IsCache = value;
                }
            }

            public void Cancel()
            {
                if (m_IsEnable)
                {
                    m_IsEnable = false;
                    m_Callback = null;
                }
            }

            public static TimeItem GetTimeItemByID(int id)
            {
                TimeItem unit;
                if (s_TimeItemMap.TryGetValue(id, out unit))
                {
                    return unit;
                }
                return null;
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

            public void OnCacheReset()
            {
                m_CallbackTick = 0;
                m_Callback = null;
                m_IsEnable = true;
                m_HeapIndex = 0;
                UnRegisterActiveTimeItem(this);
                m_ID = -1;
            }

            public void Recycle2Cache()
            {
                //超出缓存最大值
                ObjectPool<TimeItem>.S.Recycle(this);
            }

            private static void RegisterActiveTimeItem(TimeItem unit)
            {
                unit.id = ++s_NextID;
                s_TimeItemMap.Add(unit.id, unit);
            }

            private static void UnRegisterActiveTimeItem(TimeItem unit)
            {
                if (s_TimeItemMap.ContainsKey(unit.id))
                {
                    s_TimeItemMap.Remove(unit.id);
                }
                unit.id = -1;
            }
        }
    }
}
