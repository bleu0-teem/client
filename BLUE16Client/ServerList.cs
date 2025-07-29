using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BLUE16Client
{
    public partial class ServerList : Form
    {
        public ServerList()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.ItemHeight = 40;
            listBox1.DrawItem += listBox1_DrawItem;
            listBox1.MouseDoubleClick += listBox1_MouseDoubleClick;
            listBox1.KeyDown += listBox1_KeyDown;
            // Example server data
            var servers = new List<ServerInfo>
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
            listBox1.Items.Clear();
            listBox1.Items.AddRange(servers.ToArray());
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();
            var server = listBox1.Items[e.Index] as ServerInfo;
            if (server != null)
            {
                var font = e.Font;
                var bounds = e.Bounds;
                var g = e.Graphics;
                // Calculate vertical center
                int centerY = bounds.Top + bounds.Height / 2;
                int dotRadius = 7;
                int dotX = bounds.Left + 10;
                int dotY = centerY - dotRadius;
                // Draw filled green circle (dot) with black outline
                using (Brush dotBrush = new SolidBrush(Color.Green))
                using (Pen dotPen = new Pen(Color.Black, 2))
                {
                    g.FillEllipse(dotBrush, dotX, dotY, dotRadius * 2, dotRadius * 2);
                    g.DrawEllipse(dotPen, dotX, dotY, dotRadius * 2, dotRadius * 2);
                }
                // Draw online count (e.g., 10/12) next to dot in green
                string onlineText = $"{server.OnlineCount}/12";
                var onlineFont = new Font(font.FontFamily, font.Size, FontStyle.Bold);
                var onlineSize = g.MeasureString(onlineText, onlineFont);
                int onlineX = dotX + dotRadius * 2 + 8;
                int onlineY = centerY - (int)(onlineSize.Height / 2);
                g.DrawString(onlineText, onlineFont, Brushes.Green, onlineX, onlineY);
                // Draw server name centered horizontally
                string nameText = server.Name;
                var nameSize = g.MeasureString(nameText, font);
                int nameX = bounds.Left + (bounds.Width - (int)nameSize.Width) / 2;
                int nameY = centerY - (int)(nameSize.Height / 2);
                g.DrawString(nameText, font, Brushes.Black, nameX, nameY);

                // Draw host right-aligned and vertically centered (calculate hostX early for badge logic)
                string hostText = $"Host: {server.Owner}";
                var hostSize = g.MeasureString(hostText, font);
                int hostX = bounds.Right - (int)hostSize.Width - 10;
                int hostY = centerY - (int)(hostSize.Height / 2);

                // Draw OFFICIAL badge if criteria met
                if (server.Name.ToLower().Contains("official") && server.Owner.Equals("BLUE16", StringComparison.OrdinalIgnoreCase))
                {
                    string badge = "OFFICIAL";
                    var badgeFont = new Font(font.FontFamily, font.Size, FontStyle.Bold);
                    var badgeSize = g.MeasureString(badge, badgeFont);
                    int badgeX = nameX + (int)nameSize.Width + 12;
                    int badgeY = centerY - (int)(badgeSize.Height / 2);
                    // Ensure badge does not overlap host text
                    if (badgeX + badgeSize.Width > hostX)
                    {
                        badgeX = hostX - (int)badgeSize.Width - 8;
                    }
                    // Draw background rectangle for badge
                    Rectangle badgeRect = new Rectangle(badgeX - 4, badgeY - 2, (int)badgeSize.Width + 8, (int)badgeSize.Height + 4);
                    using (Brush bgBrush = new SolidBrush(Color.FromArgb(255, 255, 215, 0))) // Gold background
                    using (Pen borderPen = new Pen(Color.Orange, 1))
                    {
                        g.FillRectangle(bgBrush, badgeRect);
                        g.DrawRectangle(borderPen, badgeRect);
                    }
                    using (Brush badgeBrush = new SolidBrush(Color.Black))
                    {
                        g.DrawString(badge, badgeFont, badgeBrush, badgeX, badgeY);
                    }
                }

                // Draw host (after badge logic)
                using (Brush hostBrush = new SolidBrush(Color.DarkOrange))
                {
                    g.DrawString(hostText, font, hostBrush, hostX, hostY);
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
