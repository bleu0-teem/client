using System;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Diagnostics;

namespace BLUE16Client
{
    public partial class Home : Form
    {
        private System.Windows.Forms.Timer internetStatusTimer;
        private VersionList.VersionInfo? selectedVersionInfo;
        private CustomClientInfo? selectedCustomClient;
        private string versionsFolder => SettingsStore.VersionsFolder ?? Path.Combine(Application.StartupPath, "Versions");

        private NotifyIcon? trayIcon;
        private ContextMenuStrip? trayMenu;

        // Performance monitoring
        private System.Windows.Forms.Timer? perfTimer;
        private Panel? perfPanel;
        private Label? perfCpuTotalLabel;
        private Label? perfCpuAppLabel;
        private Label? perfMemLabel;
        private PerformanceCounter? cpuTotalCounter;
        private DateTime lastCpuSampleTime = DateTime.UtcNow;
        private TimeSpan lastProcCpu = Process.GetCurrentProcess().TotalProcessorTime;
        private PerformanceMonitorForm? perfWindow;

        public Home()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
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

            // Initialize Custom Client Manager
            CustomClientManager.Initialize();

            // Initialize Plugin Manager
            PluginManager.Initialize();

            // Initialize Client Mod Manager
            ClientModManager.Initialize();

            // Initialize tray icon
            InitializeTray();

            // Accessibility names and descriptions
            startgame.AccessibleName = "Play";
            startgame.AccessibleDescription = "Launch the selected version or custom client";
            panel3.AccessibleName = "Version panel";
            panel3.AccessibleDescription = "Displays the selected version. Right-click for options";
            label7.AccessibleName = "Selected version label";
            label7.AccessibleDescription = "Shows the current selected version";
            panel2.AccessibleName = "Server panel";
            panel2.AccessibleDescription = "Displays the selected server. Right-click for options";
            label6.AccessibleName = "Selected server label";
            label6.AccessibleDescription = "Shows the current selected server";

            // Tooltips for common actions
            var tooltip = new ToolTip();
            tooltip.SetToolTip(startgame, "Launch the selected version or custom client");
            tooltip.SetToolTip(panel3, "Click to select a version. Right-click for more options");
            tooltip.SetToolTip(label7, "Click to select a version. Right-click for more options");
            tooltip.SetToolTip(panel2, "Click to select a server. Right-click for more options");
            tooltip.SetToolTip(label6, "Click to select a server. Right-click for more options");

            // Context menu for Version panel
            var versionCtx = new ContextMenuStrip();
            var ctxSelectVersion = new ToolStripMenuItem("Select Version...");
            var ctxSelectCustom = new ToolStripMenuItem("Select Custom Client...");
            var ctxClearVersion = new ToolStripMenuItem("Clear Selection");
            versionCtx.Items.AddRange(new ToolStripItem[] { ctxSelectVersion, ctxSelectCustom, new ToolStripSeparator(), ctxClearVersion });
            panel3.ContextMenuStrip = versionCtx;
            label7.ContextMenuStrip = versionCtx;
            ctxSelectVersion.Click += (s, e) => VersionSelect_Click(s!, e);
            ctxSelectCustom.Click += (s, e) => CustomClientSelect_Click(s!, e);
            ctxClearVersion.Click += (s, e) => { selectedVersionInfo = null; selectedCustomClient = null; UpdateVersionDisplay(); };

            // Context menu for Server panel
            var serverCtx = new ContextMenuStrip();
            var ctxSelectServer = new ToolStripMenuItem("Select Server...");
            var ctxClearServer = new ToolStripMenuItem("Clear Server");
            serverCtx.Items.AddRange(new ToolStripItem[] { ctxSelectServer, new ToolStripSeparator(), ctxClearServer });
            panel2.ContextMenuStrip = serverCtx;
            label6.ContextMenuStrip = serverCtx;
            ctxSelectServer.Click += (s, e) => ServerSelect_Click(s!, e);
            ctxClearServer.Click += (s, e) => { label6.Text = "Click to select"; };

