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
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string Location { get; set; } = "";
        public IssueCategory Category { get; set; }
        public string Description { get; set; } = "";

        // Custom-built data structure instead of List<string>
        public DynamicArray<string> Attachments { get; set; } = new DynamicArray<string>();

        public ContactChannel PreferredChannel { get; set; } = ContactChannel.InApp;
        public string PhoneNumber { get; set; } = "";
    }
}
