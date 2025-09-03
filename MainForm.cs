using System;
using System.Drawing;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public class MainForm : Form
    {
        private Button btnReportIssues;
        private Button btnEvents;
        private Button btnStatus;
        private Label lblHeader;

        public MainForm()
        {
            InitializeComponent();
            Text = "Municipal Services — Main Menu";
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeComponent()
        {
            this.lblHeader = new Label();
            this.btnReportIssues = new Button();
            this.btnEvents = new Button();
            this.btnStatus = new Button();

            // Form
            this.ClientSize = new Size(560, 260);

            // Header
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblHeader.Text = "Welcome to the Municipal Services App";
            this.lblHeader.Location = new Point(30, 20);

            // Report Issues
            this.btnReportIssues.Text = "Report Issues — Implemented ✅";
            this.btnReportIssues.Location = new Point(35, 80);
            this.btnReportIssues.Size = new Size(480, 40);
            this.btnReportIssues.Click += new EventHandler(this.btnReportIssues_Click);

            // Events (disabled)
            this.btnEvents.Text = "Local Events & Announcements — Coming Soon";
            this.btnEvents.Enabled = false;
            this.btnEvents.Location = new Point(35, 130);
            this.btnEvents.Size = new Size(480, 40);

            // Status (disabled)
            this.btnStatus.Text = "Service Request Status — Coming Soon";
            this.btnStatus.Enabled = false;
            this.btnStatus.Location = new Point(35, 180);
            this.btnStatus.Size = new Size(480, 40);

            // Add controls
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.btnReportIssues);
            this.Controls.Add(this.btnEvents);
            this.Controls.Add(this.btnStatus);
        }

        private void btnReportIssues_Click(object sender, EventArgs e)
        {
            using (var f = new ReportIssueForm())
            {
                f.ShowDialog(this);
            }
        }
    }
}
