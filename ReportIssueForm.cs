using MunicipalServicesApp.Data;
using MunicipalServicesApp.DataStructures; // DynamicArray<T>
using MunicipalServicesApp.Models;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public partial class ReportIssueForm : Form
    {
        // Theme
        private static readonly Color Primary = Color.FromArgb(0, 102, 84);
        private static readonly Color PrimaryDark = Color.FromArgb(0, 82, 68);
        private static readonly Color TextBody = Color.FromArgb(33, 37, 41);
        private static readonly Color TextMuted = Color.FromArgb(73, 80, 87);

        private Label lblTitle;

        private Label lblLocation;
        private TextBox txtLocation;
        private Label lblCategory;
        private ComboBox cboCategory;

        private Label lblDescription;
        private RichTextBox rtbDescription;
        private Label lblCount; // character counter

        private Label lblAttachments;
        private ListBox lstFiles;
        private Button btnAttach;
        private Button btnOpen;
        private Button btnRemove;

        private GroupBox grpContact;
        private RadioButton rbInApp;
        private RadioButton rbSMS;
        private RadioButton rbWhatsApp;
        private TextBox txtPhone;
        private Label lblPhone;

        private ProgressBar pbSubmit;
        private Label lblTip;

        private Button btnSubmit;
        private Button btnBack;

        public ReportIssueForm()
        {
            BuildUI();

            Text = "Report an Issue";
            StartPosition = FormStartPosition.CenterParent;
            KeyPreview = true;
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };

            // Populate category dropdown
            cboCategory.DataSource = Enum.GetValues(typeof(IssueCategory));

            // Engagement / UX hint
            lblTip.Text = "Tip: clear details + a photo help faster resolution.";

            // Default channel
            rbInApp.Checked = true;
        }

        private void BuildUI()
        {
            // Form canvas
            ClientSize = new Size(900, 640);
            BackColor = Color.White;

            lblTitle = new Label
            {
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 16f, FontStyle.Bold),
                ForeColor = TextBody,
                Text = "Report a Municipal Issue",
                Location = new Point(20, 18)
            };

            // === Row 1: Location & Category ===
            lblLocation = new Label
            {
                AutoSize = true,
                Text = "• Location (required)",
                ForeColor = TextBody,
                Location = new Point(20, 66)
            };
            txtLocation = new TextBox { Location = new Point(24, 88), Width = 370, TabIndex = 0 };

            lblCategory = new Label
            {
                AutoSize = true,
                Text = "• Category (required)",
                ForeColor = TextBody,
                Location = new Point(412, 66)
            };
            cboCategory = new ComboBox
            {
                Location = new Point(416, 88),
                Width = 240,
                DropDownStyle = ComboBoxStyle.DropDownList,
                TabIndex = 1
            };

            // === Row 2: Description with counter ===
            lblDescription = new Label
            {
                AutoSize = true,
                Text = "• Description (required)",
                ForeColor = TextBody,
                Location = new Point(20, 128)
            };
            rtbDescription = new RichTextBox
            {
                Location = new Point(24, 150),
                Size = new Size(632, 150),
                DetectUrls = false,
                TabIndex = 2
            };
            lblCount = new Label
            {
                AutoSize = true,
                ForeColor = TextMuted,
                Location = new Point(24, 305),
                Text = "0 characters"
            };
            rtbDescription.TextChanged += (s, e) =>
            {
                int len = rtbDescription.Text.Length;
                lblCount.Text = len + " character" + (len == 1 ? "" : "s");
            };

            // === Row 3: Attachments ===
            lblAttachments = new Label
            {
                AutoSize = true,
                Text = "Attachments (optional)",
                ForeColor = TextBody,
                Location = new Point(20, 332)
            };

            lstFiles = new ListBox
            {
                Location = new Point(24, 354),
                Size = new Size(632, 96),
                IntegralHeight = false,
                TabIndex = 3
            };

            btnAttach = new Button
            {
                Text = "Add…",
                Location = new Point(24, 456),
                Size = new Size(80, 32),
                BackColor = Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                TabIndex = 4
            };
            btnAttach.FlatAppearance.BorderSize = 0;
            btnAttach.MouseEnter += (s, e) => btnAttach.BackColor = PrimaryDark;
            btnAttach.MouseLeave += (s, e) => btnAttach.BackColor = Primary;
            btnAttach.Click += btnAttach_Click;

            btnOpen = new Button
            {
                Text = "Open",
                Location = new Point(110, 456),
                Size = new Size(80, 32),
                FlatStyle = FlatStyle.Flat,
                TabIndex = 5
            };
            btnOpen.Click += (s, e) => OpenSelectedAttachment();

            btnRemove = new Button
            {
                Text = "Remove",
                Location = new Point(196, 456),
                Size = new Size(90, 32),
                FlatStyle = FlatStyle.Flat,
                TabIndex = 6
            };
            btnRemove.Click += (s, e) => RemoveSelectedAttachment();

            // === Row 4: Contact preference ===
            grpContact = new GroupBox
            {
                Text = "Preferred Contact Channel",
                Location = new Point(672, 150),
                Size = new Size(206, 170)
            };
            rbInApp = new RadioButton { Text = "In-App", Location = new Point(16, 28), AutoSize = true, TabIndex = 7 };
            rbSMS = new RadioButton { Text = "SMS", Location = new Point(16, 54), AutoSize = true, TabIndex = 8 };
            rbWhatsApp = new RadioButton { Text = "WhatsApp", Location = new Point(16, 80), AutoSize = true, TabIndex = 9 };

            lblPhone = new Label { AutoSize = true, Text = "Phone (for SMS/WhatsApp):", Location = new Point(16, 106) };
            txtPhone = new TextBox { Location = new Point(16, 126), Width = 170, TabIndex = 10 };

            grpContact.Controls.Add(rbInApp);
            grpContact.Controls.Add(rbSMS);
            grpContact.Controls.Add(rbWhatsApp);
            grpContact.Controls.Add(lblPhone);
            grpContact.Controls.Add(txtPhone);

            // === Footer: progress + actions ===
            pbSubmit = new ProgressBar
            {
                Location = new Point(24, 510),
                Size = new Size(632, 16),
                Visible = false
            };

            lblTip = new Label
            {
                AutoSize = true,
                ForeColor = TextMuted,
                Location = new Point(24, 532),
                Text = ""
            };

            btnSubmit = new Button
            {
                Text = "Submit",
                Location = new Point(760, 568),
                Size = new Size(118, 38),
                BackColor = Primary,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                TabIndex = 11
            };
            btnSubmit.FlatAppearance.BorderSize = 0;
            btnSubmit.MouseEnter += (s, e) => btnSubmit.BackColor = PrimaryDark;
            btnSubmit.MouseLeave += (s, e) => btnSubmit.BackColor = Primary;
            btnSubmit.Click += btnSubmit_Click;
            this.AcceptButton = btnSubmit; // Enter submits

            btnBack = new Button
            {
                Text = "Back",
                Location = new Point(670, 568),
                Size = new Size(80, 38),
                FlatStyle = FlatStyle.Flat,
                TabIndex = 12
            };
            btnBack.Click += (s, e) => Close();
            this.CancelButton = btnBack; // Esc back

            // Add controls
            Controls.Add(lblTitle);

            Controls.Add(lblLocation);
            Controls.Add(txtLocation);
            Controls.Add(lblCategory);
            Controls.Add(cboCategory);

            Controls.Add(lblDescription);
            Controls.Add(rtbDescription);
            Controls.Add(lblCount);

            Controls.Add(lblAttachments);
            Controls.Add(lstFiles);
            Controls.Add(btnAttach);
            Controls.Add(btnOpen);
            Controls.Add(btnRemove);

            Controls.Add(grpContact);

            Controls.Add(pbSubmit);
            Controls.Add(lblTip);
            Controls.Add(btnBack);
            Controls.Add(btnSubmit);
        }

        // ================== Actions ==================

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
                        var path = dlg.FileNames[i];
                        if (!lstFiles.Items.Contains(path))
                            lstFiles.Items.Add(path);
                    }
                    lblTip.Text = "Nice! Attachments added.";
                }
            }
        }

        private void OpenSelectedAttachment()
        {
            if (lstFiles.SelectedItem == null) return;
            try
            {
                var path = lstFiles.SelectedItem.ToString();
                if (File.Exists(path))
                    Process.Start(path);
                else
                    MessageBox.Show("File not found on disk.", "Open Attachment",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch { /* noop */ }
        }

        private void RemoveSelectedAttachment()
        {
            if (lstFiles.SelectedItem == null) return;
            int idx = lstFiles.SelectedIndex;
            lstFiles.Items.RemoveAt(idx);
            lblTip.Text = "Attachment removed.";
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            // Inline validation with focus guidance
            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Please enter a location.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLocation.Focus(); return;
            }
            if (cboCategory.SelectedItem == null)
            {
                MessageBox.Show("Please select a category.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboCategory.DroppedDown = true; return;
            }
            if (string.IsNullOrWhiteSpace(rtbDescription.Text))
            {
                MessageBox.Show("Please describe the issue.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtbDescription.Focus(); return;
            }
            if ((rbSMS.Checked || rbWhatsApp.Checked) && string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                MessageBox.Show("Phone number required for SMS/WhatsApp.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhone.Focus(); return;
            }

            // Build model
            var issue = new Issue();
            issue.Location = txtLocation.Text.Trim();
            issue.Category = (IssueCategory)cboCategory.SelectedItem;
            issue.Description = rtbDescription.Text.Trim();

            // Attachments -> DynamicArray<string>
            var attachments = new DynamicArray<string>();
            for (int i = 0; i < lstFiles.Items.Count; i++)
            {
                string p = lstFiles.Items[i].ToString();
                if (!string.IsNullOrEmpty(p)) attachments.Add(p);
            }
            issue.Attachments = attachments;

            // Channel & phone
            issue.PreferredChannel =
                rbSMS.Checked ? ContactChannel.SMS :
                (rbWhatsApp.Checked ? ContactChannel.WhatsApp : ContactChannel.InApp);
            issue.PhoneNumber = txtPhone.Text.Trim();

            // Progress feedback
            pbSubmit.Visible = true; pbSubmit.Value = 10;
            lblTip.Text = "Submitting… please wait.";

            // Copy attachments to local folder (demo)
            string saveDir = Path.Combine(Directory.GetCurrentDirectory(), "Attachments", issue.Id.ToString());
            Directory.CreateDirectory(saveDir);
            foreach (string file in issue.Attachments)
            {
                try
                {
                    string name = Path.GetFileName(file);
                    if (!string.IsNullOrEmpty(name))
                    {
                        string dest = Path.Combine(saveDir, name);
                        if (File.Exists(file)) File.Copy(file, dest, true);
                    }
                }
                catch { /* ignore copy errors in demo */ }
            }
            pbSubmit.Value = 60;

            await Task.Delay(300); // tiny UX pause

            IssueStore.Add(issue);
            pbSubmit.Value = 100;

            MessageBox.Show(
                "Thank you! Your report has been logged.\n\n"
                + "Reference: " + issue.Id + "\n"
                + "Channel: " + issue.PreferredChannel,
                "Report Submitted",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            lblTip.Text = "Report captured. You can submit another or go back to the main menu.";
            pbSubmit.Visible = false;
            ClearForm();
        }

        private void ClearForm()
        {
            txtLocation.Clear();
            if (cboCategory.Items.Count > 0) cboCategory.SelectedIndex = 0;
            rtbDescription.Clear();
            lstFiles.Items.Clear();
            rbInApp.Checked = true;
            txtPhone.Clear();
            rtbDescription.Focus();
        }
    }
}
