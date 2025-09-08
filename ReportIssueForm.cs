using MunicipalServicesApp.Data;
using MunicipalServicesApp.DataStructures; // <-- custom DynamicArray<T>
using MunicipalServicesApp.Models;
using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public class ReportIssueForm : Form
    {
        private Label lblTitle;

        private Label lblLocation;
        private TextBox txtLocation;

        private Label lblCategory;
        private ComboBox cboCategory;

        private Label lblDescription;
        private RichTextBox rtbDescription;

        private Label lblAttachments;
        private ListBox lstFiles;
        private Button btnAttach;

        private GroupBox grpContact;
        private RadioButton rbInApp;
        private RadioButton rbSMS;
        private RadioButton rbWhatsApp;
        private TextBox txtPhone;

        private ProgressBar pbSubmit;
        private Label lblTip;

        private Button btnSubmit;
        private Button btnBack;

        public ReportIssueForm()
        {
            InitializeComponent();
            Text = "Report an Issue";
            StartPosition = FormStartPosition.CenterParent;

            // Populate category dropdown
            this.cboCategory.DataSource = Enum.GetValues(typeof(IssueCategory));

            // Engagement / UX hint
            this.lblTip.Text = "Tip: Clear details and a photo help faster resolution.";

            // Default channel
            this.rbInApp.Checked = true;
        }

        private void InitializeComponent()
        {
            this.lblTitle = new Label();

            this.lblLocation = new Label();
            this.txtLocation = new TextBox();

            this.lblCategory = new Label();
            this.cboCategory = new ComboBox();

            this.lblDescription = new Label();
            this.rtbDescription = new RichTextBox();

            this.lblAttachments = new Label();
            this.lstFiles = new ListBox();
            this.btnAttach = new Button();

            this.grpContact = new GroupBox();
            this.rbInApp = new RadioButton();
            this.rbSMS = new RadioButton();
            this.rbWhatsApp = new RadioButton();
            this.txtPhone = new TextBox();

            this.pbSubmit = new ProgressBar();
            this.lblTip = new Label();

            this.btnSubmit = new Button();
            this.btnBack = new Button();

            // Form
            this.ClientSize = new Size(760, 560);

            // Title
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            this.lblTitle.Text = "Report Issues";
            this.lblTitle.Location = new Point(20, 15);

            // Location
            this.lblLocation.AutoSize = true;
            this.lblLocation.Text = "Location:";
            this.lblLocation.Location = new Point(20, 60);

            this.txtLocation.Location = new Point(100, 56);
            this.txtLocation.Size = new Size(300, 24);
            this.txtLocation.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Category
            this.lblCategory.AutoSize = true;
            this.lblCategory.Text = "Category:";
            this.lblCategory.Location = new Point(420, 60);

            this.cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cboCategory.Location = new Point(490, 56);
            this.cboCategory.Size = new Size(240, 24);
            this.cboCategory.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            // Description
            this.lblDescription.AutoSize = true;
            this.lblDescription.Text = "Description:";
            this.lblDescription.Location = new Point(20, 100);

            this.rtbDescription.Location = new Point(23, 120);
            this.rtbDescription.Size = new Size(707, 140);
            this.rtbDescription.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // Attachments
            this.lblAttachments.AutoSize = true;
            this.lblAttachments.Text = "Attachments:";
            this.lblAttachments.Location = new Point(20, 270);

            this.lstFiles.Location = new Point(23, 290);
            this.lstFiles.Size = new Size(600, 95);
            this.lstFiles.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            this.btnAttach.Text = "Attach Images/Documents…";
            this.btnAttach.Location = new Point(630, 290);
            this.btnAttach.Size = new Size(100, 60);
            this.btnAttach.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.btnAttach.Click += new EventHandler(this.btnAttach_Click);

            // Contact group
            this.grpContact.Text = "Preferred Contact Channel";
            this.grpContact.Location = new Point(23, 395);
            this.grpContact.Size = new Size(450, 90);

            this.rbInApp.Text = "In-App";
            this.rbInApp.Location = new Point(15, 25);

            this.rbSMS.Text = "SMS";
            this.rbSMS.Location = new Point(100, 25);

            this.rbWhatsApp.Text = "WhatsApp";
            this.rbWhatsApp.Location = new Point(160, 25);

            this.txtPhone.Location = new Point(15, 55);
            this.txtPhone.Size = new Size(410, 24);
            this.txtPhone.Text = "";
            var lblPhone = new Label();
            lblPhone.Text = "Phone (for SMS/WhatsApp):";
            lblPhone.Location = new Point(15, 40);
            lblPhone.AutoSize = true;

            this.grpContact.Controls.Add(this.rbInApp);
            this.grpContact.Controls.Add(this.rbSMS);
            this.grpContact.Controls.Add(this.rbWhatsApp);
            this.grpContact.Controls.Add(lblPhone);
            this.grpContact.Controls.Add(this.txtPhone);

            // Progress + tip
            this.pbSubmit.Location = new Point(23, 495);
            this.pbSubmit.Size = new Size(450, 18);
            this.pbSubmit.Visible = false;

            this.lblTip.AutoSize = true;
            this.lblTip.Location = new Point(23, 520);
            this.lblTip.Size = new Size(400, 18);

            // Buttons
            this.btnSubmit.Text = "Submit";
            this.btnSubmit.Location = new Point(610, 485);
            this.btnSubmit.Size = new Size(120, 36);
            this.btnSubmit.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnSubmit.Click += new EventHandler(this.btnSubmit_Click);

            this.btnBack.Text = "Back to Main Menu";
            this.btnBack.Location = new Point(480, 485);
            this.btnBack.Size = new Size(120, 36);
            this.btnBack.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnBack.Click += new EventHandler(this.btnBack_Click);

            // Add controls
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.cboCategory);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.rtbDescription);
            this.Controls.Add(this.lblAttachments);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.btnAttach);
            this.Controls.Add(this.grpContact);
            this.Controls.Add(this.pbSubmit);
            this.Controls.Add(this.lblTip);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.btnBack);
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Title = "Attach images or documents";
                dlg.Multiselect = true;
                dlg.Filter = "Images or Documents|*.jpg;*.jpeg;*.png;*.pdf;*.docx;*.xlsx;*.txt|All files|*.*";

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    for (int i = 0; i < dlg.FileNames.Length; i++)
                    {
                        this.lstFiles.Items.Add(dlg.FileNames[i]);
                    }
                    this.lblTip.Text = "Nice! Attachments added.";
                }
            }
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(this.txtLocation.Text))
            {
                MessageBox.Show("Please enter a location.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.txtLocation.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(this.rtbDescription.Text))
            {
                MessageBox.Show("Please describe the issue.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.rtbDescription.Focus();
                return;
            }

            // Build model
            var issue = new Issue();
            issue.Location = this.txtLocation.Text.Trim();
            issue.Category = (IssueCategory)this.cboCategory.SelectedItem;
            issue.Description = this.rtbDescription.Text.Trim();

            // Build attachments with custom DynamicArray<string>
            var attachments = new DynamicArray<string>();
            for (int i = 0; i < this.lstFiles.Items.Count; i++)
            {
                object obj = this.lstFiles.Items[i];
                if (obj != null)
                {
                    string path = obj.ToString();
                    if (!string.IsNullOrEmpty(path))
                    {
                        attachments.Add(path);
                    }
                }
            }
            issue.Attachments = attachments;

            // Channel & phone
            issue.PreferredChannel =
                this.rbSMS.Checked ? ContactChannel.SMS :
                (this.rbWhatsApp.Checked ? ContactChannel.WhatsApp : ContactChannel.InApp);
            issue.PhoneNumber = this.txtPhone.Text.Trim();

            // Progress
            this.pbSubmit.Visible = true; this.pbSubmit.Value = 10;
            this.lblTip.Text = "Submitting… please wait.";

            // Copy attachments into app folder (demo only)
            string saveDir = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", issue.Id.ToString());
            Directory.CreateDirectory(saveDir);

            foreach (string file in issue.Attachments) // IEnumerable<T> supported
            {
                try
                {
                    string fileName = Path.GetFileName(file);
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        string dest = Path.Combine(saveDir, fileName);
                        if (File.Exists(file)) File.Copy(file, dest, true);
                    }
                }
                catch
                {
                    // ignore copy errors for demo
                }
            }
            this.pbSubmit.Value = 60;

            await Task.Delay(400); // small UX pause

            IssueStore.Add(issue);
            this.pbSubmit.Value = 100;

            MessageBox.Show(
                "Thank you! Your report has been logged."
                + Environment.NewLine + Environment.NewLine
                + "Reference: " + issue.Id
                + Environment.NewLine
                + "Channel: " + issue.PreferredChannel,
                "Report Submitted",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.lblTip.Text = "Report captured. You can submit another or go back to main menu.";
            this.pbSubmit.Visible = false;
            ClearForm();
        }

        private void ClearForm()
        {
            this.txtLocation.Clear();
            if (this.cboCategory.Items.Count > 0) this.cboCategory.SelectedIndex = 0;
            this.rtbDescription.Clear();
            this.lstFiles.Items.Clear();
            this.rbInApp.Checked = true;
            this.txtPhone.Clear();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
