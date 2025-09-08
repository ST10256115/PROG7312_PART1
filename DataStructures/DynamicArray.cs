using System;
using System.Collections;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Simple resizable array (custom-built data structure).
    /// Supports Add, RemoveAt, Indexer, Count, ToArray, and foreach.
    /// </summary>
    public class DynamicArray<T> : IEnumerable<T>
    {
        private T[] _items;
        private int _count;

        public DynamicArray() : this(8) { }

        public DynamicArray(int capacity)
        {
            if (capacity < 1) capacity = 1;
            _items = new T[capacity];
            _count = 0;
        }

        public int Count { get { return _count; } }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));
                return _items[index];
            }
            set
            {
                if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));
                _items[index] = value;
            }
        }

        public void Add(T item)
        {
            EnsureCapacity(_count + 1);
            _items[_count++] = item;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));
            for (int i = index; i < _count - 1; i++)
            {
                _items[i] = _items[i + 1];
            }
            _items[_count - 1] = default(T);
            _count--;
        }

        public T[] ToArray()
        {
            var arr = new T[_count];
            Array.Copy(_items, arr, _count);
            return arr;
        }

        private void EnsureCapacity(int desired)
        {
            if (desired <= _items.Length) return;
            int newCap = _items.Length * 2;
            if (newCap < desired) newCap = desired;
            Array.Resize(ref _items, newCap);
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++)
                yield return _items[i];
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