            // Initialize Performance Monitoring UI (bottom-right compact panel)
            try
            {
                perfPanel = new Panel
                {
                    Width = 240,
                    Height = 64,
                    BackColor = Color.FromArgb(245, 245, 245),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                };
                // Position near bottom-right corner with 8px margin
                perfPanel.Left = this.ClientSize.Width - perfPanel.Width - 8;
                perfPanel.Top = this.ClientSize.Height - perfPanel.Height - 8;

                var title = new Label
                {
                    Text = "Performance",
                    Font = new Font(Font, FontStyle.Bold),
                    AutoSize = true,
                    Left = 8,
                    Top = 6
                };
                perfCpuTotalLabel = new Label { Text = "CPU Total: -- %", AutoSize = true, Left = 8, Top = 24 };
                perfCpuAppLabel = new Label { Text = "CPU App: -- %", AutoSize = true, Left = 8, Top = 40 };
                perfMemLabel = new Label { Text = "Memory: --", AutoSize = true, Left = 120, Top = 24 };

                perfPanel.Controls.Add(title);
                perfPanel.Controls.Add(perfCpuTotalLabel);
                perfPanel.Controls.Add(perfCpuAppLabel);
                perfPanel.Controls.Add(perfMemLabel);

                // Adjust colors for dark mode after settings applied
                this.Controls.Add(perfPanel);
                perfPanel.Cursor = Cursors.Hand;
                perfPanel.DoubleClick += (s, e) => ShowPerformanceWindow();
                this.Resize += (s, e) =>
                {
                    if (perfPanel != null)
                    {
                        perfPanel.Left = this.ClientSize.Width - perfPanel.Width - 8;
                        perfPanel.Top = this.ClientSize.Height - perfPanel.Height - 8;
                    }
                };

                // Initialize counters and timer
                cpuTotalCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
                _ = cpuTotalCounter.NextValue(); // prime, first read is 0

                perfTimer = new System.Windows.Forms.Timer { Interval = 1000 };
                perfTimer.Tick += PerfTimer_Tick;
                perfTimer.Start();
            }
            catch
            {
                // Silently ignore if performance counters are unavailable
            }
        }

        private void PerfTimer_Tick(object? sender, EventArgs e)
        {
            try
            {
                var now = DateTime.UtcNow;
                var proc = System.Diagnostics.Process.GetCurrentProcess();

                // App CPU usage based on TotalProcessorTime delta
                var newProcCpu = proc.TotalProcessorTime;
                var cpuMs = (newProcCpu - lastProcCpu).TotalMilliseconds;
                var wallMs = (now - lastCpuSampleTime).TotalMilliseconds;
                var cpuApp = wallMs > 0 ? Math.Min(100.0, Math.Max(0.0, (cpuMs / wallMs) * 100.0 / Environment.ProcessorCount)) : 0.0;
                lastProcCpu = newProcCpu;
                lastCpuSampleTime = now;

                // System total CPU
                float cpuTotal = 0f;
                if (cpuTotalCounter != null)
                {
                    cpuTotal = cpuTotalCounter.NextValue();
                }

                // Memory usage
                long working = proc.WorkingSet64;
                long managed = GC.GetTotalMemory(false);

                if (perfCpuAppLabel != null)
                    perfCpuAppLabel.Text = $"CPU App: {cpuApp:0.0} %";
                if (perfCpuTotalLabel != null)
                    perfCpuTotalLabel.Text = $"CPU Total: {cpuTotal:0.0} %";
                if (perfMemLabel != null)
                    perfMemLabel.Text = $"Memory: {FormatBytes(working)}\nManaged: {FormatBytes(managed)}";
            }
            catch
            {
                // Ignore sampling errors
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024.0;
            }
            return $"{len:0.##} {sizes[order]}";
        }

