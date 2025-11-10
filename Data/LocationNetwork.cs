using System;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    /// <summary>
    /// Builds an MST across open issue locations using a custom Graph and Prim's algorithm.
    /// </summary>
    public static class LocationNetwork
    {
        // Simple coordinate struct (double precision)
        private struct Coord
        {
            public double X;
            public double Y;
            public Coord(double x, double y) { X = x; Y = y; }
        }

        // Known demo locations → fixed coordinates (arbitrary but consistent for display)
        private static readonly HashTable<string, Coord> _known = BuildKnown();

        /// <summary>
        /// Compute an MST over all OPEN issues (excluding Resolved/Closed/Rejected).
        /// Returns (total length, edges as (fromId, toId, weight)).
        /// If fewer than 2 open issues exist, returns 0 and empty edge list.
        /// </summary>
        public static Tuple<double, DynamicArray<Tuple<Guid, Guid, double>>> BuildOpenIssuesMst()
        {
            // 1) Collect open issues + coordinates
            var all = IssueStore.GetAllSnapshot();
            var ids = new DynamicArray<Guid>();
            var coords = new DynamicArray<Coord>();

            for (int i = 0; i < all.Length; i++)
            {
                var issue = all[i];
                if (IsOpen(issue.Id))
                {
                    ids.Add(issue.Id);
                    coords.Add(Map(issue.Location));
                }
            }

            if (ids.Count < 2)
                return new Tuple<double, DynamicArray<Tuple<Guid, Guid, double>>>(0.0, new DynamicArray<Tuple<Guid, Guid, double>>());

            // 2) Build a complete undirected weighted graph over indices [0..n-1]
            int n = ids.Count;
            var g = new Graph<int>();
            for (int i = 0; i < n; i++) g.AddVertex(i);

            for (int i = 0; i < n; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    double w = Distance(coords[i], coords[j]);
                    g.AddEdge(i, j, w, true);
                }
            }

            // 3) Compute MST starting from node 0
            var mst = g.PrimMst(0);
            double total = mst.Item1;
            var edgesIdx = mst.Item2; // DynamicArray<Tuple<int,int,double>>

            // 4) Map back to Issue IDs
            var edges = new DynamicArray<Tuple<Guid, Guid, double>>(edgesIdx.Count);
            for (int i = 0; i < edgesIdx.Count; i++)
            {
                var e = edgesIdx[i];
                Guid a = ids[e.Item1];
                Guid b = ids[e.Item2];
                edges.Add(new Tuple<Guid, Guid, double>(a, b, e.Item3));
            }

            return new Tuple<double, DynamicArray<Tuple<Guid, Guid, double>>>(total, edges);
        }

        // ---------------- helpers ----------------

        private static bool IsOpen(Guid issueId)
        {
            var tl = StatusIndex.GetTimeline(issueId);
            if (tl == null || tl.Length == 0) return true; // treat as open if no updates
            var last = tl[tl.Length - 1].Status;
            return last != IssueStatus.Closed
                && last != IssueStatus.Resolved
                && last != IssueStatus.Rejected;
        }

        private static double Distance(Coord a, Coord b)
        {
            double dx = a.X - b.X;
            double dy = a.Y - b.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static Coord Map(string location)
        {
            string key = (location ?? "").Trim().ToLowerInvariant();
            if (key.Length == 0) return new Coord(0, 0);

            Coord c;
            if (_known.TryGetValue(key, out c)) return c;

            // Deterministic fallback mapping: simple hash → grid within [0..100]
            int h = SimpleHash(key);
            double x = (h & 0xFF);           // 0..255
            double y = ((h >> 8) & 0xFF);    // 0..255
            // scale into 0..100 range
            x = (x / 255.0) * 100.0;
            y = (y / 255.0) * 100.0;
            return new Coord(x, y);
        }

        private static int SimpleHash(string s)
        {
            // Lightweight, stable hash (case already normalized)
            unchecked
            {
                int h = 23;
                for (int i = 0; i < s.Length; i++)
                    h = h * 31 + s[i];
                return h;
            }
        }

        private static HashTable<string, Coord> BuildKnown()
        {
            var map = new HashTable<string, Coord>();
            map.AddOrUpdate("mamelodi", new Coord(12, 78));
            map.AddOrUpdate("soshanguve", new Coord(18, 66));
            map.AddOrUpdate("soshanguve park", new Coord(20, 64));
            map.AddOrUpdate("atteridgeville", new Coord(8, 54));
            map.AddOrUpdate("town hall", new Coord(50, 50));
            map.AddOrUpdate("cbd", new Coord(52, 48));
            map.AddOrUpdate("hatfield", new Coord(60, 52));
            map.AddOrUpdate("menlyn", new Coord(68, 44));
            map.AddOrUpdate("centurion", new Coord(40, 40));
            map.AddOrUpdate("arcadia", new Coord(56, 56));
            return map;
        }
    }
}
