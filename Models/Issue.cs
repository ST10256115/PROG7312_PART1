using System;
using MunicipalServicesApp.DataStructures;

namespace MunicipalServicesApp.Models
{
    public enum IssueCategory
    {
        Sanitation, Roads, Electricity, Water, Waste, Utilities, Safety
    }

    public enum ContactChannel
    {
        None, SMS, WhatsApp, InApp
    }

    public class Issue
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Use UTC to avoid timezone surprises; mark kind explicitly.
        private DateTime _createdAt = DateTime.UtcNow;
        public DateTime CreatedAt
        {
            get { return DateTime.SpecifyKind(_createdAt, DateTimeKind.Utc); }
            set
            {
                if (value.Kind == DateTimeKind.Utc) _createdAt = value;
                else _createdAt = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        private string _location = "";
        public string Location
        {
            get { return _location ?? ""; }
            set { _location = value ?? ""; }
        }

        public IssueCategory Category { get; set; }

        private string _description = "";
        public string Description
        {
            get { return _description ?? ""; }
            set { _description = value ?? ""; }
        }

        // Custom-built data structure instead of List<string>
        private DynamicArray<string> _attachments = new DynamicArray<string>();
        public DynamicArray<string> Attachments
        {
            get { return _attachments ?? (_attachments = new DynamicArray<string>()); }
            set { _attachments = value ?? new DynamicArray<string>(); }
        }

        public ContactChannel PreferredChannel { get; set; } = ContactChannel.InApp;

        private string _phoneNumber = "";
        public string PhoneNumber
        {
            get { return _phoneNumber ?? ""; }
            set { _phoneNumber = value ?? ""; }
        }

        public Issue()
        {
            // defaults initialized above
        }

        /// <summary>
        /// Friendly one-line summary used in lists/logs.
        /// </summary>
        public string Summary()
        {
            try
            {
                var when = CreatedAt.ToLocalTime().ToString("dd MMM yyyy HH:mm");
                return $"{Id} — {when} — {Category} — {Location}";
            }
            catch
            {
                return Id.ToString();
            }
        }
    }
}
