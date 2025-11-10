using System;
using System.IO;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;
using Newtonsoft.Json;

namespace MunicipalServicesApp.Data
{
    public static class EventStore
    {
        private static readonly string DbPath = Path.Combine(Directory.GetCurrentDirectory(), "events.json");
        private static readonly string BakPath = Path.Combine(Directory.GetCurrentDirectory(), "events.json.bak");
        private static readonly string TmpPath = Path.Combine(Directory.GetCurrentDirectory(), "events.json.tmp");

        // Core data structures
        private static readonly HashTable<string, DynamicArray<EventItem>> _byCategory =
            new HashTable<string, DynamicArray<EventItem>>();
        private static readonly CustomSet<string> _categories = new CustomSet<string>();
        private static readonly PriorityQueue<EventItem> _upcoming =
            new PriorityQueue<EventItem>(ComparerFactory.Create<EventItem>((a, b) => DateTime.Compare(a.StartUtc, b.StartUtc)));

        // Engagement DS: track searches (stack for recent, queue for to-process)
        private static readonly CustomStack<string> _searchHistory = new CustomStack<string>();
        private static readonly CustomQueue<string> _searchQueue = new CustomQueue<string>();
        private static readonly HashTable<string, int> _searchCounts = new HashTable<string, int>(); // for recommendations

        // light sync for search counters (WinForms is mostly single-threaded, but be safe)
        private static readonly object _sync = new object();

        static EventStore()
        {
            LoadOrSeed();
            RebuildIndexes();
        }

        public static string[] Categories()
        {
            return _categories.ToArray();
        }

        public static EventItem[] Upcoming(int max = 20)
        {
            // Copy to a temp heap so we don't mutate the main one
            var temp = new PriorityQueue<EventItem>(
                ComparerFactory.Create<EventItem>((a, b) => DateTime.Compare(a.StartUtc, b.StartUtc)));

            EventItem[] all = AllEvents();
            for (int i = 0; i < all.Length; i++) temp.Enqueue(all[i]);

            int n = Math.Min(max, temp.Count);
            EventItem[] arr = new EventItem[n];
            for (int i = 0; i < n; i++) arr[i] = temp.Dequeue();
            return arr;
        }

        public static EventItem[] Search(string query, string category, DateTime? from, DateTime? to)
        {
            string q = (query ?? "").Trim().ToLowerInvariant();
            string cat = (category ?? "").Trim();

            // Track searches for recommendations (thread-safe)
            if (!string.IsNullOrEmpty(q)) TrackSearch(q);
            if (!string.IsNullOrEmpty(cat)) TrackSearch("cat:" + cat.ToLowerInvariant());

            // Normalize date bounds to UTC once
            DateTime? fUtc = from.HasValue ? (DateTime?)from.Value.ToUniversalTime() : null;
            DateTime? tUtc = to.HasValue ? (DateTime?)to.Value.ToUniversalTime() : null;

            DynamicArray<EventItem> pool;

            if (!string.IsNullOrEmpty(cat) && _byCategory.TryGetValue(cat, out pool))
            {
                // use filtered pool
            }
            else
            {
                // merge all categories
                pool = new DynamicArray<EventItem>();
                string[] cats = _byCategory.Keys();
                for (int c = 0; c < cats.Length; c++)
                {
                    DynamicArray<EventItem> bucket;
                    if (_byCategory.TryGetValue(cats[c], out bucket))
                    {
                        foreach (var ev in bucket) pool.Add(ev);
                    }
                }
            }

            // min-heap by date to return in chronological order
            var heap = new PriorityQueue<EventItem>(
                ComparerFactory.Create<EventItem>((a, b) => DateTime.Compare(a.StartUtc, b.StartUtc)));

            foreach (var ev in pool)
            {
                if (!string.IsNullOrEmpty(q))
                {
                    string blob = (ev.Title + " " + ev.Description + " " + ev.Location).ToLowerInvariant();
                    if (blob.IndexOf(q, StringComparison.Ordinal) < 0) continue;
                }
                if (fUtc.HasValue && ev.StartUtc < fUtc.Value) continue;
                if (tUtc.HasValue && ev.StartUtc > tUtc.Value) continue;

                heap.Enqueue(ev);
            }

            int count = heap.Count;
            EventItem[] result = new EventItem[count];
            for (int i = 0; i < count; i++) result[i] = heap.Dequeue();
            return result;
        }

        public static string[] RecommendTopCategories(int max = 3)
        {
            // snapshot keys
            string[] keys = _searchCounts.Keys();
            int n = keys.Length;
            if (n == 0) return new string[0];

            // simple selection sort by descending count
            for (int i = 0; i < n - 1; i++)
            {
                for (int j = i + 1; j < n; j++)
                {
                    int ci = GetCount(keys[i]);
                    int cj = GetCount(keys[j]);
                    if (cj > ci)
                    {
                        string tmp = keys[i];
                        keys[i] = keys[j];
                        keys[j] = tmp;
                    }
                }
            }

            // return only category tokens (prefixed with "cat:")
            DynamicArray<string> outs = new DynamicArray<string>();
            for (int i = 0; i < n && outs.Count < max; i++)
            {
                if (keys[i].StartsWith("cat:"))
                {
                    outs.Add(keys[i].Substring(4));
                }
            }
            return outs.ToArray();
        }

