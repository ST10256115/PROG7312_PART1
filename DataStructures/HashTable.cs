using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServicesApp.DataStructures
{
    public class HashTable<TKey, TValue>
    {
        private class Node
        {
            public TKey Key;
            public TValue Value;
            public Node Next;
        }

        private Node[] _buckets = new Node[16];
        private int _count;

        public int Count { get { return _count; } }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Node n = FindNode(key);
            if (n != null) { value = n.Value; return true; }
            value = default(TValue); return false;
        }

        public bool ContainsKey(TKey key) { return FindNode(key) != null; }

        public void AddOrUpdate(TKey key, TValue value)
        {
            int idx = IndexFor(key);
            Node n = _buckets[idx];
            while (n != null)
            {
                if (Equals(n.Key, key)) { n.Value = value; return; }
                n = n.Next;
            }
            Node nn = new Node { Key = key, Value = value, Next = _buckets[idx] };
            _buckets[idx] = nn;
            _count++;
            if (_count > _buckets.Length * 2) Resize();
        }

        public bool Remove(TKey key)
        {
            int idx = IndexFor(key);
            Node prev = null; Node cur = _buckets[idx];
            while (cur != null)
            {
                if (Equals(cur.Key, key))
                {
                    if (prev == null) _buckets[idx] = cur.Next; else prev.Next = cur.Next;
                    _count--; return true;
                }
                prev = cur; cur = cur.Next;
            }
            return false;
        }

        public TKey[] Keys()
        {
            TKey[] arr = new TKey[_count];
            int i = 0;
            for (int b = 0; b < _buckets.Length; b++)
            {
                Node n = _buckets[b];
                while (n != null) { arr[i++] = n.Key; n = n.Next; }
            }
            return arr;
        }

        private Node FindNode(TKey key)
        {
            int idx = IndexFor(key);
            Node n = _buckets[idx];
            while (n != null)
            {
                if (Equals(n.Key, key)) return n;
                n = n.Next;
            }
            return null;
        }

        private int IndexFor(TKey key)
        {
            int h = key == null ? 0 : key.GetHashCode() & 0x7fffffff;
            return h % _buckets.Length;
        }

        private void Resize()
        {
            Node[] old = _buckets;
            _buckets = new Node[old.Length * 2];
            _count = 0;
            for (int i = 0; i < old.Length; i++)
            {
                Node n = old[i];
                while (n != null)
                {
                    AddOrUpdate(n.Key, n.Value);
                    n = n.Next;
                }
            }
        }
    }
}

