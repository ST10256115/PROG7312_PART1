using System;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    /// <summary>
    /// AVL index of Issues by CreatedAt (ascending). Each key holds a bucket of issues to handle identical timestamps.
    /// </summary>
    public static class IssueAvlIndex
    {
        private static AvlTree<DateTime, DynamicArray<Issue>> _byCreated =
            new AvlTree<DateTime, DynamicArray<Issue>>();

        /// <summary>
        /// Rebuild the index from a snapshot (call on app load).
        /// </summary>
        public static void Rebuild(Issue[] snapshot)
        {
            _byCreated = new AvlTree<DateTime, DynamicArray<Issue>>();
            if (snapshot == null) return;

            for (int i = 0; i < snapshot.Length; i++)
            {
                var it = snapshot[i];
                Add(it);
            }
        }

        /// <summary>
        /// Add one issue to the AVL index (call from IssueStore.Add).
        /// </summary>
        public static void Add(Issue issue)
        {
            if (issue == null) return;
            var key = issue.CreatedAt;

            _byCreated.AddOrUpdate(
                key,
                NewBucket(issue),
                // merge: append to bucket
                delegate (DynamicArray<Issue> oldBucket, DynamicArray<Issue> newBucket)
                {
                    // newBucket has 1 element (the new issue)
                    for (int i = 0; i < newBucket.Count; i++) oldBucket.Add(newBucket[i]);
                    return oldBucket;
                });
        }

        /// <summary>
        /// In-order traversal (ascending by CreatedAt). Returns up to 'max' issues.
        /// </summary>
        public static Issue[] InOrder(int max)
        {
            var outList = new DynamicArray<Issue>();
            if (max < 1) return outList.ToArray();

            int remaining = max;
            _byCreated.InOrder(delegate (DateTime key, DynamicArray<Issue> bucket)
            {
                if (remaining <= 0) return;
                for (int i = 0; i < bucket.Count && remaining > 0; i++)
                {
                    outList.Add(bucket[i]);
                    remaining--;
                }
            });

            return outList.ToArray();
        }

        private static DynamicArray<Issue> NewBucket(Issue it)
        {
            var b = new DynamicArray<Issue>(1);
            b.Add(it);
            return b;
        }
    }
}
