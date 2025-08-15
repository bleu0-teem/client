using System;
using System.Drawing;
using System.Windows.Forms;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BLUE16Client
{
    public partial class VersionList : Form
    {
        public string? SelectedVersion { get; private set; }
        public VersionInfo? SelectedVersionInfo { get; private set; }
        private void ApplyTheme()
        {
            if (SettingsStore.DarkMode)
            {
                this.BackColor = Color.FromArgb(32, 32, 32);
                listBox1.BackColor = Color.FromArgb(45, 45, 45);
                listBox1.ForeColor = Color.White;
                foreach (Control c in this.Controls)
                {
                    if (c is Label)
                        c.ForeColor = Color.White;
                    else if (c is Button)
                    {
                        c.BackColor = Color.FromArgb(60, 60, 60);
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
                        c.BackColor = SystemColors.Control;
                        c.ForeColor = Color.Black;
                    }
                }
            }
        }

        private Font ubuntuFont = new Font("Ubuntu", 10F, FontStyle.Regular);

        public VersionList()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            ApplyTheme();
            this.Font = ubuntuFont;
            listBox1.Font = new Font("Ubuntu", 12F, FontStyle.Regular);
            listBox1.MouseDoubleClick += listBox1_MouseDoubleClick;
            listBox1.KeyDown += listBox1_KeyDown;
            listBox1.Items.Clear();
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.ItemHeight = 32;
            listBox1.DrawItem += listBox1_DrawItem;
            LoadVersionsAsync();
        }

        private async void LoadVersionsAsync()
        {
            try
            {
                using var client = new HttpClient();
                var jsonUrl = SettingsStore.GetResourceUrl("src/json/clientversions.json");
                var json = await client.GetStringAsync(jsonUrl);
                var result = JsonSerializer.Deserialize<VersionListResponse>(json);
                if (result != null && result.offline)
                {
                    MessageBox.Show(result.message ?? "Server is offline.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Home.SetStatusDisconnected();
                    return;
                }
                if (result?.versions != null)
                {
                    var infos = new List<VersionInfo>();
                    foreach (var v in result.versions)
                    {
                        if (v.@public) // Only show public versions
                        {
                            infos.Add(new VersionInfo {
                                Name = v.version,
                                Status = v.vulnerable ? "Vulnerable" : "Stable",
                                AltName = v.altname ?? string.Empty,
                                Desc = v.desc ?? string.Empty,
                                DownloadUrl = v.download_url,
                                ExecutableFile = v.executable_file,
                                PatchBy = v.patchby,
                                Offline = v.offline,
                                Vulnerable = v.vulnerable,
                                LaunchArguments = v.launch_arguments
                            });
                        }
                    }
                    listBox1.Items.AddRange(infos.ToArray());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load versions: {ex.Message}");
            }
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();
            var version = listBox1.Items[e.Index] as VersionInfo;
            if (version != null)
            {
                var font = new Font("Ubuntu", 12F, FontStyle.Regular);
                var bounds = e.Bounds;
                var g = e.Graphics;
                // Draw version name
                Brush nameBrush = SettingsStore.DarkMode ? Brushes.White : Brushes.Black;
                g.DrawString(version.Name, font, nameBrush, bounds.Left + 10, bounds.Top + 6);
                // Draw status with color
                Color statusColor = Color.Gray;
                if (version.Status == "Stable") statusColor = Color.Green;
                else if (version.Status == "Unstable") statusColor = Color.Orange;
                else if (version.Status == "Broken") statusColor = Color.Red;
                else if (version.Status == "Vulnerable") statusColor = Color.OrangeRed;
                else if (version.Status == "Private") statusColor = Color.Gray;
                using (Brush statusBrush = new SolidBrush(statusColor))
                {
                    g.DrawString(version.Status, font, statusBrush, bounds.Right - 110, bounds.Top + 6);
                }
            }
            e.DrawFocusRectangle();
        }

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
            if (e.KeyCode == Keys.Enter && listBox1.SelectedItem != null)
            {
                ConfirmSelection();
                e.Handled = true;
            }
        }

        private void ConfirmSelection()
        {
            if (listBox1.SelectedItem is VersionInfo version)
            {
                SelectedVersion = version.Name;
                SelectedVersionInfo = version;
                if (version.Offline)
                {
                    MessageBox.Show("Warning: Online features are not available for this version. Server selection disabled while this version is selected.", "Offline Version", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                if (version.Vulnerable)
                {
                    MessageBox.Show("Warning: This version is marked as vulnerable! It is not reccomended to play in unknown public servers with this.", "Vulnerable Version", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        public class VersionInfo
        {
            public string Name { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty;
            public string AltName { get; set; } = string.Empty;
            public string Desc { get; set; } = string.Empty;
            public string? DownloadUrl { get; set; }
            public string? ExecutableFile { get; set; }
            public string? PatchBy { get; set; }
            public bool Offline { get; set; }
            public bool Vulnerable { get; set; }
            public string? LaunchArguments { get; set; }
            public override string ToString() => Name;
        }

        public class VersionListResponse
        {
            public List<VersionJson>? versions { get; set; }
            public bool offline { get; set; }
            public string? message { get; set; }
        }
        public class VersionJson
        {
            public string version { get; set; } = string.Empty;
            public string? download_url { get; set; }
            public string? rcc_version { get; set; }
            public string? launch_arguments { get; set; }
            public bool vulnerable { get; set; }
            public bool @public { get; set; }
            public bool offline { get; set; }
            public string? executable_file { get; set; }
            public string? patchby { get; set; }
            public string? author { get; set; }
            public string? altname { get; set; }
            public string? desc { get; set; }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Do nothing here (no auto-close on selection)
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
}
