using System;
using System.Windows.Forms;
using System.Drawing;

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
            this.ClientSize = new Size(400, 220); // Make the settings popup bigger
            // Load current settings
            checkBoxDarkMode.Checked = SettingsStore.DarkMode;
            comboBoxLanguage.SelectedItem = SettingsStore.Language;
            textBoxFolder = new TextBox { Width = 220, Text = SettingsStore.VersionsFolder ?? string.Empty, Location = new Point(20, 120) };
            buttonBrowse = new Button { Text = "Browse...", Location = new Point(250, 120), Width = 80 };
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
            this.Controls.Add(new Label { Text = "Versions Folder:", Location = new Point(20, 100), Width = 120 });
            this.Controls.Add(textBoxFolder);
            this.Controls.Add(buttonBrowse);
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (SettingsStore.DarkMode)
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
            SettingsStore.DarkMode = checkBoxDarkMode.Checked;
            SettingsStore.Language = comboBoxLanguage.SelectedItem?.ToString() ?? "English";
            SettingsStore.VersionsFolder = textBoxFolder.Text;
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
