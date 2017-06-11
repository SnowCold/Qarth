//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/PTFramework
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;

namespace PTGame.Framework
{
    public enum eBinaryHeapBuildMode
    {
        kNLog = 1,
        kN = 2,
    }

    public enum eBinaryHeapSortMode
    {
        kMin = 0,
        kMax = 1,
    }

    //优先队列&二叉堆
    public class BinaryHeap<T> where T : IBinaryHeapElement
    {
        protected T[] m_Array;

        protected float m_GrowthFactor = 1.6f;
        protected int m_LastChildIndex;//最后子节点的位置
        protected eBinaryHeapSortMode m_SortMode;

        public BinaryHeap(int minSize, eBinaryHeapSortMode sortMode)
        {
            m_SortMode = sortMode;
            m_Array = new T[minSize];
            m_LastChildIndex = 0;
        }

        public BinaryHeap(T[] dataArray, eBinaryHeapSortMode sortMode)
        {
            m_SortMode = sortMode;
            int minSize = 10;
            if (dataArray != null)
            {
                minSize = dataArray.Length + 1;
            }

            m_Array = new T[minSize];
            m_LastChildIndex = 0;
            Insert(dataArray, eBinaryHeapBuildMode.kN);
        }

#region 公开方法

        #region 清空
        public void Clear()
        {
            m_Array = new T[10];
            m_LastChildIndex = 0;
        }
        #endregion

        #region 插入
        public void Insert(T[] dataArray, eBinaryHeapBuildMode buildMode)
        {
            if (dataArray == null)
            {
                throw new NullReferenceException("BinaryHeap Not Support Insert Null Object");
            }

            int totalLength = m_LastChildIndex + dataArray.Length + 1;
            if (m_Array.Length < totalLength)
            {
                ResizeArray(totalLength);
            }

            if (buildMode == eBinaryHeapBuildMode.kNLog)
            {
                //方式1:直接添加，每次添加都会上浮
                for (int i = 0; i < dataArray.Length; ++i)
                {
                    Insert(dataArray[i]);
                }
            }
            else
            {
                //数量比较大的情况下会快一些
                //方式2:先添加完，然后排序
                for (int i = 0; i < dataArray.Length; ++i)
                {
                    m_Array[++m_LastChildIndex] = dataArray[i];
                }

                SortAsCurrentMode();
            }
        }

        public void Insert(T element)
        {
            if (element == null)
            {
                throw new NullReferenceException("BinaryHeap Not Support Insert Null Object");
            }

            int index = ++m_LastChildIndex;

            if (index == m_Array.Length)
            {
                ResizeArray();
            }

            m_Array[index] = element;

            ProcolateUp(index);
        }
#endregion

#region 弹出
        public T Pop()
        {
            if (m_LastChildIndex < 1)
            {
                return default(T);
            }

            T result = m_Array[1];
            m_Array[1] = m_Array[m_LastChildIndex--];
            ProcolateDown(1);
            return result;
        }

        public T Top()
        {
            if (m_LastChildIndex < 1)
            {
                return default(T);
            }

            return m_Array[1];
        }
#endregion

#region 重新排序
        public void Sort(eBinaryHeapSortMode sortMode)
        {
            if (m_SortMode == sortMode)
            {
                return;
            }
            m_SortMode = sortMode;
            SortAsCurrentMode();
        }

        public void RebuildAtIndex(int index)
        {
            if (index > m_LastChildIndex)
            {
                return;
            }

            //1.首先找父节点，是否比父节点小，如果满足则上浮,不满足下沉
            var element = m_Array[index];

            int parentIndex = index >> 1;
            if (parentIndex > 0)
            {
                if (m_SortMode == eBinaryHeapSortMode.kMin)
                {
                    if (element.sortScore < m_Array[parentIndex].sortScore)
                    {
                        ProcolateUp(index);
                    }
                    else
                    {
                        ProcolateDown(index);
                    }
                }
                else
                {
                    if (element.sortScore > m_Array[parentIndex].sortScore)
                    {
                        ProcolateUp(index);
                    }
                    else
                    {
                        ProcolateDown(index);
                    }
                }
            }
            else
            {
                ProcolateDown(index);
            }
        }

