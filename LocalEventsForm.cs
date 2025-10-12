using System;
using System.Drawing;
using System.Windows.Forms;
using MunicipalServicesApp.Data;
using MunicipalServicesApp.Models;

namespace MunicipalServicesApp
{
    public class LocalEventsForm : Form
    {
        private Label lblTitle;
        private TextBox txtQuery;
        private ComboBox cboCategory;
        private DateTimePicker dtFrom;
        private DateTimePicker dtTo;
        private CheckBox chkFrom;
        private CheckBox chkTo;
        private Button btnSearch;
        private Button btnClear;
        private Button btnBack;
        private ListView lvResults;
        private Label lblRecs;

        public LocalEventsForm()
        {
            InitializeComponent();
            LoadFilters();
            DoSearch();
        }

        private void InitializeComponent()
        {
            this.Text = "Local Events & Announcements";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(880, 580);

            lblTitle = new Label { Text = "Local Events & Announcements", AutoSize = true, Font = new Font("Segoe UI", 13f, FontStyle.Bold), Location = new Point(18, 16) };

            txtQuery = new TextBox { Location = new Point(22, 58), Width = 220 };
            cboCategory = new ComboBox { Location = new Point(252, 58), Width = 180, DropDownStyle = ComboBoxStyle.DropDownList };
            chkFrom = new CheckBox { Text = "From:", Location = new Point(446, 60), AutoSize = true };
            dtFrom = new DateTimePicker { Location = new Point(500, 58), Width = 150, Enabled = false };
            chkTo = new CheckBox { Text = "To:", Location = new Point(660, 60), AutoSize = true };
            dtTo = new DateTimePicker { Location = new Point(700, 58), Width = 150, Enabled = false };
            chkFrom.CheckedChanged += (s, e) => dtFrom.Enabled = chkFrom.Checked;
            chkTo.CheckedChanged += (s, e) => dtTo.Enabled = chkTo.Checked;

            btnSearch = new Button { Text = "Search", Location = new Point(22, 92), Width = 100 };
            btnClear = new Button { Text = "Clear", Location = new Point(128, 92), Width = 100 };
            btnBack = new Button { Text = "Back to Menu", Location = new Point(700, 520), Width = 150, Anchor = AnchorStyles.Bottom | AnchorStyles.Right };

            btnSearch.Click += (s, e) => DoSearch();
            btnClear.Click += (s, e) => { txtQuery.Text = ""; cboCategory.SelectedIndex = 0; chkFrom.Checked = false; chkTo.Checked = false; DoSearch(); };
            btnBack.Click += (s, e) => this.Close();

            lvResults = new ListView { Location = new Point(22, 132), Size = new Size(828, 372), View = View.Details, FullRowSelect = true, GridLines = false, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right };
            lvResults.Columns.Add("Date & Time", 160);
            lvResults.Columns.Add("Title", 260);
            lvResults.Columns.Add("Category", 120);
            lvResults.Columns.Add("Location", 160);
            lvResults.Columns.Add("Description", 120);

            lblRecs = new Label { AutoSize = true, Location = new Point(252, 95), ForeColor = Color.FromArgb(33, 37, 41) };

            this.Controls.Add(lblTitle);
            this.Controls.Add(txtQuery);
            this.Controls.Add(cboCategory);
            this.Controls.Add(chkFrom);
            this.Controls.Add(dtFrom);
            this.Controls.Add(chkTo);
            this.Controls.Add(dtTo);
            this.Controls.Add(btnSearch);
            this.Controls.Add(btnClear);
            this.Controls.Add(lblRecs);
            this.Controls.Add(lvResults);
            this.Controls.Add(btnBack);
        }

        private void LoadFilters()
        {
            // categories
            string[] cats = EventStore.Categories();
            cboCategory.Items.Clear();
            cboCategory.Items.Add("(All categories)");
            for (int i = 0; i < cats.Length; i++) cboCategory.Items.Add(cats[i]);
            cboCategory.SelectedIndex = 0;

            // show top category recommendations
            string[] top = EventStore.RecommendTopCategories(3);
            if (top.Length > 0)
                lblRecs.Text = "Suggested: " + string.Join(" • ", top);
            else
                lblRecs.Text = "";
        }

        private void DoSearch()
        {
            string q = txtQuery.Text;
            string cat = cboCategory.SelectedIndex <= 0 ? "" : (string)cboCategory.SelectedItem;
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
                lvResults.Items.Add(li);
            }
            lvResults.EndUpdate();

            LoadFilters(); // refresh suggestions based on new search tokens
        }
    }
}