        private void ShowPerformanceWindow()
        {
            try
            {
                if (perfWindow == null || perfWindow.IsDisposed)
                {
                    perfWindow = new PerformanceMonitorForm();
                }
                if (!perfWindow.Visible)
                {
                    perfWindow.Show(this);
                }
                perfWindow.BringToFront();
                perfWindow.Activate();
            }
            catch
            {
                // ignore
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            // Cleanup performance monitoring resources
            try
            {
                if (perfTimer != null)
                {
                    perfTimer.Stop();
                    perfTimer.Tick -= PerfTimer_Tick;
                }
                cpuTotalCounter?.Dispose();
                if (perfWindow != null && !perfWindow.IsDisposed)
                {
                    perfWindow.Close();
                    perfWindow.Dispose();
                }
            }
            catch { }
            DiscordRpcManager.Instance.Shutdown();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            // Apply settings immediately on startup (theme/language) and reflect login if credentials loaded
            ApplySettings();
            if (SettingsStore.IsLoggedIn)
            {
                label3.Text = $"Logged in as {SettingsStore.Username}";
            }
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

        private void InitializeTray()
        {
            trayMenu = new ContextMenuStrip();
            var showItem = new ToolStripMenuItem("Show");
            var perfItem = new ToolStripMenuItem("Performance Monitor...");
            var playItem = new ToolStripMenuItem("Play");
            var exitItem = new ToolStripMenuItem("Exit");
            showItem.Click += (s, e) => RestoreFromTray();
            perfItem.Click += (s, e) => ShowPerformanceWindow();
            playItem.Click += (s, e) => Startgame_Click(this, EventArgs.Empty);
            exitItem.Click += (s, e) => { trayIcon!.Visible = false; Close(); };
            trayMenu.Items.Add(showItem);
            trayMenu.Items.Add(perfItem);
            trayMenu.Items.Add(playItem);
            trayMenu.Items.Add(new ToolStripSeparator());
            trayMenu.Items.Add(exitItem);

            trayIcon = new NotifyIcon
            {
                Visible = false,
                Text = "BLUE16Client",
                ContextMenuStrip = trayMenu,
                Icon = SystemIcons.Application
            };
            trayIcon.DoubleClick += (s, e) => RestoreFromTray();
        }

        private void MinimizeToTray()
        {
            if (trayIcon == null) return;
            trayIcon.Visible = true;
            this.Hide();
            trayIcon.BalloonTipTitle = "BLUE16Client";
            trayIcon.BalloonTipText = "Running in the tray";
            trayIcon.ShowBalloonTip(2000);
        }

        private void RestoreFromTray()
        {
            if (trayIcon == null) return;
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
            trayIcon.Visible = false;
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
                // Adjust perf panel colors for custom theme
                if (perfPanel != null)
                {
                    perfPanel.BackColor = theme.PanelColor;
                    foreach (Control cc in perfPanel.Controls)
                    {
                        cc.ForeColor = theme.LabelColor;
                    }
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
                if (perfPanel != null)
                {
                    perfPanel.BackColor = Color.FromArgb(45, 45, 45);
                    foreach (Control cc in perfPanel.Controls)
                    {
                        cc.ForeColor = Color.White;
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
                if (perfPanel != null)
                {
                    perfPanel.BackColor = Color.White;
                    foreach (Control cc in perfPanel.Controls)
                    {
                        cc.ForeColor = Color.Black;
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
            if (!SettingsStore.IsLoggedIn)
            {
                using (var loginForm = new LoginForm())
                {
                    if (loginForm.ShowDialog() == DialogResult.OK)
                    {
                        // Update Discord RPC to show logged-in status
                        if (selectedVersionInfo != null)
                        {
                            DiscordRpcManager.Instance.UpdateForVersion(selectedVersionInfo);
                        }
                        // Save credentials already handled in LoginForm; reflect UI state
                        label3.Text = $"Logged in as {SettingsStore.Username}!";
                        DarkMessageBox.Show($"Logged in as {SettingsStore.Username}!", "Login Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            else
            {
                var result = DarkMessageBox.Show($"Logged in as {SettingsStore.Username}\nDo you want to log out?", 
                    "Profile", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                
                if (result == DialogResult.Yes)
                {
                    SettingsStore.ClearCredentialsSecure();
                    // Update Discord RPC to remove login status
                    if (selectedVersionInfo != null)
                    {
                        DiscordRpcManager.Instance.UpdateForVersion(selectedVersionInfo);
                    }
                    label3.Text = string.Empty;
                    DarkMessageBox.Show("Logged out successfully.", "Logout", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void Home_Load(object sender, EventArgs e)
        {
            this.Resize += (s, ev) =>
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    MinimizeToTray();
                }
            };
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
                if (versionList.ShowDialog() == DialogResult.OK)
                {
                    if (versionList.SelectedCustomClient != null)
                    {
                        selectedCustomClient = versionList.SelectedCustomClient;
                        selectedVersionInfo = null;
                    }
                    else
                    {
                        selectedVersionInfo = versionList.SelectedVersionInfo;
                        selectedCustomClient = null;
                    }
                    UpdateVersionDisplay();
                }
            }
        }

        private void CustomClientSelect_Click(object sender, EventArgs e)
        {
            using (var clientSelector = new CustomClientSelector())
            {
                if (clientSelector.ShowDialog() == DialogResult.OK)
                {
                    selectedCustomClient = clientSelector.SelectedClient;
                    selectedVersionInfo = null; // Clear version selection
                    UpdateVersionDisplay();
                    // Update Discord RPC for custom client
                    if (selectedCustomClient != null)
                        DiscordRpcManager.Instance.UpdateForCustomClient(selectedCustomClient);
                }
            }
        }

        private void UpdateVersionDisplay()
        {
            if (selectedCustomClient != null)
            {
                label7.Text = selectedCustomClient.Name;
                label4.Text = "Custom Client Information";
                
                string status = selectedCustomClient.IsSupported ? "Supported" : "Unsupported";
                string details = $"RCCS: {Path.GetFileName(selectedCustomClient.RccsPath)}\n" +
                               $"Client: {Path.GetFileName(selectedCustomClient.ClientPath)}\n" +
                               $"Status: {status}";
                
                if (!selectedCustomClient.IsSupported && !string.IsNullOrEmpty(selectedCustomClient.ErrorMessage))
                {
                    details += $"\nError: {selectedCustomClient.ErrorMessage}";
                }
                
                if (selectedCustomClient.AutoLaunchServer && !string.IsNullOrEmpty(selectedCustomClient.ServerPath))
                {
                    details += $"\nAuto-launch server: {Path.GetFileName(selectedCustomClient.ServerPath)}";
                }
                
                // Update Discord RPC
                DiscordRpcManager.Instance.UpdateForCustomClient(selectedCustomClient);
            }
            else if (selectedVersionInfo != null)
            {
                label7.Text = selectedVersionInfo.Name;
                label4.Text = "Version Information";
                
                string details = $"Status: {selectedVersionInfo.Status}\n" +
                               $"Alt Name: {selectedVersionInfo.AltName}\n" +
                               $"Patched by: {selectedVersionInfo.PatchBy}\n" +
                               $"Offline: {selectedVersionInfo.Offline}\n" +
                               $"Description: {selectedVersionInfo.Desc}";
                
                // Update Discord RPC with version info
                DiscordRpcManager.Instance.UpdateForVersion(selectedVersionInfo);
            }
            else
            {
                label7.Text = "Click to select";
                label4.Text = "Version Information";
                // Reset Discord RPC
                DiscordRpcManager.Instance.UpdatePresence("In Menu", "Idle");
            }
        }

        private async void Startgame_Click(object sender, EventArgs e)
        {
            if (selectedVersionInfo == null && selectedCustomClient == null)
            {
                MessageBox.Show("Please select a version or custom client first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Handle custom client launch
            if (selectedCustomClient != null)
            {
                if (!selectedCustomClient.IsSupported)
                {
                    MessageBox.Show($"Cannot launch unsupported client: {selectedCustomClient.ErrorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string? serverAddress = label6.Text != "Click to select" ? label6.Text : null;
                
                label2.Text = "STATUS: Launching custom client...";
                label2.ForeColor = Color.Orange;

                // Plugin hook: before launch
                var launchCtxCustom = new LaunchContext
                {
                    Version = null,
                    CustomClient = selectedCustomClient,
                    Server = serverAddress,
                    WorkingDirectory = Path.GetDirectoryName(selectedCustomClient.ClientPath),
                    ExecutablePath = selectedCustomClient.ClientPath,
                    Arguments = selectedCustomClient.LaunchArguments
                };
                PluginManager.BeforeLaunch(launchCtxCustom);

                // Mods hook: pre-launch
                var modCtxCustom = new ModContext
                {
                    Version = null,
                    CustomClient = selectedCustomClient,
                    Server = serverAddress,
                    WorkingDirectory = Path.GetDirectoryName(selectedCustomClient.ClientPath),
                    ExecutablePath = selectedCustomClient.ClientPath,
                    Arguments = selectedCustomClient.LaunchArguments
                };
                ClientModManager.OnPreLaunch(modCtxCustom);

                bool success = await CustomClientManager.LaunchCustomClientAsync(selectedCustomClient, serverAddress);
                
                if (success)
                {
                    label2.Text = "STATUS: Custom client launched";
                    label2.ForeColor = Color.Green;
                    PluginManager.AfterLaunch(launchCtxCustom, true, null);
                    ClientModManager.OnPostLaunch(modCtxCustom, true, null);
                }
                else
                {
                    label2.Text = "STATUS: Launch failed";
                    label2.ForeColor = Color.Red;
                    var err = new Exception("Custom client launch failed");
                    PluginManager.AfterLaunch(launchCtxCustom, false, err);
                    ClientModManager.OnPostLaunch(modCtxCustom, false, err);
                }
                return;
            }

            // Handle regular version launch
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
                // Plugin hook: before launch
                var launchCtx = new LaunchContext
                {
                    Version = selectedVersionInfo,
                    CustomClient = null,
                    Server = label6.Text != "Click to select" ? label6.Text : null,
                    WorkingDirectory = versionFolder,
                    ExecutablePath = exePath,
                    Arguments = startInfo.Arguments
                };
                PluginManager.BeforeLaunch(launchCtx);

                // Mods hook: pre-launch
                var modCtx = new ModContext
                {
                    Version = selectedVersionInfo,
                    CustomClient = null,
                    Server = label6.Text != "Click to select" ? label6.Text : null,
                    WorkingDirectory = versionFolder,
                    ExecutablePath = exePath,
                    Arguments = startInfo.Arguments
                };
                ClientModManager.OnPreLaunch(modCtx);

                System.Diagnostics.Process.Start(startInfo);
                PluginManager.AfterLaunch(launchCtx, true, null);
                ClientModManager.OnPostLaunch(modCtx, true, null);
            }
            catch (Exception ex)
            {
                var launchCtx = new LaunchContext
                {
                    Version = selectedVersionInfo,
                    CustomClient = null,
                    Server = label6.Text != "Click to select" ? label6.Text : null,
                    WorkingDirectory = versionFolder,
                    ExecutablePath = exePath,
                    Arguments = selectedVersionInfo.LaunchArguments
                };
                PluginManager.AfterLaunch(launchCtx, false, ex);
                var modCtx = new ModContext
                {
                    Version = selectedVersionInfo,
                    CustomClient = null,
                    Server = label6.Text != "Click to select" ? label6.Text : null,
                    WorkingDirectory = versionFolder,
                    ExecutablePath = exePath,
                    Arguments = selectedVersionInfo.LaunchArguments
                };
                ClientModManager.OnPostLaunch(modCtx, false, ex);
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