        // ---------------- internal helpers ----------------

        private static void TrackSearch(string token)
        {
            lock (_sync)
            {
                _searchHistory.Push(token);
                _searchQueue.Enqueue(token);

                int current;
                if (_searchCounts.TryGetValue(token, out current)) _searchCounts.AddOrUpdate(token, current + 1);
                else _searchCounts.AddOrUpdate(token, 1);
            }
        }

        private static int GetCount(string key)
        {
            int c;
            if (_searchCounts.TryGetValue(key, out c)) return c;
            return 0;
        }

        private static EventItem[] AllEvents()
        {
            string[] cats = _byCategory.Keys();
            DynamicArray<EventItem> all = new DynamicArray<EventItem>();
            for (int i = 0; i < cats.Length; i++)
            {
                DynamicArray<EventItem> list;
                if (_byCategory.TryGetValue(cats[i], out list))
                {
                    foreach (var ev in list) all.Add(ev);
                }
            }
            return all.ToArray();
        }

        private static void RebuildIndexes()
        {
            _upcoming.Clear();
            string[] cats = _byCategory.Keys();
            for (int i = 0; i < cats.Length; i++)
            {
                DynamicArray<EventItem> list;
                if (_byCategory.TryGetValue(cats[i], out list))
                {
                    foreach (var ev in list)
                    {
                        _upcoming.Enqueue(ev);
                        _categories.Add(ev.Category);
                    }
                }
            }
        }

        private static void LoadOrSeed()
        {
            // 1) Try main file
            if (File.Exists(DbPath))
            {
                try
                {
                    var json = File.ReadAllText(DbPath);
                    var arr = JsonConvert.DeserializeObject<EventItem[]>(json);
                    if (arr != null) { Ingest(arr); return; }
                }
                catch { /* try backup */ }
            }

            // 2) Try backup file
            if (File.Exists(BakPath))
            {
                try
                {
                    var jsonBak = File.ReadAllText(BakPath);
                    var arrBak = JsonConvert.DeserializeObject<EventItem[]>(jsonBak);
                    if (arrBak != null) { Ingest(arrBak); return; }
                }
                catch { /* fall through to seed */ }
            }

            // 3) Seed demo events then write atomically
            var seed = new EventItem[]
            {
                new EventItem{ Title="Planned Water Interruption - Ward 7", Category="Utilities", Location="Mamelodi",      StartUtc=DateTime.UtcNow.AddDays(2).AddHours(3), Description="Maintenance on main line."},
                new EventItem{ Title="Community Clean-up",                   Category="Community", Location="Soshanguve Park", StartUtc=DateTime.UtcNow.AddDays(1).AddHours(1), Description="Bring gloves and bags."},
                new EventItem{ Title="Public Safety Meeting",                 Category="Safety",    Location="Town Hall",      StartUtc=DateTime.UtcNow.AddDays(3).AddHours(2), Description="Ward councillor Q&A."},
                new EventItem{ Title="Road Repair Notice",                    Category="Roads",     Location="CBD",            StartUtc=DateTime.UtcNow.AddDays(1).AddHours(6), Description="Lane closures between 09:00–15:00."},
                new EventItem{ Title="Electricity Planned Outage",            Category="Utilities", Location="Atteridgeville", StartUtc=DateTime.UtcNow.AddDays(4),             Description="Substation maintenance."},
            };

            Ingest(seed);
            try
            {
                var jsonSeed = JsonConvert.SerializeObject(seed, Formatting.Indented);
                WriteAtomic(jsonSeed);
            }
            catch { /* best effort for seed write */ }
        }

        private static void Ingest(EventItem[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                var ev = items[i];
                DynamicArray<EventItem> list;
                if (!_byCategory.TryGetValue(ev.Category, out list))
                {
                    list = new DynamicArray<EventItem>();
                    _byCategory.AddOrUpdate(ev.Category, list);
                }
                list.Add(ev);
            }
        }

        private static void WriteAtomic(string json)
        {
            // 1) write tmp
            File.WriteAllText(TmpPath, json);

            // 2) backup current
            if (File.Exists(DbPath))
            {
                try { File.Copy(DbPath, BakPath, true); } catch { /* best effort */ }
            }

            // 3) replace
            File.Copy(TmpPath, DbPath, true);

            // 4) clean tmp
            try { if (File.Exists(TmpPath)) File.Delete(TmpPath); } catch { /* ignore */ }
        }
    }

    // Small helper for IComparer without lambdas in .NET Framework
    internal static class ComparerFactory
    {
        private class C<T> : System.Collections.Generic.IComparer<T>
        {
            private Func<T, T, int> _f;
            public C(Func<T, T, int> f) { _f = f; }
            public int Compare(T x, T y) { return _f(x, y); }
        }
        public static System.Collections.Generic.IComparer<T> Create<T>(Func<T, T, int> f) { return new C<T>(f); }
    }
}
