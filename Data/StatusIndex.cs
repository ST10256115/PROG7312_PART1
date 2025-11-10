using System;
using System.Collections.Generic;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.DataStructures;

namespace MunicipalServicesApp.Data
{
    /// <summary>
    /// Manages all status timelines using custom binary trees.
    /// Each issue ID maps to its own BinaryTree<StatusNode>.
    /// </summary>
    public static class StatusIndex
    {
        // Map issueId -> BinaryTree<StatusNode>
        private static readonly HashTable<Guid, BinaryTree<StatusNode>> _statusMap =
            new HashTable<Guid, BinaryTree<StatusNode>>();

        /// <summary>
        /// Ensures a timeline tree exists for an issue.
        /// </summary>
        public static BinaryTree<StatusNode> Ensure(Guid issueId)
        {
            BinaryTree<StatusNode> tree;
            if (!_statusMap.TryGetValue(issueId, out tree))
            {
                tree = new BinaryTree<StatusNode>();
                tree.InsertRoot(new StatusNode(IssueStatus.Submitted, DateTime.UtcNow, "Issue submitted."));
                _statusMap.AddOrUpdate(issueId, tree);
            }
            return tree;
        }

        /// <summary>
        /// Adds a new status update if valid.
        /// </summary>
        public static bool AddStatus(Guid issueId, IssueStatus newStatus, string note)
        {
            BinaryTree<StatusNode> tree = Ensure(issueId);

            // get most recent node
            StatusNode last = null;
            foreach (var s in tree.InOrder())
                last = s;

            IssueStatus current = last != null ? last.Status : IssueStatus.Submitted;
            if (!IssueStatusHelper.IsValidTransition(current, newStatus))
                return false;

            // Add to right child of last node (acts like chronological order)
            var node = new StatusNode(newStatus, DateTime.UtcNow, note);
            BinaryTree<StatusNode>.Node parent = FindNode(tree.Root, last);
            if (parent == null)
            {
                // If no previous node, just ensure root
                if (tree.Root == null)
                    tree.InsertRoot(node);
            }
            else
            {
                // Always grow to the right for chronological timeline
                if (parent.Right == null)
                    tree.InsertRight(parent, node);
                else
                    tree.InsertRight(parent.Right, node); // chain
            }

            _statusMap.AddOrUpdate(issueId, tree);
            return true;
        }

        /// <summary>
        /// Retrieves all statuses in order for display.
        /// </summary>
        public static StatusNode[] GetTimeline(Guid issueId)
        {
            BinaryTree<StatusNode> tree;
            if (_statusMap.TryGetValue(issueId, out tree))
            {
                var list = new DynamicArray<StatusNode>();
                foreach (var s in tree.InOrder()) list.Add(s);
                return list.ToArray();
            }
            return new StatusNode[0];
        }

        /// <summary>
        /// Returns a summary suitable for search trees or graphs later.
        /// </summary>
        public static KeyValuePair<Guid, IssueStatus>[] SnapshotLatest()
        {
            var arr = new DynamicArray<KeyValuePair<Guid, IssueStatus>>();
            var keys = _statusMap.Keys();
            for (int i = 0; i < keys.Length; i++)
            {
                BinaryTree<StatusNode> tree;
                if (_statusMap.TryGetValue(keys[i], out tree))
                {
                    StatusNode last = null;
                    foreach (var s in tree.InOrder())
                        last = s;
                    if (last != null)
                        arr.Add(new KeyValuePair<Guid, IssueStatus>(keys[i], last.Status));
                }
            }
            return arr.ToArray();
        }

        // --- helpers ---

        private static BinaryTree<StatusNode>.Node FindNode(BinaryTree<StatusNode>.Node node, StatusNode val)
        {
            if (node == null || val == null) return null;
            if (ReferenceEquals(node.Value, val)) return node;
            var l = FindNode(node.Left, val);
            if (l != null) return l;
            return FindNode(node.Right, val);
        }
    }
}
