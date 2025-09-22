using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BLUE16Client
{
    public class PluginsForm : Form
    {
        private ListView list;
        private Button enableBtn;
        private Button disableBtn;
        private Button refreshBtn;
        private Label infoLabel;

        public PluginsForm()
        {
            this.Text = "Plugins";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(560, 360);

            list = new ListView
            {
                Left = 12,
                Top = 12,
                Width = 536,
                Height = 260,
                View = View.Details,
                FullRowSelect = true,
                MultiSelect = false
            };
            list.Columns.Add("Name", 180);
            list.Columns.Add("Version", 80);
            list.Columns.Add("Author", 120);
            list.Columns.Add("Enabled", 80);

            enableBtn = new Button { Left = 12, Top = 284, Width = 90, Text = "Enable" };
            disableBtn = new Button { Left = 108, Top = 284, Width = 90, Text = "Disable" };
            refreshBtn = new Button { Left = 204, Top = 284, Width = 90, Text = "Refresh" };
            infoLabel = new Label { Left = 12, Top = 318, Width = 536, Height = 24, Text = "Select a plugin to view details." };

            enableBtn.Click += (s, e) => ToggleSelected(true);
            disableBtn.Click += (s, e) => ToggleSelected(false);
            refreshBtn.Click += (s, e) => LoadPlugins();
            list.SelectedIndexChanged += (s, e) => UpdateInfo();

            this.Controls.Add(list);
            this.Controls.Add(enableBtn);
            this.Controls.Add(disableBtn);
            this.Controls.Add(refreshBtn);
            this.Controls.Add(infoLabel);

            ApplyTheme();
            LoadPlugins();
        }

        private void ApplyTheme()
        {
            if (SettingsStore.CurrentCustomTheme != null)
            {
                var t = SettingsStore.CurrentCustomTheme;
                this.BackColor = t.BackColor;
                foreach (Control c in this.Controls)
                {
                    if (c is Label) c.ForeColor = t.LabelColor; else c.ForeColor = t.ForeColor;
                    if (c is Button btn) { btn.BackColor = t.ButtonColor; btn.ForeColor = t.ForeColor; }
                }
                list.BackColor = t.PanelColor; list.ForeColor = t.ForeColor;
                return;
            }
            if (SettingsStore.DarkMode)
            {
                this.BackColor = Color.FromArgb(32, 32, 32);
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                    if (c is Button btn) { btn.BackColor = Color.FromArgb(60, 60, 60); btn.ForeColor = Color.White; }
                }
                list.BackColor = Color.FromArgb(45, 45, 45); list.ForeColor = Color.White;
            }
            else
            {
                this.BackColor = SystemColors.Control;
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.Black;
                    if (c is Button btn) { btn.BackColor = SystemColors.Control; btn.ForeColor = Color.Black; }
                }
                list.BackColor = Color.White; list.ForeColor = Color.Black;
            }
        }

        private void LoadPlugins()
        {
            list.Items.Clear();
            foreach (var p in PluginManager.Plugins)
            {
                var it = new ListViewItem(new[] { p.Name, p.Version, p.Author, p.Enabled ? "Yes" : "No" });
                it.Tag = p.Id;
                list.Items.Add(it);
            }
            UpdateInfo();
        }

        private void ToggleSelected(bool enabled)
        {
            if (list.SelectedItems.Count == 0) return;
            var id = list.SelectedItems[0].Tag as string;
            if (string.IsNullOrEmpty(id)) return;
            PluginManager.SetEnabled(id, enabled);
            LoadPlugins();
        }

        private void UpdateInfo()
        {
            if (list.SelectedItems.Count == 0)
            {
                infoLabel.Text = "Select a plugin to view details.";
                return;
            }
            var id = list.SelectedItems[0].Tag as string;
            var p = PluginManager.Plugins.FirstOrDefault(x => x.Id == id);
            if (p == null)
            {
                infoLabel.Text = "";
                return;
            }
            infoLabel.Text = $"{p.Name} v{p.Version} by {p.Author} — {(p.Enabled ? "Enabled" : "Disabled")}\n{p.Description}";
        }
    }
}
