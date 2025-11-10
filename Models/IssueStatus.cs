using System;
using MunicipalServicesApp.DataStructures;

namespace MunicipalServicesApp.Models
{
    /// <summary>
    /// Lifecycle states for a municipal service request.
    /// </summary>
    public enum IssueStatus
    {
        Submitted = 0,
        Triaged = 1,
        Assigned = 2,
        InProgress = 3,
        Resolved = 4,
        Closed = 5,
        Rejected = 6
    }

    /// <summary>
    /// Helpers for status flows (used by Status page & trees).
    /// </summary>
    public static class IssueStatusHelper
    {
        /// <summary>
        /// The default forward flow used in the UI timeline.
        /// </summary>
        public static DynamicArray<IssueStatus> DefaultFlow()
        {
            var arr = new DynamicArray<IssueStatus>(6);
            arr.Add(IssueStatus.Submitted);
            arr.Add(IssueStatus.Triaged);
            arr.Add(IssueStatus.Assigned);
            arr.Add(IssueStatus.InProgress);
            arr.Add(IssueStatus.Resolved);
            arr.Add(IssueStatus.Closed);
            return arr;
        }

        /// <summary>
        /// Returns true if moving from current -> next is allowed in the normal flow.
        /// Rejections are allowed from any state except Closed/Rejected.
        /// </summary>
        public static bool IsValidTransition(IssueStatus current, IssueStatus next)
        {
            if (current == IssueStatus.Rejected || current == IssueStatus.Closed)
                return false;

            if (next == IssueStatus.Rejected)
                return true;

            // normal forward steps
            switch (current)
            {
                case IssueStatus.Submitted: return next == IssueStatus.Triaged;
                case IssueStatus.Triaged: return next == IssueStatus.Assigned;
                case IssueStatus.Assigned: return next == IssueStatus.InProgress;
                case IssueStatus.InProgress: return next == IssueStatus.Resolved;
                case IssueStatus.Resolved: return next == IssueStatus.Closed;
                default: return false;
            }
        }
    }
}
