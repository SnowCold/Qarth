using System;

namespace PTGame.Framework
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


