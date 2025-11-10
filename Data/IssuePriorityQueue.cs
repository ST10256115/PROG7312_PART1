using System;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp.Data
{
    /// <summary>
    /// Feeds a custom MinHeap<Issue> ordered by CreatedAt (oldest first),
    /// with an option to include only "open" issues (based on StatusIndex).
    /// </summary>
    public static class IssuePriorityQueue
    {
        // Compare by CreatedAt ascending, tie-break by Id
        private sealed class IssueCreatedAtComparer : System.Collections.Generic.IComparer<Issue>
        {
            public int Compare(Issue a, Issue b)
            {
                if (ReferenceEquals(a, b)) return 0;
                if (a == null) return -1;
                if (b == null) return 1;

                int c = a.CreatedAt.CompareTo(b.CreatedAt);
                if (c != 0) return c;
                return a.Id.CompareTo(b.Id);
            }
        }

        /// <summary>
        /// Build a heap from the current IssueStore snapshot.
        /// </summary>
        public static MinHeap<Issue> BuildHeap(bool onlyOpen = true)
        {
            var heap = new MinHeap<Issue>(new IssueCreatedAtComparer(), 32);
            var issues = IssueStore.GetAllSnapshot();

            for (int i = 0; i < issues.Length; i++)
            {
                var issue = issues[i];

                // Make sure there's at least a Submitted node in the status tree
                StatusIndex.Ensure(issue.Id);

                if (!onlyOpen || IsOpen(issue.Id))
                    heap.Enqueue(issue);
            }

            return heap;
        }

        /// <summary>
        /// Returns the K oldest "open" issues (CreatedAt ascending).
        /// </summary>
        public static Issue[] OldestOpen(int k)
        {
            if (k <= 0) return new Issue[0];

            var heap = BuildHeap(true);
            int take = Math.Min(k, heap.Count);

            var outArr = new DynamicArray<Issue>(take);
            for (int i = 0; i < take; i++)
                outArr.Add(heap.Dequeue());

            return outArr.ToArray();
        }

        // ----------------- helpers -----------------

        private static bool IsOpen(Guid issueId)
        {
            // If there is no timeline or last state is not a terminal (Closed/Resolved/Rejected), consider it open.
            var tl = StatusIndex.GetTimeline(issueId);
            if (tl == null || tl.Length == 0) return true;

            var last = tl[tl.Length - 1].Status;
            return last != IssueStatus.Closed
                && last != IssueStatus.Resolved
                && last != IssueStatus.Rejected;
        }
    }
}
