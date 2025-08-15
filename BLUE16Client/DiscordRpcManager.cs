using DiscordRPC;
using System;

namespace BLUE16Client
{
    public class DiscordRpcManager
    {
        private static DiscordRpcManager? _instance;
        private readonly DiscordRpcClient? _client;
        private const string CLIENT_ID = "1403342242831405086";

        private DiscordRpcManager()
        {
            if (!SettingsStore.EnableDiscordRpc)
                return;
            try
            {
                _client = new DiscordRpcClient(CLIENT_ID);
                _client.Initialize();

                // Set default presence
                UpdatePresence("In Menu", "Idle");
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Failed to initialize Discord RPC: {ex.Message}", 
                    "Discord RPC Error", System.Windows.Forms.MessageBoxButtons.OK, 
                    System.Windows.Forms.MessageBoxIcon.Warning);
            }
        }

        public static DiscordRpcManager Instance
        {
            get
            {
                _instance ??= new DiscordRpcManager();
                return _instance;
            }
        }

        public void UpdatePresence(string details, string state, string? largeImageKey = "blue16_logo", 
            string? largeImageText = "BLUE16Client", string? smallImageKey = null, string? smallImageText = null)
        {
            if (!SettingsStore.EnableDiscordRpc)
                return;
            try
            {
                if (_client?.IsInitialized != true)
                    return;

                _client.SetPresence(new RichPresence
                {
                    Details = details,
                    State = state,
                    Assets = new Assets
                    {
                        LargeImageKey = largeImageKey,
                        LargeImageText = largeImageText,
                        SmallImageKey = smallImageKey,
                        SmallImageText = smallImageText
                    },
                    Timestamps = Timestamps.Now
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to update Discord presence: {ex.Message}");
            }
        }

        public void UpdateForVersion(VersionList.VersionInfo version)
        {
            string state = version.Offline ? "Playing Offline" : "Playing Online";
            UpdatePresence($"Playing {version.Name}", state);
        }

        public void UpdateForServer(VersionList.VersionInfo version, string serverName)
        {
            UpdatePresence($"Playing {version.Name}", $"Server: {serverName}");
        }

        public void Shutdown()
        {
            if (!SettingsStore.EnableDiscordRpc)
                return;
            try
            {
                if (_client?.IsInitialized == true)
                {
                    _client.Dispose();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to shutdown Discord RPC: {ex.Message}");
            }
        }
    }
}