using System;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Compression;
using System.Net.Http;

namespace BLUE16Client
{
    public partial class Home : Form
    {
        private System.Windows.Forms.Timer internetStatusTimer;
        private VersionList.VersionInfo? selectedVersionInfo;
        private string versionsFolder => SettingsStore.VersionsFolder ?? Path.Combine(Application.StartupPath, "Versions");

        public Home()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.Sizable;
            this.MaximizeBox = false;
            internetStatusTimer = new System.Windows.Forms.Timer();
            internetStatusTimer.Interval = 5000;
            internetStatusTimer.Tick += InternetStatusTimer_Tick;
            internetStatusTimer.Start();
            UpdateInternetStatus();
            label4.Text = "Version Information";
            label7.Text = "Select a version";
            label2.Text = "STATUS:";
            label2.ForeColor = Color.Black;
            label3.Text = "";
            label6.Text = "Click to select";
            label1.Text = "Version:";
            label5.Text = "Server:";
            label7.Text = "Click to select";
            startgame.Click += Startgame_Click;
            label6.Click += ServerSelect_Click;
            panel2.Click += ServerSelect_Click;
            label7.Click += VersionSelect_Click;
            panel3.Click += VersionSelect_Click;
            iconbutton.Click += IconButton_Click;
            playbutton.Click += PlayButton_Click;
            profilebutton.Click += ProfileButton_Click;

            // Initialize Discord RPC
            _ = DiscordRpcManager.Instance;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            DiscordRpcManager.Instance.Shutdown();
        }

        private void InternetStatusTimer_Tick(object sender, EventArgs e)
        {
            UpdateInternetStatus();
        }

        private void UpdateInternetStatus()
        {
            if (IsInternetAvailable())
            {
                label2.Text = "STATUS: Connected";
                label2.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                label2.Text = "STATUS: Disconnected, using Github repo";
                label2.ForeColor = System.Drawing.Color.Red;
            }
        }