        private void SortAsCurrentMode()
        {
            int startChild = m_LastChildIndex >> 1;
            for (int i = startChild; i > 0; --i)
            {
                ProcolateDown(i);
            }
        }
#endregion

#region 指定位置删除
        public void RemoveAt(int index)
        {
            if (index > m_LastChildIndex || index < 1)
            {
                return;
            }

            if (index == m_LastChildIndex)
            {
                --m_LastChildIndex;
                m_Array[index] = default(T);
                return;
            }

            m_Array[index] = m_Array[m_LastChildIndex--];
            m_Array[index].heapIndex = index;
            RebuildAtIndex(index);
        }
#endregion

#region 索引查找
        //这个索引和大小排序之间没有任何关系
        public T GetElement(int index)
        {
            if (index > m_LastChildIndex)
            {
                return default(T);
            }
            return m_Array[index];
        }
#endregion

#region 判定辅助
        public bool HasValue()
        {
            return m_LastChildIndex > 0;
        }
#endregion

#region 内部方法
        protected void ResizeArray(int newSize = -1)
        {
            if (newSize < 0)
            {
                newSize = System.Math.Max(m_Array.Length + 4, (int)System.Math.Round(m_Array.Length * m_GrowthFactor));
            }

            if (newSize > 1 << 30)
            {
                throw new System.Exception("Binary Heap Size really large (2^18). A heap size this large is probably the cause of pathfinding running in an infinite loop. " +
                    "\nRemove this check (in BinaryHeap.cs) if you are sure that it is not caused by a bug");
            }

            T[] tmp = new T[newSize];
            for (int i = 0; i < m_Array.Length; i++)
            {
                tmp[i] = m_Array[i];
            }

            m_Array = tmp;
        }

        //上浮:空穴思想
        protected void ProcolateUp(int index)
        {
            var element = m_Array[index];
            if (element == null)
            {
                return;
            }

            float sortScore = element.sortScore;

            int parentIndex = index >> 1;

            if (m_SortMode == eBinaryHeapSortMode.kMin)
            {
                while (parentIndex >= 1 && sortScore < m_Array[parentIndex].sortScore)
                {
                    m_Array[index] = m_Array[parentIndex];
                    m_Array[index].heapIndex = index;
                    index = parentIndex;
                    parentIndex = index >> 1;
                }
            }
            else
            {
                while (parentIndex >= 1 && sortScore > m_Array[parentIndex].sortScore)
                {
                    m_Array[index] = m_Array[parentIndex];
                    m_Array[index].heapIndex = index;
                    index = parentIndex;
                    parentIndex = index >> 1;
                }
            }
            m_Array[index] = element;
            m_Array[index].heapIndex = index;
        }

        protected void ProcolateDown(int index)
        {
            var element = m_Array[index];
            if (element == null)
            {
                return;
            }

            int childIndex = index << 1;

            if (m_SortMode == eBinaryHeapSortMode.kMin)
            {
                while (childIndex <= m_LastChildIndex)
                {
                    if (childIndex != m_LastChildIndex)
                    {
                        if (m_Array[childIndex + 1].sortScore < m_Array[childIndex].sortScore)
                        {
                            childIndex = childIndex + 1;
                        }
                    }

                    if (m_Array[childIndex].sortScore < element.sortScore)
                    {
                        m_Array[index] = m_Array[childIndex];
                        m_Array[index].heapIndex = index;
                    }
                    else
                    {
                        break;
                    }

                    index = childIndex;
                    childIndex = index << 1;
                }
            }
            else
            {
                while (childIndex <= m_LastChildIndex)
                {
                    if (childIndex != m_LastChildIndex)
                    {
                        if (m_Array[childIndex + 1].sortScore > m_Array[childIndex].sortScore)
                        {
                            childIndex = childIndex + 1;
                        }
                    }

                    if (m_Array[childIndex].sortScore > element.sortScore)
                    {
                        m_Array[index] = m_Array[childIndex];
                        m_Array[index].heapIndex = index;
                    }
                    else
                    {
                        break;
                    }

                    index = childIndex;
                    childIndex = index << 1;
                }
            }

            m_Array[index] = element;
            m_Array[index].heapIndex = index;
        }
#endregion

#endregion
    }
}


