using System;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Simple binary min-heap built from scratch.
    /// Uses an IComparer<T> supplied via constructor.
    /// Backed by an array; resizes as needed.
    /// </summary>
    public class MinHeap<T>
    {
        private T[] _data;
        private int _count;
        private readonly System.Collections.Generic.IComparer<T> _comparer;

        public MinHeap(System.Collections.Generic.IComparer<T> comparer, int capacity = 16)
        {
            if (comparer == null) throw new ArgumentNullException(nameof(comparer));
            if (capacity < 1) capacity = 1;
            _comparer = comparer;
            _data = new T[capacity];
            _count = 0;
        }

        public int Count { get { return _count; } }

        public void Clear()
        {
            Array.Clear(_data, 0, _count);
            _count = 0;
        }

        public void Enqueue(T item)
        {
            EnsureCapacity(_count + 1);
            _data[_count] = item;
            SiftUp(_count);
            _count++;
        }

        public T Peek()
        {
            if (_count == 0) throw new InvalidOperationException("Heap is empty.");
            return _data[0];
        }

        public T Dequeue()
        {
            if (_count == 0) throw new InvalidOperationException("Heap is empty.");
            T root = _data[0];
            _count--;
            if (_count > 0)
            {
                _data[0] = _data[_count];
                _data[_count] = default(T);
                SiftDown(0);
            }
            else
            {
                _data[0] = default(T);
            }
            return root;
        }

        private void EnsureCapacity(int desired)
        {
            if (desired <= _data.Length) return;
            int newCap = _data.Length * 2;
            if (newCap < desired) newCap = desired;
            Array.Resize(ref _data, newCap);
        }

        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_comparer.Compare(_data[index], _data[parent]) < 0)
                {
                    Swap(index, parent);
                    index = parent;
                }
                else break;
            }
        }

        private void SiftDown(int index)
        {
            while (true)
            {
                int left = index * 2 + 1;
                int right = left + 1;
                int smallest = index;

                if (left < _count && _comparer.Compare(_data[left], _data[smallest]) < 0)
                    smallest = left;
                if (right < _count && _comparer.Compare(_data[right], _data[smallest]) < 0)
                    smallest = right;

                if (smallest == index) break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            T t = _data[i];
            _data[i] = _data[j];
            _data[j] = t;
        }
    }
}