        private bool IsInternetAvailable()
        {
            try
            {
                using (var ping = new Ping())
                {
                    var reply = ping.Send("https://www.blue16.site", 1000);
                    return reply.Status == IPStatus.Success;
                }
            }
            catch
            {
                return false;
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            if (installProgressBar == null)
                return;
            installProgressBar.Value = 0;
            installProgressBar.Visible = true;
            installProgressBar.BringToFront();
            for (int i = 0; i <= 100; i += 5)
            {
                installProgressBar.Value = i;
                await System.Threading.Tasks.Task.Delay(50);
            }
            await System.Threading.Tasks.Task.Delay(700);
            installProgressBar.Visible = false;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void settingsbutton_Click(object sender, EventArgs e)
        {
            using (var settings = new SettingsForm())
            {
                if (settings.ShowDialog() == DialogResult.OK)
                {
                    ApplySettings();
                }
            }
        }

        public void ApplySettings()
        {
            if (SettingsStore.CurrentCustomTheme != null)
            {
                var theme = SettingsStore.CurrentCustomTheme;
                this.BackColor = theme.BackColor;
                foreach (Control c in this.Controls)
                {
                    if (c is Panel)
                    {
                        c.BackColor = theme.PanelColor;
                        c.ForeColor = theme.ForeColor;
                    }
                    else if (c is Label)
                    {
                        c.ForeColor = theme.LabelColor;
                    }
                    else if (c is Button button)
                    {
                        button.BackColor = theme.ButtonColor;
                        button.ForeColor = theme.ForeColor;
                        if (theme.MainFont != null) button.Font = theme.MainFont;
                    }
                    if (theme.MainFont != null) c.Font = theme.MainFont;
                }
            }
            else if (SettingsStore.DarkMode)
            {
                this.BackColor = Color.FromArgb(32, 32, 32);
                foreach (Control c in this.Controls)
                {
                    if (c is Panel)
                    {
                        c.BackColor = Color.FromArgb(45, 45, 45);
                        c.ForeColor = Color.White;
                    }
                    else if (c is Label)
                    {
                        c.ForeColor = Color.White;
                    }
                    else if (c is Button button)
                    {
                        button.BackColor = Color.FromArgb(60, 60, 60);
                        button.ForeColor = Color.White;
                        if (button == iconbutton || button == playbutton || button == profilebutton || button == settingsbutton)
                        {
                            button.BackColor = Color.FromArgb(45, 45, 45);
                            button.ForeColor = Color.White;
                            // Invert icon colors
                            button.Image = InvertImage((Bitmap)button.Image);
                        }
                    }
                }
            }
            else
            {
                this.BackColor = Color.Gainsboro;
                foreach (Control c in this.Controls)
                {
                    if (c is Panel)
                    {
                        c.BackColor = Color.White;
                        c.ForeColor = Color.Black;
                    }
                    else if (c is Label)
                    {
                        c.ForeColor = Color.Black;
                    }
                    else if (c is Button button)
                    {
                        button.BackColor = SystemColors.Control;
                        button.ForeColor = Color.Black;
                        if (button == iconbutton || button == playbutton || button == profilebutton || button == settingsbutton)
                        {
                            button.BackColor = Color.White;
                            button.ForeColor = Color.Black;
                            // Revert icon colors
                            button.Image = InvertImage((Bitmap)button.Image);
                        }
                    }
                }
            }

            // Apply language setting
            switch (SettingsStore.Language)
            {
                case "English":
                    label1.Text = "Version:";
                    label4.Text = "Changelog and everything you need to know:";
                    label5.Text = "Server:";
                    startgame.Text = "PLAY";
                    break;
                case "Polski":
                    label1.Text = "Wersja:";
                    label4.Text = "Lista zmian i wszystko, co musisz wiedzieć:";
                    label5.Text = "Serwer:";
                    startgame.Text = "GRAJ";
                    break;
                case "Deutsch":
                    label1.Text = "Version:";
                    label4.Text = "?nderungsprotokoll und alles, was Sie wissen m?ssen:";
                    label5.Text = "Server:";
                    startgame.Text = "SPIELEN";
                    break;
            }
        }

        private Bitmap InvertImage(Bitmap original)
        {
            Bitmap inverted = new Bitmap(original.Width, original.Height);
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixel = original.GetPixel(x, y);
                    Color invertedPixel = Color.FromArgb(pixel.A, 255 - pixel.R, 255 - pixel.G, 255 - pixel.B);
                    inverted.SetPixel(x, y, invertedPixel);
                }
            }
            return inverted;
        }

