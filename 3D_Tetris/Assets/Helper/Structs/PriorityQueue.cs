using System;
using System.Collections.Generic;

namespace Helper.Structs
{
    public class PriorityQueue<T>
    {
        private List<PriorityQueueItem> _binTree = new List<PriorityQueueItem>();

        private class PriorityQueueItem : IComparable<PriorityQueueItem>
        {
            public T obj;
            private int priority;

            private IComparable<PriorityQueueItem> _comparableImplementation;

            public PriorityQueueItem(T obj, int priority)
            {
                this.obj = obj;
                this.priority = priority;
            }

            public int CompareTo(PriorityQueueItem other)
            {
                if (priority > other.priority)
                    return 1;
                else
                    return -1;
            }
        }

        public void Push(T obj, int priority)
        {
            _binTree.Add(new PriorityQueueItem(obj, priority));
            var ci = _binTree.Count - 1;
            while (ci > 0)
            {
                var pi = (ci - 1) / 2;
                if (_binTree[ci].CompareTo(_binTree[pi]) >= 0)
                    break;

                var tmp = _binTree[ci];
                _binTree[ci] = _binTree[pi];
                _binTree[pi] = tmp;
                ci = pi;
            }
        }

        public T Pop()
        {
            // Assumes pq isn't empty
            var li = _binTree.Count - 1;
            var frontItem = _binTree[0];
            _binTree[0] = _binTree[li];
            _binTree.RemoveAt(li);

            --li;
            var pi = 0;
            while (true)
            {
                var ci = pi * 2 + 1;
                if (ci > li) break;
                var rc = ci + 1;
                if (rc <= li && _binTree[rc].CompareTo(_binTree[ci]) < 0)
                    ci = rc;
                if (_binTree[pi].CompareTo(_binTree[ci]) <= 0) break;
                var tmp = _binTree[pi];
                _binTree[pi] = _binTree[ci];
                _binTree[ci] = tmp;
                pi = ci;
            }

            return frontItem.obj;
        }
    }
}