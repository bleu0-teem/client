using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace BLUE16Client
{
    public partial class ServerList : Form
    {
        private Panel headerPanel;
        private TextBox searchBox;
        private TextBox ipBox;
        private Button ipConnectButton;
        private int hoveredIndex = -1;
        private List<ServerInfo> allServers = new List<ServerInfo>();
        private bool isOnlineEnabled = true;

        // Custom theme function
        public void ApplyCustomTheme(Color backColor, Color foreColor, Font? font = null)
        {
            this.BackColor = backColor;
            foreach (Control c in this.Controls)
            {
                c.BackColor = backColor;
                c.ForeColor = foreColor;
                if (font != null) c.Font = font;
            }
            listBox1.BackColor = backColor;
            listBox1.ForeColor = foreColor;
            if (font != null) listBox1.Font = font;
        }

        // Call this to disable server selection for offline versions
        public void DisableOnlineSelection()
        {
            isOnlineEnabled = false;
            listBox1.Enabled = false;
            searchBox.Enabled = false;
            ipBox.Enabled = false;
            ipConnectButton.Enabled = false;
            createServerButton.Enabled = false;
        }

        private Font ubuntuFont = new Font("Ubuntu", 10F, FontStyle.Regular);

        private void InitializeCustomComponents()
        {
            // Header Panel
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = SettingsStore.DarkMode ? Color.FromArgb(36, 37, 38) : Color.WhiteSmoke,
                Padding = new Padding(0, 0, 0, 0)
            };
            var headerLabel = new Label
            {
                Text = "Select Server",
                Font = new Font("Ubuntu", 18F, FontStyle.Bold),
                ForeColor = SettingsStore.DarkMode ? Color.White : Color.Black,
                Dock = DockStyle.Left,
                Width = 250,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0)
            };
            searchBox = new TextBox
            {
                Font = new Font("Ubuntu", 12F, FontStyle.Regular),
                Width = 220,
                Height = 32,
                Location = new Point(headerPanel.Width - 250, 14),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                PlaceholderText = "Search..."
            };
            searchBox.TextChanged += (s, e) => FilterServers();
            headerPanel.Controls.Add(headerLabel);
            headerPanel.Controls.Add(searchBox);

            // IP entry UI
            ipBox = new TextBox
            {
                Font = new Font("Ubuntu", 10F, FontStyle.Regular),
                Width = 180,
                Height = 28,
                Location = new Point(headerPanel.Width - 480, 16),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                PlaceholderText = "Enter IP to join..."
            };
            ipConnectButton = new Button
            {
                Text = "Join by IP",
                Font = new Font("Ubuntu", 10F, FontStyle.Regular),
                Location = new Point(headerPanel.Width - 290, 16),
                Size = new Size(90, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            ipConnectButton.Click += IpConnectButton_Click;
            headerPanel.Controls.Add(ipBox);
            headerPanel.Controls.Add(ipConnectButton);
            this.Controls.Add(headerPanel);

            // ListBox design
            listBox1.BorderStyle = BorderStyle.None;
            listBox1.Padding = new Padding(0);
            listBox1.BackColor = SettingsStore.DarkMode ? Color.FromArgb(32, 32, 32) : Color.White;
            listBox1.ForeColor = SettingsStore.DarkMode ? Color.White : Color.Black;
            listBox1.MouseMove += (s, e) => {
                int idx = listBox1.IndexFromPoint(e.Location);
                if (hoveredIndex != idx) { hoveredIndex = idx; listBox1.Invalidate(); }
            };
            listBox1.MouseLeave += (s, e) => { hoveredIndex = -1; listBox1.Invalidate(); };
        }

        private void IpConnectButton_Click(object sender, EventArgs e)
        {
            string ip = ipBox.Text.Trim();
            if (string.IsNullOrEmpty(ip) || !IPAddress.TryParse(ip, out _))
            {
                MessageBox.Show("Please enter a valid IP address.", "Invalid IP", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SelectedServer = ip;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void FilterServers()
        {
            string query = searchBox.Text.Trim().ToLower();
            listBox1.Items.Clear();
            foreach (var s in allServers)
            {
                if (string.IsNullOrEmpty(query) || s.Name.ToLower().Contains(query) || s.Owner.ToLower().Contains(query))
                    listBox1.Items.Add(s);
            }
        }

        private void LoadServers()
        {
            // Example server data (could be replaced with real data loading)
            allServers = new List<ServerInfo>
            {
                new ServerInfo { Name = "First server", OnlineCount = 12, Owner = "noobie" },
                new ServerInfo { Name = "one server", OnlineCount = 5, Owner = "Maxf3m" },
                new ServerInfo { Name = "hi server", OnlineCount = 20, Owner = "techplayz" },
                new ServerInfo { Name = "no server", OnlineCount = 0, Owner = "skvertyy" },
                new ServerInfo { Name = "Way looooooooooooong server", OnlineCount = 4, Owner = "utrenkl" },
                new ServerInfo { Name = "h", OnlineCount = 3, Owner = "waynelera" },
                new ServerInfo { Name = "Natural Disaster Survival", OnlineCount = 11, Owner = "Roblox" },
                new ServerInfo { Name = "Hiii!", OnlineCount = 5, Owner = "BLUE16" },
                new ServerInfo { Name = "Gay", OnlineCount = 12, Owner = "clonedpidoras" },
                new ServerInfo { Name = "OFFICIAL SERVER BLUE16 REAL 100%", OnlineCount = 5, Owner = "BIue16" },
                new ServerInfo { Name = "hi server", OnlineCount = 20, Owner = "bot1" },
                new ServerInfo { Name = "no server", OnlineCount = 0, Owner = "bot2" },
                new ServerInfo { Name = "Way looooooooooooong server", OnlineCount = 4, Owner = "bot3" },
                new ServerInfo { Name = "h", OnlineCount = 3, Owner = "bot4" },
                new ServerInfo { Name = "Natural Disaster Survival", OnlineCount = 11, Owner = "bot5" },
                new ServerInfo { Name = "Bye!", OnlineCount = 5, Owner = "bot6" }
            };
            FilterServers();
        }

        public ServerList()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            // Use default theme on startup
            ApplyCustomTheme(SettingsStore.DarkMode ? Color.FromArgb(32, 32, 32) : SystemColors.Control,
                             SettingsStore.DarkMode ? Color.White : Color.Black,
                             ubuntuFont);
            InitializeCustomComponents();
            this.Font = ubuntuFont;
            listBox1.Font = new Font("Ubuntu", 12F, FontStyle.Regular);
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.ItemHeight = 56;
            listBox1.DrawItem += listBox1_DrawItem;
            listBox1.MouseDoubleClick += listBox1_MouseDoubleClick;
            listBox1.KeyDown += listBox1_KeyDown;
            LoadServers();
            ApplyTheme();

            // Accessibility
            listBox1.AccessibleName = "Server list";
            listBox1.AccessibleDescription = "List of available servers to join";
            if (ipBox != null)
            {
                ipBox.AccessibleName = "IP address";
                ipBox.AccessibleDescription = "Enter server IP to join";
            }
            if (ipConnectButton != null)
            {
                ipConnectButton.AccessibleName = "Join by IP";
                ipConnectButton.AccessibleDescription = "Join the server by IP address";
            }

            // Context menu for servers
            var ctx = new ContextMenuStrip();
            var joinItem = new ToolStripMenuItem("Join");
            var copyName = new ToolStripMenuItem("Copy Name");
            ctx.Items.AddRange(new ToolStripItem[] { joinItem, copyName });
            listBox1.ContextMenuStrip = ctx;

            listBox1.MouseUp += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    int index = listBox1.IndexFromPoint(e.Location);
                    if (index != ListBox.NoMatches)
                    {
                        listBox1.SelectedIndex = index;
                    }
                }
            };
            joinItem.Click += (s, e) => { ConfirmSelection(); };
            copyName.Click += (s, e) =>
            {
                if (listBox1.SelectedItem is ServerInfo si)
                {
                    try { Clipboard.SetText(si.Name); } catch { }
                }
            };
        }

        private void ApplyTheme()
        {
            if (SettingsStore.CurrentCustomTheme != null)
            {
                var theme = SettingsStore.CurrentCustomTheme;
                this.BackColor = theme.BackColor;
                listBox1.BackColor = theme.PanelColor;
                listBox1.ForeColor = theme.ForeColor;
                foreach (Control c in this.Controls)
                {
                    if (c is Label)
                        c.ForeColor = theme.LabelColor;
                    else if (c is Button)
                    {
                        c.BackColor = theme.ButtonColor;
                        c.ForeColor = theme.ForeColor;
                    }
                    if (theme.MainFont != null) c.Font = theme.MainFont;
                }
            }
            else if (SettingsStore.DarkMode)
            {
                this.BackColor = Color.FromArgb(32, 32, 32);
                listBox1.BackColor = Color.FromArgb(45, 45, 55);
                listBox1.ForeColor = Color.White;
                foreach (Control c in this.Controls)
                {
                    if (c is Label)
                        c.ForeColor = Color.White;
                    else if (c is Button)
                    {
                        c.BackColor = Color.FromArgb(55, 55, 65);
                        c.ForeColor = Color.White;
                    }
                    if (c is TextBox)
                    {
                        c.BackColor = Color.FromArgb(50, 50, 60);
                        c.ForeColor = Color.White;
                    }
                }
            }
            else
            {
                this.BackColor = SystemColors.Control;
                listBox1.BackColor = Color.White;
                listBox1.ForeColor = Color.Black;
                foreach (Control c in this.Controls)
                {
                    if (c is Label)
                        c.ForeColor = Color.Black;
                    else if (c is Button)
                    {
                        c.BackColor = Color.FromArgb(230, 230, 240);
                        c.ForeColor = Color.Black;
                    }
                    if (c is TextBox)
                    {
                        c.BackColor = Color.White;
                        c.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            var server = listBox1.Items[e.Index] as ServerInfo;
            var bounds = e.Bounds;
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            Color cardColor = hoveredIndex == e.Index
                ? (SettingsStore.DarkMode ? Color.FromArgb(55, 55, 65) : Color.FromArgb(230, 230, 240))
                : (SettingsStore.DarkMode ? Color.FromArgb(45, 45, 55) : Color.FromArgb(245, 245, 250));
            Color borderColor = hoveredIndex == e.Index
                ? (SettingsStore.DarkMode ? Color.FromArgb(80, 120, 255) : Color.FromArgb(80, 120, 255))
                : (SettingsStore.DarkMode ? Color.FromArgb(60, 60, 70) : Color.FromArgb(220, 220, 230));
            // Card shadow
            Rectangle shadowRect = new Rectangle(bounds.Left + 2, bounds.Top + 4, bounds.Width - 4, bounds.Height - 2);
            using (SolidBrush shadowBrush = new SolidBrush(Color.FromArgb(30, 0, 0, 0)))
                g.FillRectangle(shadowBrush, shadowRect);
            // Card background
            using (GraphicsPath path = new GraphicsPath())
            {
                int radius = 12;
                path.AddArc(bounds.Left, bounds.Top, radius, radius, 180, 90);
                path.AddArc(bounds.Right - radius, bounds.Top, radius, radius, 270, 90);
                path.AddArc(bounds.Right - radius, bounds.Bottom - radius, radius, radius, 0, 90);
                path.AddArc(bounds.Left, bounds.Bottom - radius, radius, radius, 90, 90);
                path.CloseFigure();
                using (SolidBrush cardBrush = new SolidBrush(cardColor))
                    g.FillPath(cardBrush, path);
                using (Pen borderPen = new Pen(borderColor, hoveredIndex == e.Index ? 2 : 1))
                    g.DrawPath(borderPen, path);
            }
            // Online icon
            int iconX = bounds.Left + 16;
            int iconY = bounds.Top + 16;
            Color onlineColor = server.OnlineCount > 0 ? Color.LimeGreen : Color.Gray;
            using (Brush onlineBrush = new SolidBrush(onlineColor))
                g.FillEllipse(onlineBrush, iconX, iconY, 20, 20);
            using (Pen outlinePen = new Pen(Color.White, 2))
                g.DrawEllipse(outlinePen, iconX, iconY, 20, 20);
            // Server name
            var font = new Font("Ubuntu", 13F, FontStyle.Bold);
            var subFont = new Font("Ubuntu", 10F, FontStyle.Regular);
            Brush nameBrush = SettingsStore.DarkMode ? Brushes.White : Brushes.Black;
            g.DrawString(server.Name, font, nameBrush, bounds.Left + 48, bounds.Top + 8);
            // Host
            g.DrawString($"Host: {server.Owner}", subFont, SettingsStore.DarkMode ? Brushes.Gainsboro : Brushes.DimGray, bounds.Left + 48, bounds.Top + 30);
            // Online count
            string statusText = $"{server.OnlineCount}/12 online";
            var statusBrush = new SolidBrush(server.OnlineCount > 0 ? Color.LimeGreen : Color.Gray);
            g.DrawString(statusText, subFont, statusBrush, bounds.Right - 110, bounds.Top + 8);
            // OFFICIAL badge
            if (server.Name.ToLower().Contains("official") && server.Owner.Equals("BLUE16", StringComparison.OrdinalIgnoreCase))
            {
                string badge = "OFFICIAL";
                var badgeFont = new Font("Ubuntu", 10F, FontStyle.Bold);
                var badgeSize = g.MeasureString(badge, badgeFont);
                int badgeX = bounds.Right - (int)badgeSize.Width - 20;
                int badgeY = bounds.Top + 32;
                Rectangle badgeRect = new Rectangle(badgeX - 4, badgeY - 2, (int)badgeSize.Width + 8, (int)badgeSize.Height + 4);
                using (Brush bgBrush = new SolidBrush(Color.FromArgb(255, 255, 215, 0)))
                using (Pen borderPen = new Pen(Color.Orange, 1))
                {
                    g.FillRectangle(bgBrush, badgeRect);
                    g.DrawRectangle(borderPen, badgeRect);
                }
                using (Brush badgeBrush = new SolidBrush(SettingsStore.DarkMode ? Color.White : Color.Black))
                {
                    g.DrawString(badge, badgeFont, badgeBrush, badgeX, badgeY);
                }
            }
            // Not Verified badge for custom domains
            if (SettingsStore.ServerDomain != "github.com" && SettingsStore.ServerDomain != "blue16.site" && SettingsStore.ServerDomain != "blue16-web.vercel.app")
            {
                string badge = "NOT VERIFIED";
                var badgeFont = new Font("Ubuntu", 10F, FontStyle.Bold);
                var badgeSize = g.MeasureString(badge, badgeFont);
                int badgeX = bounds.Right - (int)badgeSize.Width - 120;
                int badgeY = bounds.Top + 32;
                Rectangle badgeRect = new Rectangle(badgeX - 4, badgeY - 2, (int)badgeSize.Width + 8, (int)badgeSize.Height + 4);
                using (Brush bgBrush = new SolidBrush(Color.FromArgb(255, 200, 50, 50)))
                using (Pen borderPen = new Pen(Color.Red, 1))
                {
                    g.FillRectangle(bgBrush, badgeRect);
                    g.DrawRectangle(borderPen, badgeRect);
                }
                using (Brush badgeBrush = new SolidBrush(SettingsStore.DarkMode ? Color.White : Color.Black))
                {
                    g.DrawString(badge, badgeFont, badgeBrush, badgeX, badgeY);
                }
            }
            e.DrawFocusRectangle();
        }

        public string? SelectedServer { get; private set; }

        private void listBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            int index = listBox1.IndexFromPoint(e.Location);
            if (index != ListBox.NoMatches)
            {
                listBox1.SelectedIndex = index;
                ConfirmSelection();
            }
        }

        private void listBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && listBox1.SelectedItem is ServerInfo)
            {
                ConfirmSelection();
                e.Handled = true;
            }
        }

        private void ConfirmSelection()
        {
            if (listBox1.SelectedItem is ServerInfo server)
            {
                SelectedServer = server.Name;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Do nothing here (no auto-close on selection)
        }

        private void createServerButton_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Create server functionality coming soon!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Allow arrow key navigation in listBox1
            if (listBox1.Focused)
            {
                if (keyData == Keys.Up && listBox1.SelectedIndex > 0)
                {
                    listBox1.SelectedIndex--;
                    return true;
                }
                if (keyData == Keys.Down && listBox1.SelectedIndex < listBox1.Items.Count - 1)
                {
                    listBox1.SelectedIndex++;
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    public class ServerInfo
    {
        public string Name { get; set; } = string.Empty;
        public int OnlineCount { get; set; }
        public string Owner { get; set; } = string.Empty;
        public override string ToString() => Name;
    }
}
