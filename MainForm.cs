using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public class MainForm : Form
    {
        // Color palette
        private static readonly Color Primary = Color.FromArgb(0, 102, 84);
        private static readonly Color PrimaryDark = Color.FromArgb(0, 82, 68);
        private static readonly Color Accent = Color.FromArgb(255, 179, 0);
        private static readonly Color Surface = Color.FromArgb(245, 247, 250);
        private static readonly Color TextBody = Color.FromArgb(33, 37, 41);

        private Panel header;
        private Label lblTitle;
        private Label lblSubtitle;

        private TableLayoutPanel grid;
        private ToolTip tip;

        public MainForm()
        {
            InitializeComponent();
            Text = "Municipal Services — Home";
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(920, 620);
            DoubleBuffered = true;
        }

        private void InitializeComponent()
        {
            tip = new ToolTip();
            BackColor = Surface;

            // ===== Header (light gradient) =====
            header = new Panel { Dock = DockStyle.Top, Height = 120 };
            header.Paint += Header_Paint;

            lblTitle = new Label
            {
                AutoSize = true,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI Semibold", 20f, FontStyle.Bold),
                Text = "City of e-Services"
            };
            lblSubtitle = new Label
            {
                AutoSize = true,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 11.5f, FontStyle.Regular),
                Text = "Report issues, stay informed, and track service delivery in your community."
            };

            header.Controls.Add(lblTitle);
            header.Controls.Add(lblSubtitle);
            header.Resize += (s, e) =>
            {
                lblTitle.Location = new Point(24, 24);
                lblSubtitle.Location = new Point(26, 64);
            };
            Controls.Add(header);

            // ===== Grid with cards =====
            grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Surface,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(24),
            };
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));

            var cardReport = CreateNavCard(
                "Report an Issue",
                "Log problems like water leaks, electricity faults, road damage, refuse collection, and more.",
                (s, e) =>
                {
                    using (var f = new ReportIssueForm())
                    {
                        f.StartPosition = FormStartPosition.CenterParent;
                        f.ShowDialog(this); // modal with owner
                    }
                },
                true,
                "Live"
            );

            // UPDATED: enable and wire up Local Events & Notices
            var cardEvents = CreateNavCard(
                "Local Events & Notices",
                "Find municipal announcements, planned outages, public meetings, and community events.",
                (s, e) =>
                {
                    using (var f = new LocalEventsForm())
                    {
                        f.StartPosition = FormStartPosition.CenterParent;
                        f.ShowDialog(this); // modal with owner
                    }
                },
                true,           // enabled for Part 2
                "New"
            );

            var cardStatus = CreateNavCard(
                "Track Request Status",
                "Enter a reference to view progress, assigned department, and expected resolution date.",
                null,
                false,
                "Coming soon"
            );

            grid.Controls.Add(cardReport, 0, 0);
            grid.Controls.Add(cardEvents, 1, 0);
            grid.Controls.Add(cardStatus, 2, 0);

            Controls.Add(grid);
        }

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            using (var lg = new LinearGradientBrush(header.ClientRectangle, Color.White, Color.LightGray, 0f))
            {
                e.Graphics.FillRectangle(lg, header.ClientRectangle);
            }
            using (var pen = new Pen(Accent, 3))
            {
                e.Graphics.DrawLine(pen, 0, header.Height - 2, header.Width, header.Height - 2);
            }
        }

        private Panel CreateNavCard(string title, string description, EventHandler onClick, bool enabled, string badgeText)
        {
            var card = new Panel
            {
                Margin = new Padding(12),
                Padding = new Padding(18),
                BackColor = Color.White,
                Height = 220,
                Dock = DockStyle.Fill
            };

            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                using (var path = RoundedRect(rect, 16))
                using (var borderPen = new Pen(Color.FromArgb(225, 228, 232), 1))
                {
                    g.DrawPath(borderPen, path);
                }
            };

            var badge = new Label
            {
                AutoSize = true,
                Text = badgeText.ToUpperInvariant(),
                BackColor = enabled ? Color.FromArgb(232, 247, 238) : Color.FromArgb(248, 249, 250),
                ForeColor = enabled ? Primary : Color.FromArgb(108, 117, 125),
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Padding = new Padding(8, 4, 8, 4)
            };

            var lbl = new Label
            {
                AutoSize = true,
                Text = title,
                Font = new Font("Segoe UI Semibold", 15f, FontStyle.Bold),
                ForeColor = TextBody,
                MaximumSize = new Size(1000, 0)
            };

            var desc = new Label
            {
                AutoSize = false,
                Text = description,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(73, 80, 87),
                MaximumSize = new Size(1000, 0),
                Height = 60
            };

            var cta = new Button
            {
                Text = enabled ? "Open" : "Unavailable",
                AutoSize = false,
                Height = 38,
                Width = 140,
                FlatStyle = FlatStyle.Flat,
                BackColor = enabled ? Primary : Color.FromArgb(233, 236, 239),
                ForeColor = enabled ? Color.White : Color.FromArgb(108, 117, 125),
                Cursor = enabled ? Cursors.Hand : Cursors.No,
                TabStop = enabled
            };
            cta.FlatAppearance.BorderSize = 0;

            if (enabled && onClick != null)
            {
                cta.MouseEnter += (s, e) => cta.BackColor = PrimaryDark;
                cta.MouseLeave += (s, e) => cta.BackColor = Primary;
                cta.Click += onClick;
                tip.SetToolTip(cta, "Click to continue");
            }
            else
            {
                tip.SetToolTip(cta, "This feature will be available soon");
            }

            badge.Location = new Point(6, 6);
            lbl.Location = new Point(6, 44);
            desc.Location = new Point(8, 80);
            desc.Width = card.Width - 32;
            cta.Location = new Point(8, 150);

            card.Resize += (s, e) => { desc.Width = card.Width - 32; };

            card.Controls.Add(badge);
            card.Controls.Add(lbl);
            card.Controls.Add(desc);
            card.Controls.Add(cta);

            if (enabled && onClick != null)
            {
                card.Cursor = Cursors.Hand;
                card.Click += onClick;
                foreach (Control ctl in card.Controls)
                    ctl.Click += onClick;
            }

            return card;
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            var path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
