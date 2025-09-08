using System;
using System.IO;
using MunicipalServicesApp.DataStructures;
using MunicipalServicesApp.Models;
using Newtonsoft.Json;

namespace MunicipalServicesApp.Data
{
    public static class IssueStore
    {
        private static readonly string DbPath = Path.Combine(Directory.GetCurrentDirectory(), "issues.json");

        // In-memory custom data structure
        private static readonly DynamicArray<Issue> _issues = Load();

        /// <summary>
        /// Snapshot for safe enumeration in UI.
        /// </summary>
        public static Issue[] GetAllSnapshot()
        {
            return _issues.ToArray();
        }

        public static void Add(Issue issue)
        {
            _issues.Add(issue);
            Save();
        }

        // ---------------------- Persistence ----------------------

        private static DynamicArray<Issue> Load()
        {
            var result = new DynamicArray<Issue>();

            if (!File.Exists(DbPath))
                return result;

            try
            {
                var json = File.ReadAllText(DbPath);

                // Deserialize to DTOs that use arrays (JSON-friendly)
                var dtos = JsonConvert.DeserializeObject<IssueDto[]>(json);
                if (dtos == null) return result;

                for (int i = 0; i < dtos.Length; i++)
                {
                    var dto = dtos[i];
                    var issue = new Issue
                    {
                        Id = dto.Id,
                        CreatedAt = dto.CreatedAt,
                        Location = dto.Location ?? "",
                        Category = dto.Category,
                        Description = dto.Description ?? "",
                        PreferredChannel = dto.PreferredChannel,
                        PhoneNumber = dto.PhoneNumber ?? ""
                    };

                    // Map array -> DynamicArray<string>
                    var atts = new DynamicArray<string>();
                    if (dto.Attachments != null)
                    {
                        for (int a = 0; a < dto.Attachments.Length; a++)
                        {
                            var path = dto.Attachments[a];
                            if (!string.IsNullOrEmpty(path))
                                atts.Add(path);
                        }
                    }
                    issue.Attachments = atts;

                    result.Add(issue);
                }
            }
            catch
            {
                // If the existing JSON is incompatible, we fail soft and start fresh.
                // (You can delete issues.json to reset.)
            }

            return result;
        }

        private static void Save()
        {
            // Map Issues -> DTOs with string[] for attachments
            var issues = _issues.ToArray();
            var dtos = new IssueDto[issues.Length];
            for (int i = 0; i < issues.Length; i++)
            {
                var issue = issues[i];
                dtos[i] = new IssueDto
                {
                    Id = issue.Id,
                    CreatedAt = issue.CreatedAt,
                    Location = issue.Location,
                    Category = issue.Category,
                    Description = issue.Description,
                    Attachments = issue.Attachments != null ? issue.Attachments.ToArray() : new string[0],
                    PreferredChannel = issue.PreferredChannel,
                    PhoneNumber = issue.PhoneNumber
                };
            }

            var json = JsonConvert.SerializeObject(dtos, Formatting.Indented);
            File.WriteAllText(DbPath, json);
        }

        // ---------------------- DTO (file shape) ----------------------
        // Kept private to this store to avoid leaking JSON concerns elsewhere.
        private class IssueDto
        {
            public Guid Id { get; set; }
            public DateTime CreatedAt { get; set; }
            public string Location { get; set; }
            public IssueCategory Category { get; set; }
            public string Description { get; set; }
            public string[] Attachments { get; set; }
            public ContactChannel PreferredChannel { get; set; }
            public string PhoneNumber { get; set; }
        }
    }
}
