using MunicipalServicesApp.Data;
using MunicipalServicesApp.DataStructures;
using System;

namespace MunicipalServicesApp.DataStructures
{
    /// <summary>
    /// Simple undirected weighted graph using custom-built data structures.
    /// Vertices are stored in a CustomSet<T>; edges in a HashTable<T, DynamicArray<Edge<T>>>.
    /// </summary>
    public class Graph<T>
    {
        public class Edge<TV>
        {
            public TV To;
            public double Weight;

            public Edge(TV to, double weight)
            {
                To = to;
                Weight = weight;
            }

            public override string ToString()
            {
                return To + " (" + Weight + ")";
            }
        }

        private readonly CustomSet<T> _vertices;
        private readonly HashTable<T, DynamicArray<Edge<T>>> _adj;

        public Graph()
        {
            _vertices = new CustomSet<T>();
            _adj = new HashTable<T, DynamicArray<Edge<T>>>();
        }

        public void AddVertex(T v)
        {
            if (!_vertices.Contains(v))
            {
                _vertices.Add(v);
                _adj.AddOrUpdate(v, new DynamicArray<Edge<T>>());
            }
        }

        public void AddEdge(T from, T to, double weight = 1.0, bool undirected = true)
        {
            AddVertex(from);
            AddVertex(to);

            DynamicArray<Edge<T>> list;
            if (!_adj.TryGetValue(from, out list))
            {
                list = new DynamicArray<Edge<T>>();
                _adj.AddOrUpdate(from, list);
            }
            list.Add(new Edge<T>(to, weight));

            if (undirected)
            {
                if (!_adj.TryGetValue(to, out list))
                {
                    list = new DynamicArray<Edge<T>>();
                    _adj.AddOrUpdate(to, list);
                }
                list.Add(new Edge<T>(from, weight));
            }
        }

        public T[] Vertices()
        {
            return _vertices.ToArray();
        }

        public Edge<T>[] Neighbors(T v)
        {
            DynamicArray<Edge<T>> list;
            if (_adj.TryGetValue(v, out list))
                return list.ToArray();
            return new Edge<T>[0];
        }

        public bool Contains(T v)
        {
            return _vertices.Contains(v);
        }

        /// <summary>
        /// Breadth-First Search (BFS) traversal — returns path from source to target.
        /// Used for department routing.
        /// </summary>
        public DynamicArray<T> BfsPath(T source, T target)
        {
            var visited = new CustomSet<T>();
            var queue = new CustomQueue<T>();
            var parent = new HashTable<T, T>();

            visited.Add(source);
            queue.Enqueue(source);

            bool found = false;

            while (queue.Count > 0) // <-- updated: use Count instead of IsEmpty
            {
                T cur = queue.Dequeue();
                if (Equals(cur, target))
                {
                    found = true;
                    break;
                }

                var neigh = Neighbors(cur);
                for (int i = 0; i < neigh.Length; i++)
                {
                    var nxt = neigh[i].To;
                    if (!visited.Contains(nxt))
                    {
                        visited.Add(nxt);
                        parent.AddOrUpdate(nxt, cur);
                        queue.Enqueue(nxt);
                    }
                }
            }

            var path = new DynamicArray<T>();
            if (!found) return path; // empty

            // reconstruct backwards
            T node = target;
            while (true)
            {
                path.Insert(0, node);
                T p;
                if (!parent.TryGetValue(node, out p)) break;
                node = p;
            }
            return path;
        }

        /// <summary>
        /// Prim's algorithm for Minimum Spanning Tree (MST).
        /// Returns total weight and list of edges in MST.
        /// </summary>
        public Tuple<double, DynamicArray<Tuple<T, T, double>>> PrimMst(T start)
        {
            var mstEdges = new DynamicArray<Tuple<T, T, double>>();
            var visited = new CustomSet<T>();
            var pq = new PriorityQueue<Tuple<T, T, double>>(
                ComparerFactory.Create<Tuple<T, T, double>>((a, b) => a.Item3.CompareTo(b.Item3))
            );

            visited.Add(start);
            var edges = Neighbors(start);
            for (int i = 0; i < edges.Length; i++)
                pq.Enqueue(new Tuple<T, T, double>(start, edges[i].To, edges[i].Weight));

            double total = 0;

            while (pq.Count > 0)
            {
                var edge = pq.Dequeue();
                T u = edge.Item1;
                T v = edge.Item2;
                double w = edge.Item3;

                if (visited.Contains(v)) continue;
                visited.Add(v);
                total += w;
                mstEdges.Add(edge);

                var neigh = Neighbors(v);
                for (int i = 0; i < neigh.Length; i++)
                {
                    var nxt = neigh[i].To;
                    if (!visited.Contains(nxt))
                        pq.Enqueue(new Tuple<T, T, double>(v, nxt, neigh[i].Weight));
                }
            }

            return new Tuple<double, DynamicArray<Tuple<T, T, double>>>(total, mstEdges);
        }
    }
}
