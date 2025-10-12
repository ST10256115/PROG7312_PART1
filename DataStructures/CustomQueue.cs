using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServicesApp.DataStructures
{
    public class CustomQueue<T> : IEnumerable<T>
    {
        private T[] _items = new T[8];
        private int _head, _tail, _count;

        public int Count { get { return _count; } }

        public void Enqueue(T item)
        {
            if (_count == _items.Length) Resize();
            _items[_tail] = item;
            _tail = (_tail + 1) % _items.Length;
            _count++;
        }

        public T Dequeue()
        {
            if (_count == 0) throw new InvalidOperationException("Queue empty");
            T val = _items[_head];
            _items[_head] = default(T);
            _head = (_head + 1) % _items.Length;
            _count--;
            return val;
        }

        public T Peek()
        {
            if (_count == 0) throw new InvalidOperationException("Queue empty");
            return _items[_head];
        }

        private void Resize()
        {
            T[] arr = new T[_items.Length * 2];
            for (int i = 0; i < _count; i++) arr[i] = _items[(_head + i) % _items.Length];
            _items = arr;
            _head = 0; _tail = _count;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < _count; i++) yield return _items[(_head + i) % _items.Length];
        }
        IEnumerator IEnumerable.GetEnumerator() { return GetEnumerator(); }
    }
}