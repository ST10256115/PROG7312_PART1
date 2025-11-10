using System;

namespace MunicipalServicesApp.Models
{
    public class EventItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private string _title = "";
        public string Title
        {
            get { return _title ?? ""; }
            set { _title = value ?? ""; }
        }

        private string _category = "";
        public string Category
        {
            get { return _category ?? ""; }
            set { _category = value ?? ""; }
        }

        private string _location = "";
        public string Location
        {
            get { return _location ?? ""; }
            set { _location = value ?? ""; }
        }

        // Backing field ensures we always keep a UTC-kind DateTime
        private DateTime _startUtc = DateTime.UtcNow;
        public DateTime StartUtc
        {
            get { return DateTime.SpecifyKind(_startUtc, DateTimeKind.Utc); }
            set
            {
                // Preserve the moment in time and mark it as UTC.
                if (value.Kind == DateTimeKind.Utc)
                    _startUtc = value;
                else
                    _startUtc = DateTime.SpecifyKind(value, DateTimeKind.Utc);
            }
        }

        private string _description = "";
        public string Description
        {
            get { return _description ?? ""; }
            set { _description = value ?? ""; }
        }

        public EventItem()
        {
            // default ctor keeps defaults already set above
        }

        /// <summary>
        /// Nicely formatted local-time representation for UI.
        /// </summary>
        public string ToLocalDisplay()
        {
            try
            {
                return StartUtc.ToLocalTime().ToString("dd MMM yyyy HH:mm");
            }
            catch
            {
                return StartUtc.ToString();
            }
        }

        public override string ToString()
        {
            return Title + " — " + ToLocalDisplay() + " (" + Location + ")";
        }
    }
}
