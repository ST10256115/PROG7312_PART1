using System;
using System.Collections;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Simple resizable array (custom-built data structure).
    /// Supports Add, RemoveAt, Indexer, Count, ToArray, and foreach.
    /// Additional utility methods: Insert, Clear, Contains, IndexOf, AddRange, TrimExcess, CopyTo.
    /// Not thread-safe.
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

        /// <summary>Number of items stored.</summary>
        public int Count { get { return _count; } }

        /// <summary>Current underlying array capacity.</summary>
        public int Capacity
        {
            get { return _items.Length; }
            set
            {
                if (value < _count) throw new ArgumentOutOfRangeException(nameof(value), "Capacity cannot be set smaller than Count.");
                if (value != _items.Length) Array.Resize(ref _items, value);
            }
        }

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

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null) return;
            // Try to optimize if it's a collection with Count
            var col = items as ICollection<T>;
            if (col != null && col.Count > 0)
            {
                EnsureCapacity(_count + col.Count);
                col.CopyTo(_items, _count);
                _count += col.Count;
                return;
            }

            foreach (var it in items) Add(it);
        }

        public void Insert(int index, T item)
        {
            if (index < 0 || index > _count) throw new ArgumentOutOfRangeException(nameof(index));
            EnsureCapacity(_count + 1);
            if (index < _count)
            {
                Array.Copy(_items, index, _items, index + 1, _count - index);
            }
            _items[index] = item;
            _count++;
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= _count) throw new ArgumentOutOfRangeException(nameof(index));
            if (index < _count - 1)
            {
                Array.Copy(_items, index + 1, _items, index, _count - index - 1);
            }
            _items[_count - 1] = default(T);
            _count--;
        }

        public void Clear()
        {
            if (_count == 0) return;
            Array.Clear(_items, 0, _count);
            _count = 0;
        }

        public bool Contains(T item)
        {
            return IndexOf(item) >= 0;
        }

        public int IndexOf(T item)
        {
            var comparer = EqualityComparer<T>.Default;
            for (int i = 0; i < _count; i++)
                if (comparer.Equals(_items[i], item)) return i;
            return -1;
        }

        public T[] ToArray()
        {
            var arr = new T[_count];
            if (_count > 0) Array.Copy(_items, arr, _count);
            return arr;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (arrayIndex < 0 || arrayIndex > array.Length) throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            if (array.Length - arrayIndex < _count) throw new ArgumentException("Destination array is too small.");
            if (_count > 0) Array.Copy(_items, 0, array, arrayIndex, _count);
        }

        /// <summary>Reduce underlying array to match Count (optional optimization).</summary>
        public void TrimExcess()
        {
            if (_count < _items.Length) Array.Resize(ref _items, Math.Max(_count, 1));
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
            // Capture a snapshot of array and count so enumeration won't fail if the internal array is resized.
            var snapshot = _items;
            int n = _count;
            for (int i = 0; i < n; i++)
                yield return snapshot[i];
        }

        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}