        private void IconButton_Click(object sender, EventArgs e)
        {
            // Show About/Info dialog or return to main menu
            DarkMessageBox.Show("BLUE16Client\nVersion: 1.0\nDeveloped by: BLUE16", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            // Placeholder for future functionality
            DarkMessageBox.Show("Play button functionality is not implemented.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ProfileButton_Click(object sender, EventArgs e)
        {
            // Show profile settings or login dialog
            DarkMessageBox.Show("Profile functionality coming soon!", "Profile", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Home_Load(object sender, EventArgs e)
        {

        }

        private void ServerSelect_Click(object sender, EventArgs e)
        {
            using (var serverList = new ServerList())
            {
                if (serverList.ShowDialog() == DialogResult.OK && serverList.SelectedServer != null)
                {
                    label6.Text = serverList.SelectedServer;
                    label2.Text = $"Server: {serverList.SelectedServer}";
                    label2.ForeColor = System.Drawing.Color.Blue;
                    // Update Discord RPC with version and server info
                    if (selectedVersionInfo != null)
                        DiscordRpcManager.Instance.UpdateForServer(selectedVersionInfo, serverList.SelectedServer);
                }
            }
        }

        private void VersionSelect_Click(object sender, EventArgs e)
        {
            using (var versionList = new VersionList())
            {
                if (versionList.ShowDialog() == DialogResult.OK && versionList.SelectedVersionInfo != null)
                {
                    selectedVersionInfo = versionList.SelectedVersionInfo;
                    label7.Text = selectedVersionInfo.Name;
                    // Update panel1 with version info
                    label3.Text = $"Version: {selectedVersionInfo.Name} / {selectedVersionInfo.AltName}\n" +
                                  $"Download Link: {selectedVersionInfo.DownloadUrl}\n" +
                                  $"Vulnerable: {selectedVersionInfo.Vulnerable}\n" +
                                  $"Patched by: {selectedVersionInfo.PatchBy}\n" +
                                  $"Offline: {selectedVersionInfo.Offline}\n" +
                                  $"Description: {selectedVersionInfo.Desc}";

                    // Update Discord RPC with version info
                    DiscordRpcManager.Instance.UpdateForVersion(selectedVersionInfo);
                }
            }
        }

        private async void Startgame_Click(object sender, EventArgs e)
        {
            if (selectedVersionInfo == null)
            {
                MessageBox.Show("Please select a version first.", "No Version Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (selectedVersionInfo.Offline)
            {
                MessageBox.Show("Warning: Online features are not available for this version.", "Offline Version", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            if (selectedVersionInfo.Vulnerable)
            {
                MessageBox.Show("Warning: This version is marked as vulnerable!", "Vulnerable Version", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            string versionFolder = Path.Combine(versionsFolder, selectedVersionInfo.Name);
            string exePath = Path.Combine(versionFolder, selectedVersionInfo.ExecutableFile ?? "");
            if (!Directory.Exists(versionFolder) || !File.Exists(exePath))
            {
                if (string.IsNullOrEmpty(selectedVersionInfo.DownloadUrl))
                {
                    MessageBox.Show("No download URL available for this version.", "Download Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                MessageBox.Show($"Version not found locally. Downloading from {selectedVersionInfo.DownloadUrl}", "Download", MessageBoxButtons.OK, MessageBoxIcon.Information);
                try
                {
                    using var client = new HttpClient();
                    var zipBytes = await client.GetByteArrayAsync(selectedVersionInfo.DownloadUrl);
                    Directory.CreateDirectory(versionFolder);
                    string zipPath = Path.Combine(versionFolder, "version.zip");
                    int total = zipBytes.Length;
                    installProgressBar.Value = 0;
                    installProgressBar.Visible = true;
                    installProgressBar.BringToFront();
                    label2.Text = "STATUS: Installing...";
                    label2.ForeColor = Color.Orange;
                    int chunk = Math.Max(1, total / 100);
                    for (int i = 0; i < total; i += chunk)
                    {
                        installProgressBar.Value = Math.Min(100, (i * 100) / total);
                        label2.Text = $"STATUS: Installing... {installProgressBar.Value}%";
                        await Task.Delay(10); // Simulate progress
                    }
                    await File.WriteAllBytesAsync(zipPath, zipBytes);
                    installProgressBar.Value = 100;
                    label2.Text = "STATUS: Extracting...";
                    ZipFile.ExtractToDirectory(zipPath, versionFolder, true);
                    File.Delete(zipPath);
                    installProgressBar.Visible = false;
                    label2.Text = "STATUS: Installed";
                    label2.ForeColor = Color.Green;
                }
                catch (Exception ex)
                {
                    installProgressBar.Visible = false;
                    label2.Text = "STATUS: Install Failed";
                    label2.ForeColor = Color.Red;
                    MessageBox.Show($"Failed to download or extract version: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            if (!File.Exists(exePath))
            {
                MessageBox.Show($"Executable file not found: {exePath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo(exePath);
                if (!string.IsNullOrEmpty(selectedVersionInfo.LaunchArguments))
                {
                    startInfo.Arguments = selectedVersionInfo.LaunchArguments;
                }
                startInfo.WorkingDirectory = versionFolder;
                System.Diagnostics.Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch version: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static void SetStatusDisconnected()
        {
            var mainForm = Application.OpenForms["Home"] as Home;
            if (mainForm != null)
            {
                mainForm.label2.Text = "STATUS: Disconnected";
                mainForm.label2.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}
