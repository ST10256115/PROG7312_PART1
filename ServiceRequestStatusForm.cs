using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp
{
    public partial class ServiceRequestStatusForm : Form
    {
        // Theme
        private static readonly Color Primary = Color.FromArgb(0, 102, 84);
        private static readonly Color PrimaryDark = Color.FromArgb(0, 82, 68);
        private static readonly Color TextBody = Color.FromArgb(33, 37, 41);
        private static readonly Color TextMuted = Color.FromArgb(73, 80, 87);

        // Top controls
        private Label lblTitle;
        private TextBox txtRef;
        private Button btnFind;
        private Button btnBack;

        // Left: all issues
        private GroupBox gbAll;
        private ListView lvIssues;

        // Right: details
        private GroupBox gbDetails;
        private Label lblRef;
        private Button btnCopyRef;
        private Label lblMeta;
        private GroupBox gbTimeline;
        private ListView lvTimeline;
        private GroupBox gbRoute;
        private FlowLayoutPanel flowRoute;
        private GroupBox gbNext;
        private ComboBox cboNextStatus;
        private Button btnAdvance;

        // Insights (Top-K + MST + AVL)
        private GroupBox gbInsights;
        private NumericUpDown nudTopK;
        private Button btnRefreshTopK;
        private Button btnComputeMst;
        private Label lblMstTotal;
        private Button btnAvlInOrder;   // NEW

        // Bottom-right: Top-K list
        private ListView lvTopK;

        public ServiceRequestStatusForm()
        {
            BuildUI();
            LoadAllIssues();
            LoadTopK();
        }

        // ------------------- UI -------------------

        private void BuildUI()
        {
            Text = "Service Request Status";
            StartPosition = FormStartPosition.CenterParent;
            MinimumSize = new Size(1100, 660);
            BackColor = Color.White;
            KeyPreview = true;
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };

            lblTitle = new Label
            {
                Text = "Track Request Status",
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 16f, FontStyle.Bold),
                ForeColor = TextBody,
                Location = new Point(20, 18)
            };

            txtRef = new TextBox { Location = new Point(24, 64), Width = 300, TabIndex = 0 };
            btnFind = new Button
            {
                Text = "Find by Reference",
                Location = new Point(330, 62),
                Size = new Size(160, 30),
                BackColor = Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                TabIndex = 1
            };
            btnFind.FlatAppearance.BorderSize = 0;
            btnFind.MouseEnter += (s, e) => btnFind.BackColor = PrimaryDark;
            btnFind.MouseLeave += (s, e) => btnFind.BackColor = Primary;
            btnFind.Click += (s, e) => FindByRef();
            AcceptButton = btnFind;

            btnBack = new Button
            {
                Text = "Back",
                Location = new Point(980, 18),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnBack.Click += (s, e) => Close();
            CancelButton = btnBack;

            // ===== Left: all issues =====
            gbAll = new GroupBox
            {
                Text = "All Requests",
                Location = new Point(24, 104),
                Size = new Size(520, 470),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left
            };

            lvIssues = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                HideSelection = false,
                Location = new Point(14, 24),
                Size = new Size(490, 430),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            lvIssues.Columns.Add("Ref", 160);
            lvIssues.Columns.Add("Created", 140);
            lvIssues.Columns.Add("Category", 90);
            lvIssues.Columns.Add("Location", 90);
            lvIssues.SelectedIndexChanged += (s, e) => OnSelectIssueFromList();
            gbAll.Controls.Add(lvIssues);

            // ===== Right: details =====
            gbDetails = new GroupBox
            {
                Text = "Details",
                Location = new Point(560, 104),
                Size = new Size(500, 470),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };

            lblRef = new Label
            {
                AutoSize = true,
                Location = new Point(14, 24),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            btnCopyRef = new Button
            {
                Text = "Copy Ref",
                Location = new Point(400, 20),
                Size = new Size(80, 26),
                FlatStyle = FlatStyle.Flat
            };
            btnCopyRef.Click += (s, e) =>
            {
                if (lvIssues.SelectedItems.Count == 0) return;
                var issue = (Issue)lvIssues.SelectedItems[0].Tag;
                Clipboard.SetText(issue.Id.ToString());
                MessageBox.Show("Reference copied to clipboard.", "Copy", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            lblMeta = new Label { AutoSize = true, Location = new Point(14, 46), ForeColor = TextMuted };

            // Timeline
            gbTimeline = new GroupBox { Text = "Status Timeline", Location = new Point(14, 74), Size = new Size(470, 150) };
            lvTimeline = new ListView
            {
                View = View.Details,
                FullRowSelect = false,
                HideSelection = false,
                Location = new Point(10, 20),
                Size = new Size(448, 118),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            lvTimeline.Columns.Add("When", 160);
            lvTimeline.Columns.Add("Status", 120);
            lvTimeline.Columns.Add("Note", 160);
            gbTimeline.Controls.Add(lvTimeline);

            // Route
            gbRoute = new GroupBox { Text = "Department Route", Location = new Point(14, 232), Size = new Size(470, 90) };
            flowRoute = new FlowLayoutPanel
            {
                Location = new Point(10, 22),
                Size = new Size(448, 56),
                AutoScroll = true,
                WrapContents = false
            };
            gbRoute.Controls.Add(flowRoute);

            // Next status
            gbNext = new GroupBox { Text = "Advance Status", Location = new Point(14, 328), Size = new Size(470, 64) };
            cboNextStatus = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(10, 24),
                Width = 220
            };
            var statuses = Enum.GetValues(typeof(IssueStatus));
            foreach (var s in statuses) cboNextStatus.Items.Add(s);
            if (cboNextStatus.Items.Count > 0) cboNextStatus.SelectedIndex = 0;

            btnAdvance = new Button
            {
                Text = "Apply",
                Location = new Point(240, 22),
                Size = new Size(80, 30),
                BackColor = Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnAdvance.FlatAppearance.BorderSize = 0;
            btnAdvance.MouseEnter += (s, e) => btnAdvance.BackColor = PrimaryDark;
            btnAdvance.MouseLeave += (s, e) => btnAdvance.BackColor = Primary;
            btnAdvance.Click += (s, e) => AdvanceStatus();
            gbNext.Controls.Add(cboNextStatus);
            gbNext.Controls.Add(btnAdvance);

            // Insights (Top-K + MST + AVL)
            gbInsights = new GroupBox
            {
                Text = "Insights (Top-K / MST / AVL)",
                Location = new Point(14, 396),
                Size = new Size(470, 64)
            };

            nudTopK = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 50,
                Value = 5,
                Location = new Point(12, 24),
                Width = 60
            };
            btnRefreshTopK = new Button { Text = "Top-K", Location = new Point(80, 22), Size = new Size(70, 30), FlatStyle = FlatStyle.Flat };
            btnRefreshTopK.Click += (s, e) => LoadTopK();

            btnComputeMst = new Button
            {
                Text = "Compute MST",
                Location = new Point(156, 22),
                Size = new Size(110, 30),
                FlatStyle = FlatStyle.Flat
            };
            btnComputeMst.Click += (s, e) => ComputeMst();

            lblMstTotal = new Label
            {
                AutoSize = true,
                Location = new Point(272, 26),
                ForeColor = TextBody,
                Text = "MST: (not computed)"
            };

            btnAvlInOrder = new Button
            {
                Text = "AVL (in-order)",
                Location = new Point(360, 22),
                Size = new Size(98, 30),
                FlatStyle = FlatStyle.Flat
            };
            btnAvlInOrder.Click += (s, e) => ShowAvlOrdered();

            gbInsights.Controls.Add(nudTopK);
            gbInsights.Controls.Add(btnRefreshTopK);
            gbInsights.Controls.Add(btnComputeMst);
            gbInsights.Controls.Add(lblMstTotal);
            gbInsights.Controls.Add(btnAvlInOrder);

            // Bottom-right Top-K list
            lvTopK = new ListView
            {
                View = View.Details,
                FullRowSelect = true,
                HideSelection = false,
                Location = new Point(560, 584),
                Size = new Size(500, 120),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            lvTopK.Columns.Add("Ref", 160);
            lvTopK.Columns.Add("Created", 140);
            lvTopK.Columns.Add("Category", 90);
            lvTopK.Columns.Add("Location", 90);

            gbDetails.Controls.Add(lblRef);
            gbDetails.Controls.Add(btnCopyRef);
            gbDetails.Controls.Add(lblMeta);
            gbDetails.Controls.Add(gbTimeline);
            gbDetails.Controls.Add(gbRoute);
            gbDetails.Controls.Add(gbNext);
            gbDetails.Controls.Add(gbInsights);

            Controls.Add(lblTitle);
            Controls.Add(txtRef);
            Controls.Add(btnFind);
            Controls.Add(btnBack);
            Controls.Add(gbAll);
            Controls.Add(gbDetails);
            Controls.Add(lvTopK);

            Resize += (s, e) => FixTopKPlacement();
        }

        private void FixTopKPlacement()
        {
            lvTopK.Left = gbDetails.Left;
            lvTopK.Width = gbDetails.Width;
            lvTopK.Top = gbDetails.Bottom + 8;
        }

        // ------------------- Data wiring -------------------

        private void LoadAllIssues()
        {
            var issues = IssueStore.GetAllSnapshot();

            lvIssues.BeginUpdate();
            lvIssues.Items.Clear();

            for (int i = 0; i < issues.Length; i++)
            {
                var it = issues[i];
                var li = new ListViewItem(it.Id.ToString());
                li.SubItems.Add(it.CreatedAt.ToLocalTime().ToString("dd MMM yyyy HH:mm"));
                li.SubItems.Add(it.Category.ToString());
                li.SubItems.Add(it.Location);
                li.Tag = it;
                lvIssues.Items.Add(li);
            }

            lvIssues.EndUpdate();
            AutoSizeColumns(lvIssues);
        }

        private void LoadTopK()
        {
            int k = (int)nudTopK.Value;
            var top = IssuePriorityQueue.OldestOpen(k);

            lvTopK.BeginUpdate();
            lvTopK.Items.Clear();

            for (int i = 0; i < top.Length; i++)
            {
                var it = top[i];
                var li = new ListViewItem(it.Id.ToString());
                li.SubItems.Add(it.CreatedAt.ToLocalTime().ToString("dd MMM yyyy HH:mm"));
                li.SubItems.Add(it.Category.ToString());
                li.SubItems.Add(it.Location);
                li.Tag = it;
                lvTopK.Items.Add(li);
            }

            lvTopK.EndUpdate();
            AutoSizeColumns(lvTopK, maxCol4: 200);
        }

        private void AutoSizeColumns(ListView lv, int maxCol4 = 140)
        {
            for (int i = 0; i < lv.Columns.Count; i++)
                lv.Columns[i].Width = -2; // auto
            if (lv.Columns.Count >= 4 && lv.Columns[3].Width > maxCol4)
                lv.Columns[3].Width = maxCol4;
        }

        // ------------------- Actions -------------------

        private void FindByRef()
        {
            string s = txtRef.Text.Trim();
            if (s.Length == 0)
            {
                MessageBox.Show("Enter a reference (GUID) from your submitted report.", "Search",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtRef.Focus();
                return;
            }

            Guid id;
            if (!Guid.TryParse(s, out id))
            {
                MessageBox.Show("Reference is not a valid GUID.", "Search",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtRef.SelectAll();
                txtRef.Focus();
                return;
            }

            for (int i = 0; i < lvIssues.Items.Count; i++)
            {
                if (string.Equals(lvIssues.Items[i].Text, id.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    lvIssues.Items[i].Selected = true;
                    lvIssues.EnsureVisible(i);
                    ShowIssueDetails((Issue)lvIssues.Items[i].Tag);
                    return;
                }
            }

            MessageBox.Show("Reference not found in current data.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnSelectIssueFromList()
        {
            if (lvIssues.SelectedItems.Count == 0) return;
            var issue = (Issue)lvIssues.SelectedItems[0].Tag;
            ShowIssueDetails(issue);
        }

        private void ShowIssueDetails(Issue issue)
        {
            if (issue == null) return;

            StatusIndex.Ensure(issue.Id);

            lblRef.Text = "Reference: " + issue.Id.ToString();
            lblMeta.Text = "Created: " + issue.CreatedAt.ToLocalTime().ToString("dd MMM yyyy HH:mm")
                         + "  •  Category: " + issue.Category.ToString()
                         + "  •  Location: " + (issue.Location ?? "");

            var tl = StatusIndex.GetTimeline(issue.Id);
            lvTimeline.BeginUpdate();
            lvTimeline.Items.Clear();
            for (int i = 0; i < tl.Length; i++)
            {
                var sn = tl[i];
                var li = new ListViewItem(sn.TimestampUtc.ToLocalTime().ToString("dd MMM yyyy HH:mm"));
                li.SubItems.Add(sn.Status.ToString());
                li.SubItems.Add(sn.Note);
                lvTimeline.Items.Add(li);
            }
            lvTimeline.EndUpdate();
            AutoSizeColumns(lvTimeline, maxCol4: 260);

            flowRoute.SuspendLayout();
            flowRoute.Controls.Clear();
            var route = DepartmentNetwork.FullLifecycleRoute(issue.Category);
            for (int i = 0; i < route.Length; i++)
            {
                var pill = MakePill(route[i], i == 0, i == route.Length - 1);
                flowRoute.Controls.Add(pill);
            }
            flowRoute.ResumeLayout();

            IssueStatus suggested = SuggestNextStatus(tl);
            SelectStatusInCombo(suggested);
        }

        private Control MakePill(string text, bool start, bool end)
        {
            var lbl = new Label
            {
                AutoSize = true,
                Text = text,
                BackColor = start ? Color.FromArgb(232, 247, 238)
                        : end ? Color.FromArgb(233, 236, 239)
                        : Color.FromArgb(248, 249, 250),
                ForeColor = TextBody,
                Padding = new Padding(8, 4, 8, 4),
                Margin = new Padding(4, 8, 4, 8),
                BorderStyle = BorderStyle.FixedSingle
            };
            return lbl;
        }

        private IssueStatus SuggestNextStatus(StatusNode[] timeline)
        {
            if (timeline == null || timeline.Length == 0) return IssueStatus.Triaged;
            var last = timeline[timeline.Length - 1].Status;

            var flow = IssueStatusHelper.DefaultFlow();
            for (int i = 0; i < flow.Count - 1; i++)
            {
                if (flow[i] == last) return flow[i + 1];
            }
            if (last == IssueStatus.Resolved) return IssueStatus.Closed;
            return last;
        }

        private void SelectStatusInCombo(IssueStatus status)
        {
            for (int i = 0; i < cboNextStatus.Items.Count; i++)
            {
                var s = (IssueStatus)cboNextStatus.Items[i];
                if (s == status)
                {
                    cboNextStatus.SelectedIndex = i;
                    break;
                }
            }
        }

        private void AdvanceStatus()
        {
            if (lvIssues.SelectedItems.Count == 0)
            {
                MessageBox.Show("Select a request on the left first.", "Advance Status",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var issue = (Issue)lvIssues.SelectedItems[0].Tag;
            var tl = StatusIndex.GetTimeline(issue.Id);
            var last = tl != null && tl.Length > 0 ? tl[tl.Length - 1].Status : IssueStatus.Submitted;

            var next = (IssueStatus)cboNextStatus.SelectedItem;
            if (!IssueStatusHelper.IsValidTransition(last, next))
            {
                MessageBox.Show("Invalid transition from " + last + " to " + next + ".", "Advance Status",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool ok = StatusIndex.AddStatus(issue.Id, next, "Updated via Status screen.");
            if (!ok)
            {
                MessageBox.Show("Could not apply status update.", "Advance Status",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ShowIssueDetails(issue);
            LoadTopK();

            MessageBox.Show("Status updated to " + next + ".", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        // ------------------- MST -------------------

        private void ComputeMst()
        {
            try
            {
                var res = LocationNetwork.BuildOpenIssuesMst();
                double total = res.Item1;
                var edges = res.Item2;

                lblMstTotal.Text = "MST: total length = " + total.ToString("0.00");

                if (edges == null || edges.Count == 0)
                {
                    MessageBox.Show("Not enough open issues to build an MST.", "MST",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                var sb = new StringBuilder();
                sb.AppendLine("Minimum Spanning Tree (open issues)");
                sb.AppendLine("Total length: " + total.ToString("0.00"));
                sb.AppendLine();
                for (int i = 0; i < edges.Count; i++)
                {
                    var e = edges[i];
                    sb.AppendLine(e.Item1 + "  →  " + e.Item2 + "   (" + e.Item3.ToString("0.00") + ")");
                }

                MessageBox.Show(sb.ToString(), "MST Edges", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to compute MST: " + ex.Message, "MST",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ------------------- AVL demo -------------------

        private void ShowAvlOrdered()
        {
            int k = (int)nudTopK.Value; // reuse Top-K value as the count to display
            var ordered = IssueAvlIndex.InOrder(k);

            if (ordered == null || ordered.Length == 0)
            {
                MessageBox.Show("No issues available in AVL index.", "AVL (in-order)",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var sb = new StringBuilder();
            sb.AppendLine("AVL In-Order (first " + k + " by CreatedAt):");
            sb.AppendLine();
            for (int i = 0; i < ordered.Length; i++)
            {
                var it = ordered[i];
                sb.AppendLine(
                    it.CreatedAt.ToLocalTime().ToString("dd MMM yyyy HH:mm") + "  —  "
                    + it.Id + "  —  " + it.Category + " — " + it.Location);
            }

            MessageBox.Show(sb.ToString(), "AVL (in-order)", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
