//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;

namespace Qarth
{
    public class BinarySearchTreeTest : BaseTestUnit
    {
        public class IntWrap : IBinarySearchTreeElement
        {
            private int m_Score;

            public IntWrap(int score)
            {
                m_Score = score;
            }

            public float SortScore
            {
                get { return m_Score; }
            }
        }

        public override void StartTest()
        {
            WriteBegin("BinarySearchTree");
            SCBinarySearchTree<IntWrap> tree = new SCBinarySearchTree<IntWrap>();
            IntWrap[] data = BuildIntWrapArray(new int[] { 0, 1, 2, 3, 4, 5 });//BuildIntWrapArray(new int[] { 10, 5, 8, 4, 18, 12, 20, 21 });
            tree.Insert(data);
            VisitTree(tree);
            TestRemove(tree, data[5]);
            TestRemove(tree, data[4]);
            TestRemove(tree, data[3]);
            TestRemove(tree, data[2]);
            IntWrap[] data2 = BuildIntWrapArray(new int[] { 6, 12, 234, 20, 77, 888, 76 });
            tree.Insert(data2);
            TestIterator(tree);
            WriteEnd("BinarySearchTree");
        }

        protected void TestRemove<T>(SCBinarySearchTree<T> tree, T data) where T : IBinarySearchTreeElement
        {
            WriteLine(" RemoveTest:");
            tree.Remove(data);
        }

        private void TestIterator<T>(SCBinarySearchTree<T> tree) where T : IBinarySearchTreeElement
        {
            WriteBegin("Iterator:");
            var it = tree.Iterator();
            while (it.HasNext)
            {
                T data = it.Next;
                Write("     " + data.SortScore);
            }
            WriteLine("");
            WriteEnd("Iterator:");
        }

        private void VisitTree<T>(SCBinarySearchTree<T> tree) where T : IBinarySearchTreeElement
        {
            WriteLine("Visitor:");
            tree.Accept(VisitorData);
            WriteLine("");
        }

        private void VisitorData<T>(T data) where T : IBinarySearchTreeElement
        {
            Write("     " + data.SortScore);
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
    }
}


