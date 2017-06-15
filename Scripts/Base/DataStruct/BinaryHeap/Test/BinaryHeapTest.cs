//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;

namespace Qarth
{
    public class BinaryHeapTest : BaseTestUnit
    {
        class IntWrap : IBinaryHeapElement
        {
            private int m_SortValue;
            private int m_HeapIndex;

            public IntWrap(int value)
            {
                m_SortValue = value;
            }

            public float sortScore
            {
                get
                {
                    return m_SortValue;
                }

                set
                {
                    m_SortValue = (int)value;
                }
            }

            public int heapIndex
            {
                get { return m_HeapIndex; }
                set { m_HeapIndex = value; }
            }

            public void RebuildHeap<T>(BinaryHeap<T> heap) where T : IBinaryHeapElement
            {
                heap.RebuildAtIndex(m_HeapIndex);
            }
        }

        public override void StartTest()
        {
            //测试范围：1创建，2，数组创建，3.插入，4，弹出，重建，指定位置重建，指定位置删除
            BinaryHeap<IntWrap> heap1 = new BinaryHeap<IntWrap>(10, eBinaryHeapSortMode.kMin);
            BinaryHeap<IntWrap> heap2 = new BinaryHeap<IntWrap>(10, eBinaryHeapSortMode.kMin);
            IntWrap[] mTestData = BuildRandomIntWrapArray(20);//BuildIntWrapArray(new int[]{ 10, 121, 11, 15, 18, 21, 7, 99, 182, 0, 87, 26, 21 });
            //TimeDebugger.S.Begin("Insert:NLog");
            heap1.Insert(mTestData, eBinaryHeapBuildMode.kNLog);
            //TimeDebugger.S.End();

            //TimeDebugger.S.Begin("Insert:N");
            heap2.Insert(mTestData, eBinaryHeapBuildMode.kN);
            //TimeDebugger.S.End();
            //TimeDebugger.S.Dump();

            WriteBegin("BinaryHeap");

            IntWrap insertItem = new IntWrap(39);
            heap1.Insert(insertItem);
            int oldIndex = insertItem.heapIndex;
            IntWrap findResult = heap1.GetElement(oldIndex);
            if (insertItem.Equals(findResult))
            {
                WriteLine("## Success: Find Old Element In Index");
            }
            insertItem.sortScore = 7;
            insertItem.RebuildHeap(heap1);
            int newIndex = insertItem.heapIndex;
            findResult = heap1.GetElement(newIndex);
            if (insertItem.Equals(findResult))
            {
                WriteLine("## Success: Find New Element In Index");
            }
            WriteLine("## InserTest: OldIndex:{0}, newIndex:{1}", oldIndex, insertItem.heapIndex);

            IntWrap element = null;
            int processCount = 0;
            heap1.Sort(eBinaryHeapSortMode.kMax);
            newIndex = insertItem.heapIndex;
            WriteLine("## InsertTest: NewIndex After Sort:{0}", newIndex);
            heap1.RemoveAt(insertItem.heapIndex);
            while (heap1.HasValue() && ++processCount < 100)
            {
                element = heap1.Pop();
                if (element != null)
                {
                    Write("     " + element.sortScore);
                }
                else
                {
                    Write("     NULL");
                }
            }
            WriteLine("");
            WriteEnd("BinaryHeap");
        }

        private IntWrap[] BuildIntWrapArray(int[] dataArray)
        {
            IntWrap[] result = new IntWrap[dataArray.Length];
            for (int i = 0; i < dataArray.Length; ++i)
            {
                result[i] = new IntWrap(dataArray[i]);
            }
            return result;
        }

        private IntWrap[] BuildRandomIntWrapArray(int totalCount)
        {
            IntWrap[] result = new IntWrap[totalCount];
            //Random r = new Random();
            for (int i = 0; i < totalCount; ++i)
            {
                result[i] = new IntWrap(i);
            }
            return result;
        }
    }
}


