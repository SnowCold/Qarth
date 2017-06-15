//  Desc:        Framework For Game Develop with Unity3d
//  Copyright:   Copyright (C) 2017 SnowCold. All rights reserved.
//  WebSite:     https://github.com/SnowCold/Qarth
//  Blog:        http://blog.csdn.net/snowcoldgame
//  Author:      SnowCold
//  E-mail:      snowcold.ouyang@gmail.com
using System;

namespace Qarth
{
    //链表
    public class SCLinkedList<T> : ISCList<T>, Iteratable<T>
    {
        private ListNode<T> m_HeadNode;

        public SCLinkedList()
        {

        }

        protected ListNode<T> HeadNode
        {
            get { return m_HeadNode; }
        }

        //获取队尾
        protected ListNode<T> TailNode
        {
            get
            {
                if (m_HeadNode == null)
                {
                    return null;
                }

                ListNode<T> nextNode = m_HeadNode;
                while (nextNode.Next != null)
                {
                    nextNode = nextNode.Next;
                }

                return nextNode;
            }
        }
        
#region 公共方法
        
#region 插入方法
        //插入队列头
        public void InsertHead(T data)
        {
            var preHead = m_HeadNode;

            m_HeadNode = new ListNode<T>();
            m_HeadNode.Data = data;

            m_HeadNode.Next = preHead;
        }

        //插入队列尾
        public void InsertTail(T data)
        {
            var preTail = TailNode;

            ListNode<T> tail = new ListNode<T>();
            tail.Data = data;

            if (preTail == null)
            {
                m_HeadNode = tail;
            }
            else
            {
                preTail.Next = tail;
            }
        }
#endregion

#region 删除方法
        public void RemoveHead()
        {
            if (m_HeadNode == null)
            {
                return;
            }

            m_HeadNode = m_HeadNode.Next;
        }

        public bool RemoveAt(int index)
        {
            if (m_HeadNode == null)
            {
                return false;
            }

            ListNode<T> preNode = null;
            ListNode<T> currentNode = m_HeadNode;

            while (index-- > 0 && currentNode != null)
            {
                preNode = currentNode;
                currentNode = preNode.Next;
            }

            if (currentNode == null)
            {
                return false;
            }

            //删除的是头结点
            if (preNode == null)
            {
                m_HeadNode = currentNode.Next;
            }
            else
            {
                preNode.Next = currentNode.Next;
            }
            return true;
        }

        public bool Remove(T data)
        {
            if (m_HeadNode == null)
            {
                return false;
            }

            ListNode<T> preNode = null;
            ListNode<T> currentNode = m_HeadNode;
            bool hasFind = false;

            while (currentNode != null)
            {
                if (currentNode.Data.Equals(data))
                {
                    hasFind = true;
                    break;
                }

                preNode = currentNode;
                currentNode = currentNode.Next;
            }

            if (!hasFind)
            {
                return false;
            }

            //删除的是头结点
            if (preNode == null)
            {
                m_HeadNode.Next = currentNode.Next;
            }
            else
            {
                preNode.Next = currentNode.Next;
            }
            return true;
        }
#endregion

#region 查询方法
        //查询方法，返回索引
        public int Query(T data)
        {
            if (m_HeadNode == null)
            {
                return -1;
            }

            ListNode<T> currentNode = m_HeadNode;
            int index = 0;
            while (currentNode != null)
            {
                if (currentNode.Data.Equals(data))
                {
                    return index;
                }
                currentNode = currentNode.Next;
                ++index;
            }

            return -1;
        }
#endregion

#region 获取对头

        public T HeadData
        {
            get
            {
                if (m_HeadNode == null)
                {
                    return default(T);
                }
                return m_HeadNode.Data;
            }
        }

        public T TailData
        {
            get
            {
                var tailHead = TailNode;
                if (tailHead == null)
                {
                    return default(T);
                }
                return tailHead.Data;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return m_HeadNode == null;
            }
        }
#endregion

#region 访问器遍历
        public void Accept(IListVisitor<T> visitor)
        {
            var it = Iterator();
            while (it.HasNext)
            {
                visitor.Visit(it.Next);
            }
        }

        public void Accept(ListVisitorDelegate<T> visitor)
        {
            var it = Iterator();
            while (it.HasNext)
            {
                visitor(it.Next);
            }
        }
#endregion

#region 迭代器实现
        public class LinkedListIterator : Iterator<T>
        {
            private ListNode<T> m_HeadNode;
            private ListNode<T> m_CurrentNode;

            public LinkedListIterator(ListNode<T> head)
            {
                m_HeadNode = head;
                if (m_HeadNode != null)
                {
                    m_CurrentNode = new ListNode<T>();
                    m_CurrentNode.Next = m_HeadNode;
                }
            }

            public bool HasNext
            {
                get
                {
                    return m_CurrentNode.Next != null;
                }
            }

            public T Next
            {
                get
                {
                    T r = m_CurrentNode.Next.Data;
                    m_CurrentNode = m_CurrentNode.Next;
                    return r;
                }
            }
        }

        //该链表的迭代器
        public Iterator<T> Iterator()
        {
            return new LinkedListIterator(m_HeadNode);
        }

#endregion

#endregion
    }
}


