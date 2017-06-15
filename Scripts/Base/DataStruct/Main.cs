//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;

namespace Qarth
{
    public class TestEnter : BaseTestUnit
    {
        public static int Main()
        {
            TestEnter enter = new TestEnter();
            enter.StartTest();
            return 0;
        }

        public override void StartTest()
        {
            //TestLinkList();
            //TestBinaryHeap();
            TestBinarySearchTree();
        }

        protected void TestLinkList()
        {
            ITestUnit unit = new LinkedListTest();
            unit.StartTest();
        }

        protected void TestBinaryHeap()
        {
            ITestUnit unit = new BinaryHeapTest();
            unit.StartTest();
        }

        protected void TestBinarySearchTree()
        {
            ITestUnit unit = new BinarySearchTreeTest();
            unit.StartTest();
        }
    }
}


