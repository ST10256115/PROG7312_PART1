using System;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Custom, self-balancing AVL tree (generic key/value).
    /// Operations: AddOrUpdate, ContainsKey, TryGetValue, InOrder traversal, Count.
    /// </summary>
    public class AvlTree<TKey, TValue>
    {
        private sealed class Node
        {
            public TKey Key;
            public TValue Value;
            public Node Left;
            public Node Right;
            public int Height;

            public Node(TKey key, TValue value)
            {
                Key = key;
                Value = value;
                Height = 1; // leaf height
            }
        }

        private Node _root;
        private readonly IComparer<TKey> _cmp;
        private int _count;

        public AvlTree()
            : this(Comparer<TKey>.Default)
        { }

        public AvlTree(IComparer<TKey> comparer)
        {
            _cmp = comparer ?? Comparer<TKey>.Default;
        }

        public int Count { get { return _count; } }

        /// <summary>
        /// Add a new key/value. If key exists, merge with updater(oldValue, newValue).
        /// </summary>
        public void AddOrUpdate(TKey key, TValue value, Func<TValue, TValue, TValue> updater)
        {
            if (updater == null) throw new ArgumentNullException(nameof(updater));
            bool added = false;
            _root = Insert(_root, key, value, updater, ref added);
            if (added) _count++;
        }

        public bool ContainsKey(TKey key)
        {
            Node n = _root;
            while (n != null)
            {
                int c = _cmp.Compare(key, n.Key);
                if (c == 0) return true;
                n = (c < 0) ? n.Left : n.Right;
            }
            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            Node n = _root;
            while (n != null)
            {
                int c = _cmp.Compare(key, n.Key);
                if (c == 0)
                {
                    value = n.Value;
                    return true;
                }
                n = (c < 0) ? n.Left : n.Right;
            }
            value = default(TValue);
            return false;
        }

        /// <summary>
        /// In-order traversal (ascending keys).
        /// </summary>
        public void InOrder(Action<TKey, TValue> visitor)
        {
            if (visitor == null) return;
            InOrder(_root, visitor);
        }

        // --------------- internals ---------------

        private static int H(Node n) { return n == null ? 0 : n.Height; }
        private static int Balance(Node n) { return n == null ? 0 : H(n.Left) - H(n.Right); }
        private static void Update(Node n)
        {
            int hl = H(n.Left);
            int hr = H(n.Right);
            n.Height = (hl > hr ? hl : hr) + 1;
        }

        private Node Insert(Node node, TKey key, TValue value, Func<TValue, TValue, TValue> updater, ref bool added)
        {
            if (node == null)
            {
                added = true;
                return new Node(key, value);
            }

            int c = _cmp.Compare(key, node.Key);
            if (c < 0)
            {
                node.Left = Insert(node.Left, key, value, updater, ref added);
            }
            else if (c > 0)
            {
                node.Right = Insert(node.Right, key, value, updater, ref added);
            }
            else
            {
                // Key exists: merge value with updater(old, @new)
                node.Value = updater(node.Value, value);
                return node; // height/balance unchanged
            }

            Update(node);
            return Rebalance(node);
        }

        private Node Rebalance(Node node)
        {
            int bf = Balance(node);

            // Left heavy
            if (bf > 1)
            {
                if (Balance(node.Left) < 0)
                {
                    // LR
                    node.Left = RotateLeft(node.Left);
                }
                // LL
                return RotateRight(node);
            }

            // Right heavy
            if (bf < -1)
            {
                if (Balance(node.Right) > 0)
                {
                    // RL
                    node.Right = RotateRight(node.Right);
                }
                // RR
                return RotateLeft(node);
            }

            return node;
        }

        private static Node RotateRight(Node y)
        {
            // y
            //  x
            //   T2
            Node x = y.Left;
            Node T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            Update(y);
            Update(x);
            return x;
        }

        private static Node RotateLeft(Node x)
        {
            // x
            //   y
            //  T2
            Node y = x.Right;
            Node T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            Update(x);
            Update(y);
            return y;
        }

        private static void InOrder(Node n, Action<TKey, TValue> visitor)
        {
            if (n == null) return;
            InOrder(n.Left, visitor);
            visitor(n.Key, n.Value);
            InOrder(n.Right, visitor);
        }   
    }
}
