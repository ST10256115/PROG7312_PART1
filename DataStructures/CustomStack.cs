using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServicesApp.DataStructures
{
    public class CustomStack<T> : IEnumerable<T>
    {
        private T[] _items = new T[8];
        private int _count;

        public int Count { get { return _count; } }

        public void Push(T item)
        {
            if (_count == _items.Length) Array.Resize(ref _items, _items.Length * 2);
            _items[_count++] = item;
        }

        public T Pop()
        {
            if (_count == 0) throw new InvalidOperationException("Stack empty");
            T val = _items[--_count];
            _items[_count] = default(T);
            return val;
        }

        public T Peek()
        {
            if (_count == 0) throw new InvalidOperationException("Stack empty");
            return _items[_count - 1];
        }

        public void Clear() { Array.Clear(_items, 0, _count); _count = 0; }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = _count - 1; i >= 0; i--) yield return _items[i];
        }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}