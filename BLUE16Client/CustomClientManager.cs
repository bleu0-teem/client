using System;
using System.IO;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;

namespace BLUE16Client
{
    public class CustomClientInfo
    {
        public string Name { get; set; } = string.Empty;
        public string RccsPath { get; set; } = string.Empty;
        public string ClientPath { get; set; } = string.Empty;
        public string LaunchArguments { get; set; } = string.Empty;
        public bool IsSupported { get; set; } = true;
        public string? ErrorMessage { get; set; }
        public DateTime LastTested { get; set; } = DateTime.MinValue;
        public bool AutoLaunchServer { get; set; } = true;
        public string? ServerPath { get; set; }
        public string? ServerArguments { get; set; }
    }

    public static class CustomClientManager
    {
        private static readonly string CustomClientsFile = Path.Combine(Application.StartupPath, "custom_clients.json");
        private static List<CustomClientInfo> _customClients = new List<CustomClientInfo>();

        public static List<CustomClientInfo> CustomClients => _customClients;

        public static void Initialize()
        {
            LoadCustomClients();
        }

        public static void LoadCustomClients()
        {
            try
            {
                if (File.Exists(CustomClientsFile))
                {
                    var json = File.ReadAllText(CustomClientsFile);
                    _customClients = System.Text.Json.JsonSerializer.Deserialize<List<CustomClientInfo>>(json) ?? new List<CustomClientInfo>();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load custom clients: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _customClients = new List<CustomClientInfo>();
            }
        }

        public static void SaveCustomClients()
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(_customClients, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(CustomClientsFile, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save custom clients: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public static async Task<bool> TestCustomClientAsync(CustomClientInfo client)
        {
            try
            {
                // Test RCCS
                if (!File.Exists(client.RccsPath))
                {
                    client.IsSupported = false;
                    client.ErrorMessage = "RCCS file not found";
                    return false;
                }

                // Test Roblox client
                if (!File.Exists(client.ClientPath))
                {
                    client.IsSupported = false;
                    client.ErrorMessage = "Roblox client file not found";
                    return false;
                }

                // Test if files are executable
                var rccsInfo = new FileInfo(client.RccsPath);
                var clientInfo = new FileInfo(client.ClientPath);

                if (rccsInfo.Extension.ToLower() != ".exe" && clientInfo.Extension.ToLower() != ".exe")
                {
                    client.IsSupported = false;
                    client.ErrorMessage = "Both RCCS and client should be executable files (.exe)";
                    return false;
                }

                // Try to launch RCCS briefly to test if it works
                using (var process = new Process())
                {
                    process.StartInfo.FileName = client.RccsPath;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;

                    try
                    {
                        process.Start();
                        await Task.Delay(1000); // Let it run briefly
                        if (!process.HasExited)
                        {
                            process.Kill();
                        }
                    }
                    catch (Exception ex)
                    {
                        client.IsSupported = false;
                        client.ErrorMessage = $"Failed to launch RCCS: {ex.Message}";
                        return false;
                    }
                }

                client.IsSupported = true;
                client.ErrorMessage = null;
                client.LastTested = DateTime.Now;
                SaveCustomClients();
                return true;
            }
            catch (Exception ex)
            {
                client.IsSupported = false;
                client.ErrorMessage = $"Test failed: {ex.Message}";
                client.LastTested = DateTime.Now;
                SaveCustomClients();
                return false;
            }
        }

        public static async Task<bool> LaunchCustomClientAsync(CustomClientInfo client, string? serverAddress = null)
        {
            try
            {
                if (!client.IsSupported)
                {
                    MessageBox.Show($"Cannot launch unsupported client: {client.ErrorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                // Launch server if enabled
                if (client.AutoLaunchServer && !string.IsNullOrEmpty(client.ServerPath))
                {
                    if (!File.Exists(client.ServerPath))
                    {
                        MessageBox.Show($"Server file not found: {client.ServerPath}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }

                    try
                    {
                        var serverStartInfo = new ProcessStartInfo(client.ServerPath);
                        if (!string.IsNullOrEmpty(client.ServerArguments))
                        {
                            serverStartInfo.Arguments = client.ServerArguments;
                        }
                        serverStartInfo.UseShellExecute = true;
                        serverStartInfo.WorkingDirectory = Path.GetDirectoryName(client.ServerPath);
                        Process.Start(serverStartInfo);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to launch server: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }

                // Prepare launch arguments
                string arguments = client.LaunchArguments;
                if (!string.IsNullOrEmpty(serverAddress))
                {
                    arguments += $" --server {serverAddress}";
                }

                // Launch RCCS
                var startInfo = new ProcessStartInfo(client.RccsPath);
                if (!string.IsNullOrEmpty(arguments))
                {
                    startInfo.Arguments = arguments;
                }
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Path.GetDirectoryName(client.RccsPath);

                Process.Start(startInfo);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to launch custom client: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public static void AddCustomClient(CustomClientInfo client)
        {
            _customClients.Add(client);
            SaveCustomClients();
        }

        public static void RemoveCustomClient(CustomClientInfo client)
        {
            _customClients.Remove(client);
            SaveCustomClients();
        }

        public static void UpdateCustomClient(CustomClientInfo client)
        {
            SaveCustomClients();
        }

        public static CustomClientInfo? GetCustomClient(string name)
        {
            return _customClients.Find(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
