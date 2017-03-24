using System;

namespace PTGame.Framework
{
    public interface ISCList<T>
    {
        void Accept(IListVisitor<T> visitor);
        void Accept(ListVisitorDelegate<T> visitor);
    }

    //列表访问器
    public interface IListVisitor<T>
    {
        void Visit(T data);
    }

    public delegate void ListVisitorDelegate<T>(T data);

    public interface Iterator<T>
    {
        bool HasNext
        {
            get;
        }

        T Next
        {
            get;
        }
    }

    public interface Iteratable<T>
    {
        Iterator<T> Iterator(); 
    }

    public class ListNode<T>
    {
        private T m_Data;
        ListNode<T> m_Next;
        
        public T Data
        {
            get { return m_Data; }
            set { m_Data = value; }
        }

        public ListNode<T> Next
        {
            get { return m_Next; }
            set { m_Next = value; }
        }
    }
}


