using System;

namespace PTGame.Framework
{
    public interface IBinaryHeapElement
    {
        float sortScore
        {
            get;
        }

        int heapIndex
        {
            set;
        }

        void RebuildHeap<T>(BinaryHeap<T> heap) where T : IBinaryHeapElement;
    }
}


