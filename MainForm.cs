using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace MunicipalServicesApp
{
    public class MainForm : Form
    {
        // Theme palette
        private static readonly Color Primary = Color.FromArgb(0, 102, 84);
        private static readonly Color PrimaryDark = Color.FromArgb(0, 82, 68);
        private static readonly Color Accent = Color.FromArgb(255, 179, 0);
        private static readonly Color Surface = Color.FromArgb(245, 247, 250);
        private static readonly Color CardBorder = Color.FromArgb(225, 228, 232);
        private static readonly Color TextBody = Color.FromArgb(33, 37, 41);
        private static readonly Color TextMuted = Color.FromArgb(73, 80, 87);

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
            MinimumSize = new Size(1000, 680);
            DoubleBuffered = true;
        }

        private void InitializeComponent()
        {
            tip = new ToolTip();
            BackColor = Surface;

            // ===== Header =====
            header = new Panel { Dock = DockStyle.Top, Height = 128, BackColor = Color.White };
            header.Paint += Header_Paint;

            lblTitle = new Label
            {
                AutoSize = true,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI Semibold", 22f, FontStyle.Bold),
                Text = "City of e-Services"
            };
            lblSubtitle = new Label
            {
                AutoSize = true,
                ForeColor = Color.Black,
                Font = new Font("Segoe UI", 12f, FontStyle.Regular),
                Text = "Report issues, stay informed, and track service delivery in your community."
            };

            header.Controls.Add(lblTitle);
            header.Controls.Add(lblSubtitle);
            header.Resize += (s, e) =>
            {
                lblTitle.Location = new Point(28, 26);
                lblSubtitle.Location = new Point(30, 68);
            };
            Controls.Add(header);

            // ===== 3-column grid with generous gutters =====
            grid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Surface,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(36, 24, 36, 32), // page padding
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
                        f.ShowDialog(this);
                    }
                },
                true,
                "Live"
            );

            var cardEvents = CreateNavCard(
                "Local Events & Notices",
                "Find municipal announcements, planned outages, public meetings, and community events.",
                (s, e) =>
                {
                    using (var f = new LocalEventsForm())
                    {
                        f.StartPosition = FormStartPosition.CenterParent;
                        f.ShowDialog(this);
                    }
                },
                true,
                "Updated"
            );

            var cardStatus = CreateNavCard(
                "Service Request Status",
                "Find a request by reference, view its timeline, route through departments, and see oldest open items.",
                (s, e) =>
                {
                    using (var f = new ServiceRequestStatusForm())
                    {
                        f.StartPosition = FormStartPosition.CenterParent;
                        f.ShowDialog(this);
                    }
                },
                true,
                "New"
            );

            grid.Controls.Add(cardReport, 0, 0);
            grid.Controls.Add(cardEvents, 1, 0);
            grid.Controls.Add(cardStatus, 2, 0);

            Controls.Add(grid);
        }

        private void Header_Paint(object sender, PaintEventArgs e)
        {
            using (var lg = new LinearGradientBrush(header.ClientRectangle, Color.White, Color.Gainsboro, 0f))
            {
                e.Graphics.FillRectangle(lg, header.ClientRectangle);
            }
            using (var pen = new Pen(Accent, 3))
            {
                e.Graphics.DrawLine(pen, 0, header.Height - 3, header.Width, header.Height - 3);
            }
        }

        private Panel CreateNavCard(string title, string description, EventHandler onClick, bool enabled, string badgeText)
        {
            // Measurements for neat internal spacing
            const int pad = 18;            // inner padding
            const int radius = 16;         // corner radius
            const int btnH = 40;           // button height
            const int btnW = 160;          // button width

            var card = new Panel
            {
                Margin = new Padding(18),     // gutters between cards
                Padding = new Padding(pad),
                BackColor = Color.White,
                Height = 280,                 // slightly taller; consistent height
                Dock = DockStyle.Fill
            };

            // Border and hover state
            card.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, card.Width - 1, card.Height - 1);
                using (var path = RoundedRect(rect, radius))
                using (var borderPen = new Pen(CardBorder, 1))
                {
                    g.DrawPath(borderPen, path);
                }
            };
            card.MouseEnter += (s, e) => card.BackColor = Color.FromArgb(252, 253, 255);
            card.MouseLeave += (s, e) => card.BackColor = Color.White;

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
                Font = new Font("Segoe UI Semibold", 16f, FontStyle.Bold),
                ForeColor = TextBody,
                MaximumSize = new Size(1000, 0)
            };

            var desc = new Label
            {
                AutoSize = false,
                Text = description,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular),
                ForeColor = TextMuted
            };

            var cta = new Button
            {
                Text = enabled ? "Open" : "Unavailable",
                AutoSize = false,
                Height = btnH,
                Width = btnW,
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

                // make the whole card clickable
                card.Cursor = Cursors.Hand;
                card.Click += onClick;
                foreach (Control ctl in new Control[] { badge, lbl, desc })
                    ctl.Click += onClick;
            }
            else
            {
                tip.SetToolTip(cta, "This feature will be available soon");
            }

            // Add controls
            card.Controls.Add(badge);
            card.Controls.Add(lbl);
            card.Controls.Add(desc);
            card.Controls.Add(cta);

            // Responsive layout inside the card (keeps everything neat)
            card.Resize += (s, e) =>
            {
                // inner content width
                int innerW = card.ClientSize.Width - (pad * 2);

                badge.Location = new Point(pad - 12, pad - 6); // nudge up-left a bit
                lbl.Location = new Point(pad - 12, badge.Bottom + 10);

                // Wrap description to inner width, leave room above the button
                int spaceForDesc = card.ClientSize.Height - (lbl.Bottom + 16) - (btnH + 22) - pad;
                if (spaceForDesc < 40) spaceForDesc = 40;

                desc.Location = new Point(pad - 10, lbl.Bottom + 8);
                desc.Size = new Size(innerW, spaceForDesc);

                // Center button horizontally, anchor to bottom
                int btnX = (card.ClientSize.Width - btnW) / 2;
                int btnY = card.ClientSize.Height - btnH - pad;
                cta.Location = new Point(btnX, btnY);
            };

            // Fire initial layout
            card.PerformLayout();

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
    