//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;

namespace Qarth
{
    public class LinkedListTest : BaseTestUnit
    {
        public override void StartTest()
        {
            TestInt();
            TestString();
        }

#region String Test
        private void TestString()
        {
            WriteBegin("LinkListTest(String)");
            SCLinkedList<string> list = new SCLinkedList<string>();
            BuildStringLinkedListRandom(list, 0, 10);
            BuildStringLinkedListRandom(list, 11, 20);
            RemoveListAtIndex(list, 19);
            RemoveListAtIndex(list, 0);
            RemoveData(list, "Index:7");
            VisitList(list);
            FindData(list, "Index:9");
            WriteEnd("LinkListTest(String)");
        }

        private void BuildStringLinkedListRandom(SCLinkedList<string> list, int start, int end)
        {
            for (int i = start; i <= end; ++i)
            {
                list.InsertTail(string.Format("Index:{0}", i));
            }
            WriteLine("Build:[{0}:{1}]", start, end);
        }
#endregion

#region Int Test
        private void TestInt()
        {
            WriteBegin("LinkListTest(Int)");
            SCLinkedList<int> list = new SCLinkedList<int>();
            BuildIntLinkedListRandom(list, 0, 10);
            BuildIntLinkedListRandom(list, 11, 20);
            RemoveListAtIndex(list, 19);
            RemoveListAtIndex(list, 0);
            RemoveData(list, 7);
            VisitList(list);
            FindData(list, 9);
            WriteEnd("LinkListTest(Int)");
        }

        private void BuildIntLinkedListRandom(SCLinkedList<int> list, int start, int end)
        {
            for (int i = start; i <= end; ++i)
            {
                list.InsertTail(i);
            }
            WriteLine("Build:[{0}:{1}]", start, end);
        }
#endregion

        private void RemoveListAtIndex<T>(SCLinkedList<T> list, int index)
        {
            WriteLine("Remove At:{0}-Result:{1}", index, list.RemoveAt(index));
        }

        private void RemoveData<T>(SCLinkedList<T> list, T data)
        {
            WriteLine("Remove Data:{0}-Result:{1}", data, list.Remove(data));
        }

        private void VisitList<T>(SCLinkedList<T> list)
        {
            WriteLine("Data Begin:");
            list.Accept(VisitData);
            WriteLine("");
        }

        protected void FindData<T>(SCLinkedList<T> list, T data)
        {
            WriteLine("FindData{0}: Result:{1}", data, list.Query(data));
        }

        protected void VisitData<T>(T data)
        {
            if (data != null)
            {
                Write(string.Format("   {0}", data));
            }
            else
            {
                Write(" NULL ");
            }
        }
    }
}


