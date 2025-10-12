using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MunicipalServicesApp.Models
{
    public class EventItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = "";
        public string Category { get; set; } = "";
        public string Location { get; set; } = "";
        public DateTime StartUtc { get; set; } = DateTime.UtcNow;
        public string Description { get; set; } = "";

        public override string ToString()
        {
            return Title + " — " + StartUtc.ToLocalTime().ToString("dd MMM yyyy HH:mm") + " (" + Location + ")";
        }
    }
}

