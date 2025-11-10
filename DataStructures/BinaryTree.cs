using System;
using System.Collections;
using System.Collections.Generic;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// A simple generic binary tree built from scratch.
    /// Supports insertion (left/right), traversals, and enumeration.
    /// </summary>
    public class BinaryTree<T> : IEnumerable<T>
    {
        public class Node
        {
            public T Value;
            public Node Left;
            public Node Right;

            public Node(T value)
            {
                Value = value;
                Left = null;
                Right = null;
            }
        }

        public Node Root { get; private set; }
        public int Count { get; private set; }

        public BinaryTree()
        {
            Root = null;
            Count = 0;
        }

        public Node InsertRoot(T value)
        {
            if (Root != null) throw new InvalidOperationException("Root already exists.");
            Root = new Node(value);
            Count = 1;
            return Root;
        }

        public Node InsertLeft(Node parent, T value)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (parent.Left != null) throw new InvalidOperationException("Left child already exists.");
            parent.Left = new Node(value);
            Count++;
            return parent.Left;
        }

        public Node InsertRight(Node parent, T value)
        {
            if (parent == null) throw new ArgumentNullException(nameof(parent));
            if (parent.Right != null) throw new InvalidOperationException("Right child already exists.");
            parent.Right = new Node(value);
            Count++;
            return parent.Right;
        }

        /// <summary>
        /// Performs an in-order traversal (Left, Root, Right).
        /// </summary>
        public IEnumerable<T> InOrder()
        {
            foreach (var v in InOrder(Root))
                yield return v;
        }

        private IEnumerable<T> InOrder(Node node)
        {
            if (node == null) yield break;
            foreach (var v in InOrder(node.Left)) yield return v;
            yield return node.Value;
            foreach (var v in InOrder(node.Right)) yield return v;
        }

        /// <summary>
        /// Performs a breadth-first traversal (level order).
        /// </summary>
        public IEnumerable<T> LevelOrder()
        {
            if (Root == null) yield break;

            Queue<Node> q = new Queue<Node>();
            q.Enqueue(Root);

            while (q.Count > 0)
            {
                Node n = q.Dequeue();
                yield return n.Value;
                if (n.Left != null) q.Enqueue(n.Left);
                if (n.Right != null) q.Enqueue(n.Right);
            }
        }

        /// <summary>
        /// Clears all nodes.
        /// </summary>
        public void Clear()
        {
            Root = null;
            Count = 0;
        }

        public IEnumerator<T> GetEnumerator() => InOrder().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
