using System;

namespace MunicipalServicesApp.Models
{
    /// <summary>
    /// A single checkpoint in an Issue's lifecycle.
    /// This is the payload we place inside our BinaryTree for the status timeline.
    /// </summary>
    public class StatusNode
    {
        public IssueStatus Status { get; set; }

        /// <summary>
        /// Timestamp stored as UTC for consistency across machines.
        /// </summary>
        private DateTime _timestampUtc = DateTime.UtcNow;
        public DateTime TimestampUtc
        {
            get { return DateTime.SpecifyKind(_timestampUtc, DateTimeKind.Utc); }
            set
            {
                if (value.Kind == DateTimeKind.Utc) _timestampUtc = value;
                else _timestampUtc = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        /// <summary>
        /// Optional free-text note (who updated, brief reason, etc.).
        /// </summary>
        private string _note = "";
        public string Note
        {
            get { return _note ?? ""; }
            set { _note = value ?? ""; }
        }

        public StatusNode() { }

        public StatusNode(IssueStatus status, DateTime whenUtc, string note)
        {
            Status = status;
            TimestampUtc = whenUtc;
            Note = note ?? "";
        }

        public static StatusNode Now(IssueStatus status, string note)
        {
            return new StatusNode(status, DateTime.UtcNow, note);
        }

        public override string ToString()
        {
            return Status + " @ " + TimestampUtc.ToLocalTime().ToString("dd MMM yyyy HH:mm");
        }
    }
}
