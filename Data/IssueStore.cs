using System.Collections.Generic;
using System.IO;
using MunicipalServicesApp.Models;
using Newtonsoft.Json;

namespace MunicipalServicesApp.Data
{
    public static class IssueStore
    {
        private static readonly string DbPath = Path.Combine(
            Directory.GetCurrentDirectory(), "issues.json");

        private static readonly List<Issue> _issues = Load();

        public static IReadOnlyList<Issue> All
        {
            get { return _issues.AsReadOnly(); }
        }

        public static void Add(Issue issue)
        {
            _issues.Add(issue);
            Save();
        }

        private static List<Issue> Load()
        {
            if (!File.Exists(DbPath)) return new List<Issue>();
            var json = File.ReadAllText(DbPath);
            var data = JsonConvert.DeserializeObject<List<Issue>>(json);
            return data ?? new List<Issue>();
        }

        private static void Save()
        {
            var json = JsonConvert.SerializeObject(_issues, Formatting.Indented);
            File.WriteAllText(DbPath, json);
        }
    }
}
