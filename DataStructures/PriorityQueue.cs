using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServicesApp.DataStructures
{
    public class PriorityQueue<T>
    {
        private T[] _heap = new T[8];
        private int _count;
        private IComparer<T> _cmp;

        public PriorityQueue(IComparer<T> comparer) { _cmp = comparer; }

        public int Count { get { return _count; } }

        public void Enqueue(T item)
        {
            if (_count == _heap.Length) Array.Resize(ref _heap, _heap.Length * 2);
            _heap[_count] = item;
            SiftUp(_count++);
        }

        public T Dequeue()
        {
            if (_count == 0) throw new InvalidOperationException("Empty heap");
            T root = _heap[0];
            _heap[0] = _heap[--_count];
            _heap[_count] = default(T);
            SiftDown(0);
            return root;
        }

        public T Peek()
        {
            if (_count == 0) throw new InvalidOperationException("Empty heap");
            return _heap[0];
        }

        public void Clear()
        {
            Array.Clear(_heap, 0, _count);
            _count = 0;
        }

        private void SiftUp(int i)
        {
            while (i > 0)
            {
                int p = (i - 1) / 2;
                if (_cmp.Compare(_heap[i], _heap[p]) >= 0) break;
                Swap(i, p);
                i = p;
            }
        }

        private void SiftDown(int i)
        {
            while (true)
            {
                int l = i * 2 + 1, r = l + 1, smallest = i;
                if (l < _count && _cmp.Compare(_heap[l], _heap[smallest]) < 0) smallest = l;
                if (r < _count && _cmp.Compare(_heap[r], _heap[smallest]) < 0) smallest = r;
                if (smallest == i) break;
                Swap(i, smallest);
                i = smallest;
            }
        }

        private void Swap(int a, int b)
        {
            T t = _heap[a]; _heap[a] = _heap[b]; _heap[b] = t;
        }
    }
}