using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp
{
    public partial class LocalEventsForm : Form
    {
        // Theme
        private static readonly Color TextBody = Color.FromArgb(33, 37, 41);
        private static readonly Color TextMuted = Color.FromArgb(73, 80, 87);
        private static readonly Color Primary = Color.FromArgb(0, 102, 84);
        private static readonly Color PrimaryDark = Color.FromArgb(0, 82, 68);

        private Label lblTitle;

        private TextBox txtQuery;
        private ComboBox cboCategory;
        private CheckBox chkFrom;
        private DateTimePicker dtFrom;
        private CheckBox chkTo;
        private DateTimePicker dtTo;

        private Button btnSearch;
        private Button btnClear;
        private Button btnBack;

        private Label lblRecs;
        private ListView lvResults;
        private Label lblEmpty;

        public LocalEventsForm()
        {
            BuildUI();
            LoadFilters();
            DoSearch();
        }

        private void BuildUI()
        {
            Text = "Local Events & Announcements";
            StartPosition = FormStartPosition.CenterParent;
            ClientSize = new Size(980, 640);
            BackColor = Color.White;
            KeyPreview = true;
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };

            lblTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 16f, FontStyle.Bold),
                ForeColor = TextBody,
                Text = "Local Events & Announcements",
                Location = new Point(20, 18)
            };

            // ==== Filters row ====
            txtQuery = new TextBox { Location = new Point(24, 68), Width = 280, TabIndex = 0 };
            cboCategory = new ComboBox
            {
                Location = new Point(314, 68),
                Width = 180,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 1
            };
            chkFrom = new CheckBox { Text = "From", AutoSize = true, Location = new Point(510, 70), TabIndex = 2 };
            dtFrom = new DateTimePicker { Location = new Point(560, 68), Width = 150, Enabled = false, TabIndex = 3 };
            chkTo = new CheckBox { Text = "To", AutoSize = true, Location = new Point(720, 70), TabIndex = 4 };
            dtTo = new DateTimePicker { Location = new Point(752, 68), Width = 150, Enabled = false, TabIndex = 5 };
            chkFrom.CheckedChanged += (s, e) => dtFrom.Enabled = chkFrom.Checked;
            chkTo.CheckedChanged += (s, e) => dtTo.Enabled = chkTo.Checked;

            btnSearch = new Button
            {
                Text = "Search",
                Location = new Point(24, 102),
                Size = new Size(100, 32),
                BackColor = Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                TabIndex = 6
            };
            btnSearch.FlatAppearance.BorderSize = 0;
            btnSearch.MouseEnter += (s, e) => btnSearch.BackColor = PrimaryDark;
            btnSearch.MouseLeave += (s, e) => btnSearch.BackColor = Primary;
            btnSearch.Click += (s, e) => DoSearch();
            this.AcceptButton = btnSearch;

            btnClear = new Button
            {
                Text = "Clear",
                Location = new Point(130, 102),
                Size = new Size(100, 32),
                FlatStyle = FlatStyle.Flat,
                TabIndex = 7
            };
            btnClear.Click += (s, e) =>
            {
                txtQuery.Text = "";
                cboCategory.SelectedIndex = 0;
                chkFrom.Checked = false;
                chkTo.Checked = false;
                DoSearch();
            };

            btnBack = new Button
            {
                Text = "Back",
                Location = new Point(880, 18),
                Size = new Size(80, 32),
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                TabIndex = 8
            };
            btnBack.Click += (s, e) => Close();

            lblRecs = new Label
            {
                AutoSize = true,
                Location = new Point(246, 108),
                ForeColor = TextMuted
            };

            // ==== Results ====
            lvResults = new ListView
            {
                Location = new Point(24, 144),
                Size = new Size(936, 460),
                View = View.Details,
                FullRowSelect = true,
                GridLines = false,
                HideSelection = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            lvResults.Columns.Add("Date & Time", 170);
            lvResults.Columns.Add("Title", 320);
            lvResults.Columns.Add("Category", 140);
            lvResults.Columns.Add("Location", 160);
            lvResults.Columns.Add("Description", 260);
            lvResults.DoubleClick += (s, e) => ShowSelectedDetails();

            // Empty state (appears over list when no results)
            lblEmpty = new Label
            {
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular),
                ForeColor = TextMuted,
                Text = "No events found for the current filters.",
                Visible = false,
                Location = lvResults.Location,
                Size = lvResults.Size,
                Anchor = lvResults.Anchor
            };

            Controls.Add(lblTitle);
            Controls.Add(txtQuery);
            Controls.Add(cboCategory);
            Controls.Add(chkFrom);
            Controls.Add(dtFrom);
            Controls.Add(chkTo);
            Controls.Add(dtTo);
            Controls.Add(btnSearch);
            Controls.Add(btnClear);
            Controls.Add(btnBack);
            Controls.Add(lblRecs);
            Controls.Add(lvResults);
            Controls.Add(lblEmpty);
        }

        private void LoadFilters()
        {
            string[] cats = EventStore.Categories();
            cboCategory.Items.Clear();
            cboCategory.Items.Add("(All categories)");
            for (int i = 0; i < cats.Length; i++) cboCategory.Items.Add(cats[i]);
            cboCategory.SelectedIndex = 0;

            // Recommendations
            string[] top = EventStore.RecommendTopCategories(3);
            lblRecs.Text = top.Length > 0 ? "Suggested: " + string.Join(" • ", top) : "";
        }

        private void DoSearch()
        {
            string q = txtQuery.Text;
            string cat = (cboCategory.SelectedIndex <= 0) ? "" : (string)cboCategory.SelectedItem;
            DateTime? from = chkFrom.Checked ? (DateTime?)dtFrom.Value.Date : null;
            DateTime? to = chkTo.Checked ? (DateTime?)dtTo.Value.Date.AddDays(1).AddSeconds(-1) : null;

            EventItem[] results = EventStore.Search(q, cat, from, to);

            lvResults.BeginUpdate();
            lvResults.Items.Clear();
            for (int i = 0; i < results.Length; i++)
            {
                EventItem ev = results[i];
                var li = new ListViewItem(ev.StartUtc.ToLocalTime().ToString("dd MMM yyyy HH:mm"));
                li.SubItems.Add(ev.Title);
                li.SubItems.Add(ev.Category);
                li.SubItems.Add(ev.Location);
                li.SubItems.Add(ev.Description);
                li.Tag = ev;
                lvResults.Items.Add(li);
            }
            lvResults.EndUpdate();

            // Empty state & column sizing
            lblEmpty.Visible = (results.Length == 0);
            AutoSizeColumns();
            LoadFilters(); // refresh recs based on query/category tokens
        }

        private void AutoSizeColumns()
        {
            for (int i = 0; i < lvResults.Columns.Count; i++)
            {
                // -2 = column header + content autosize in WinForms
                lvResults.Columns[i].Width = -2;
            }
            // Keep description from getting absurdly wide on large screens
            int maxDesc = 380;
            if (lvResults.Columns.Count >= 5 && lvResults.Columns[4].Width > maxDesc)
                lvResults.Columns[4].Width = maxDesc;
        }

        private void ShowSelectedDetails()
        {
            if (lvResults.SelectedItems.Count == 0) return;
            var ev = (EventItem)lvResults.SelectedItems[0].Tag;

            var sb = new StringBuilder();
            sb.AppendLine(ev.Title);
            sb.AppendLine("Date: " + ev.StartUtc.ToLocalTime().ToString("dddd, dd MMM yyyy HH:mm"));
            sb.AppendLine("Category: " + ev.Category);
            sb.AppendLine("Location: " + ev.Location);
            if (!string.IsNullOrEmpty(ev.Description))
            {
                sb.AppendLine();
                sb.AppendLine(ev.Description);
            }

            MessageBox.Show(sb.ToString(), "Event details", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
