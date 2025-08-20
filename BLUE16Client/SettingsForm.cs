using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace BLUE16Client
{
    public partial class SettingsForm : Form
    {
        private TextBox textBoxFolder;
        private Button buttonBrowse;

        public SettingsForm()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this.ClientSize = new Size(400, 360); // Make the settings popup bigger
            // Load current settings
            checkBoxDarkMode.Checked = SettingsStore.DarkMode;
            comboBoxLanguage.SelectedItem = SettingsStore.Language;
            checkBoxDiscordRpc.Checked = SettingsStore.EnableDiscordRpc;
            comboBoxDomain.SelectedItem = (SettingsStore.ServerDomain == "github.com" || SettingsStore.ServerDomain == "blue16.site" || SettingsStore.ServerDomain == "blue16-web.vercel.app") ? SettingsStore.ServerDomain : "Custom...";
            if (comboBoxDomain.SelectedItem?.ToString() == "Custom...")
            {
                comboBoxDomain.Items.Add(SettingsStore.ServerDomain);
                comboBoxDomain.SelectedItem = SettingsStore.ServerDomain;
            }
            textBoxFolder = new TextBox { Width = 220, Text = SettingsStore.VersionsFolder ?? string.Empty, Location = new Point(20, 190) };
            buttonBrowse = new Button { Text = "Browse...", Location = new Point(250, 190), Width = 80 };
            buttonBrowse.Click += (s, e) =>
            {
                using (var fbd = new FolderBrowserDialog())
                {
                    fbd.SelectedPath = textBoxFolder.Text;
                    if (fbd.ShowDialog() == DialogResult.OK)
                    {
                        textBoxFolder.Text = fbd.SelectedPath;
                    }
                }
            };
            this.Controls.Add(new Label { Text = "Versions Folder:", Location = new Point(20, 170), Width = 120 });
            this.Controls.Add(textBoxFolder);
            this.Controls.Add(buttonBrowse);
            comboBoxTheme.SelectedItem = SettingsStore.CurrentCustomTheme == null ? (SettingsStore.DarkMode ? "Dark" : "Default") : "Custom...";
            comboBoxTheme.SelectedIndexChanged += (s, e) =>
            {
                if (comboBoxTheme.SelectedItem?.ToString() == "Custom...")
                {
                    MessageBox.Show("To use a custom theme, please import one or create your own.", "Custom Theme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (comboBoxTheme.SelectedItem?.ToString() == "Dark")
                {
                    SettingsStore.DarkMode = true;
                    SettingsStore.CurrentCustomTheme = null;
                    ApplyTheme();
                }
                else
                {
                    SettingsStore.DarkMode = false;
                    SettingsStore.CurrentCustomTheme = null;
                    ApplyTheme();
                }
            };
            buttonImportTheme.Click += (s, e) =>
            {
                using (var ofd = new OpenFileDialog { Filter = "Theme Files (*.theme)|*.theme|All Files (*.*)|*.*" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            var themeJson = File.ReadAllText(ofd.FileName);
                            var theme = System.Text.Json.JsonSerializer.Deserialize<SettingsStore.CustomTheme>(themeJson);
                            if (theme != null)
                            {
                                SettingsStore.CurrentCustomTheme = theme;
                                comboBoxTheme.SelectedItem = "Custom...";
                                ApplyTheme();
                                MessageBox.Show("Custom theme imported!", "Theme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to import theme: {ex.Message}", "Theme Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };
            buttonExportTheme.Click += (s, e) =>
            {
                if (SettingsStore.CurrentCustomTheme == null)
                {
                    MessageBox.Show("No custom theme to export.", "Theme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                using (var sfd = new SaveFileDialog { Filter = "Theme Files (*.theme)|*.theme|All Files (*.*)|*.*" })
                {
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            var themeJson = System.Text.Json.JsonSerializer.Serialize(SettingsStore.CurrentCustomTheme);
                            File.WriteAllText(sfd.FileName, themeJson);
                            MessageBox.Show("Custom theme exported!", "Theme", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Failed to export theme: {ex.Message}", "Theme Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            };
            ApplyTheme();
            comboBoxDomain.SelectedIndexChanged += (s, e) =>
            {
                if (comboBoxDomain.SelectedItem?.ToString() == "Custom...")
                {
                    string input = Microsoft.VisualBasic.Interaction.InputBox("Enter custom domain:", "Custom Domain", "");
                    if (!string.IsNullOrWhiteSpace(input))
                    {
                        if (!comboBoxDomain.Items.Contains(input))
                            comboBoxDomain.Items.Add(input);
                        comboBoxDomain.SelectedItem = input;
                    }
                }
            };
        }

        private void ApplyTheme()
        {
            if (SettingsStore.CurrentCustomTheme != null)
            {
                var theme = SettingsStore.CurrentCustomTheme;
                this.BackColor = theme.BackColor;
                foreach (Control c in this.Controls)
                {
                    if (c is Label)
                        c.ForeColor = theme.LabelColor;
                    else if (c is Button)
                    {
                        c.BackColor = theme.ButtonColor;
                        c.ForeColor = theme.ForeColor;
                    }
                    else if (c is ComboBox)
                    {
                        c.BackColor = theme.PanelColor;
                        c.ForeColor = theme.ForeColor;
                    }
                    else if (c is CheckBox)
                        c.ForeColor = theme.ForeColor;
                    if (theme.MainFont != null) c.Font = theme.MainFont;
                }
            }
            else if (SettingsStore.DarkMode)
            {
                this.BackColor = Color.FromArgb(32, 32, 32);
                foreach (Control c in this.Controls)
                {
                    if (c is Label)
                        c.ForeColor = Color.White;
                    else if (c is Button)
                    {
                        c.BackColor = Color.FromArgb(60, 60, 60);
                        c.ForeColor = Color.White;
                    }
                    else if (c is ComboBox)
                    {
                        c.BackColor = Color.FromArgb(45, 45, 45);
                        c.ForeColor = Color.White;
                    }
                    else if (c is CheckBox)
                        c.ForeColor = Color.White;
                }
            }
            else
            {
                this.BackColor = SystemColors.Control;
                foreach (Control c in this.Controls)
                {
                    if (c is Label)
                        c.ForeColor = Color.Black;
                    else if (c is Button)
                    {
                        c.BackColor = SystemColors.Control;
                        c.ForeColor = Color.Black;
                    }
                    else if (c is ComboBox)
                    {
                        c.BackColor = Color.White;
                        c.ForeColor = Color.Black;
                    }
                    else if (c is CheckBox)
                        c.ForeColor = Color.Black;
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            // Save settings to SettingsStore
            bool previousDarkMode = SettingsStore.DarkMode;
            SettingsStore.DarkMode = checkBoxDarkMode.Checked;
            SettingsStore.Language = comboBoxLanguage.SelectedItem?.ToString() ?? "English";
            SettingsStore.VersionsFolder = textBoxFolder.Text;
            SettingsStore.EnableDiscordRpc = checkBoxDiscordRpc.Checked;
            SettingsStore.ServerDomain = comboBoxDomain.SelectedItem?.ToString() ?? "github.com";
            // Only invert icons if DarkMode changed
            if (SettingsStore.DarkMode != previousDarkMode)
            {
                var home = Application.OpenForms["Home"] as Home;
                home?.ApplySettings();
            }
            // Show confirmation with dark theme
            DarkMessageBox.Show("Settings saved!", "Settings", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {

        }
    }
}
