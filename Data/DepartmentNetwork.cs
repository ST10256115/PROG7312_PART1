using System;
using MunicipalServicesApp.Models;
using MunicipalServicesApp.DataStructures;

namespace MunicipalServicesApp.Data
{
    /// <summary>
    /// Concrete department graph + routing helpers.
    /// Uses Graph&lt;string&gt; with BFS to find the shortest path from "Call Centre"
    /// to the responsible department for a given Issue.
    /// </summary>
    public static class DepartmentNetwork
    {
        private static readonly object _sync = new object();
        private static Graph<string> _graph;
        private static bool _built;

        // Canonical node names (kept as constants to avoid typos)
        public const string CallCentre = "Call Centre";
        public const string Triage = "Triage";
        public const string WaterDept = "Water";
        public const string ElectricityDept = "Electricity";
        public const string RoadsDept = "Roads";
        public const string SanitationDept = "Sanitation";
        public const string WasteDept = "Waste";
        public const string SafetyDept = "Public Safety";
        public const string FinanceDept = "Finance";
        public const string FieldOps = "Field Ops";
        public const string QA = "Quality Assurance";
        public const string CloseOut = "Close-Out";

        /// <summary>
        /// Ensure the network graph exists.
        /// </summary>
        private static void EnsureBuilt()
        {
            if (_built) return;
            lock (_sync)
            {
                if (_built) return;

                _graph = new Graph<string>();

                // --- Nodes ---
                _graph.AddVertex(CallCentre);
                _graph.AddVertex(Triage);
                _graph.AddVertex(WaterDept);
                _graph.AddVertex(ElectricityDept);
                _graph.AddVertex(RoadsDept);
                _graph.AddVertex(SanitationDept);
                _graph.AddVertex(WasteDept);
                _graph.AddVertex(SafetyDept);
                _graph.AddVertex(FinanceDept);
                _graph.AddVertex(FieldOps);
                _graph.AddVertex(QA);
                _graph.AddVertex(CloseOut);

                // --- Edges (undirected, uniform weight = 1 for BFS shortest path) ---
                // Intake → triage
                _graph.AddEdge(CallCentre, Triage);
                // Triage connects to service departments
                _graph.AddEdge(Triage, WaterDept);
                _graph.AddEdge(Triage, ElectricityDept);
                _graph.AddEdge(Triage, RoadsDept);
                _graph.AddEdge(Triage, SanitationDept);
                _graph.AddEdge(Triage, WasteDept);
                _graph.AddEdge(Triage, SafetyDept);

                // Depts → Field Ops → QA → Close-Out
                _graph.AddEdge(WaterDept, FieldOps);
                _graph.AddEdge(ElectricityDept, FieldOps);
                _graph.AddEdge(RoadsDept, FieldOps);
                _graph.AddEdge(SanitationDept, FieldOps);
                _graph.AddEdge(WasteDept, FieldOps);
                _graph.AddEdge(SafetyDept, FieldOps);

                _graph.AddEdge(FieldOps, QA);
                _graph.AddEdge(QA, CloseOut);

                // Finance reachable from Close-Out (billings/refunds if needed)
                _graph.AddEdge(CloseOut, FinanceDept);

                _built = true;
            }
        }

        /// <summary>
        /// Map an IssueCategory to the responsible department node.
        /// </summary>
        public static string DepartmentForCategory(IssueCategory cat)
        {
            switch (cat)
            {
                case IssueCategory.Water: return WaterDept;
                case IssueCategory.Electricity: return ElectricityDept;
                case IssueCategory.Roads: return RoadsDept;
                case IssueCategory.Sanitation: return SanitationDept;
                case IssueCategory.Waste: return WasteDept;
                case IssueCategory.Safety: return SafetyDept;
                case IssueCategory.Utilities: return FieldOps; // generic routing
                default: return Triage;   // fallback path via triage
            }
        }

        /// <summary>
        /// Returns the shortest path (as node names) from Call Centre to the target department.
        /// Uses BFS on the custom Graph.
        /// </summary>
        public static string[] RouteToDepartment(IssueCategory category)
        {
            EnsureBuilt();
            var dest = DepartmentForCategory(category);

            var path = _graph.BfsPath(CallCentre, dest);
            return path.ToArray();
        }

        /// <summary>
        /// Returns the full post-triage path: Call Centre → Triage → Dept → Field Ops → QA → Close-Out.
        /// If you only want the intake path, use RouteToDepartment().
        /// </summary>
        public static string[] FullLifecycleRoute(IssueCategory category)
        {
            EnsureBuilt();
            var dest = DepartmentForCategory(category);

            // path 1: intake to department
            var intake = _graph.BfsPath(CallCentre, dest);

            // path 2: department to Close-Out (via Field Ops → QA → Close-Out)
            var toClose = _graph.BfsPath(dest, CloseOut);

            // stitch: intake + (toClose without duplicating 'dest')
            var stitched = new DynamicArray<string>(intake.Count + toClose.Count);
            for (int i = 0; i < intake.Count; i++) stitched.Add(intake[i]);

            bool skipFirst = true;
            for (int i = 0; i < toClose.Count; i++)
            {
                if (skipFirst) { skipFirst = false; continue; }
                stitched.Add(toClose[i]);
            }

            return stitched.ToArray();
        }

        /// <summary>
        /// Expose the graph if needed (read-only usage in UI).
        /// </summary>
        public static Graph<string> Graph
        {
            get { EnsureBuilt(); return _graph; }
        }
    }
}
