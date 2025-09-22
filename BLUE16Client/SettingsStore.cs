using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Drawing;

public static class SettingsStore
{
    public static bool DarkMode = false;
    public static string Language = "English";
    public static string? VersionsFolder { get; set; } = Path.Combine(Application.StartupPath, "Versions");
    public static bool EnableDiscordRpc = true;
    public static string ServerDomain { get; set; } = "github.com";
    public static string? AuthToken { get; set; }
    public static string? Username { get; set; }
    public static CustomTheme? CurrentCustomTheme { get; set; }

    public static bool IsLoggedIn => !string.IsNullOrEmpty(AuthToken);

    public static string GetResourceUrl(string path)
    {
        if (ServerDomain == "github.com")
            return $"https://raw.githubusercontent.com/bleu0-teem/web/refs/heads/main/www/{path.TrimStart('/')}";
        var domain = ServerDomain.TrimEnd('/');
        if (!domain.StartsWith("http")) domain = "https://" + domain;
        return $"{domain}/{path.TrimStart('/')}";
    }

    private static string GetCredentialsFilePath()
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string folder = Path.Combine(appData, "BLUE16Client");
        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
        return Path.Combine(folder, "credentials.bin");
    }

    public static void SaveCredentialsSecure()
    {
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(AuthToken))
            return;
        var payload = new CredentialPayload { Username = Username!, AuthToken = AuthToken! };
        string json = JsonSerializer.Serialize(payload);
        byte[] plain = Encoding.UTF8.GetBytes(json);
        byte[] protectedBytes = ProtectedData.Protect(plain, null, DataProtectionScope.CurrentUser);
        File.WriteAllBytes(GetCredentialsFilePath(), protectedBytes);
    }

    public static bool TryLoadCredentialsSecure()
    {
        try
        {
            string path = GetCredentialsFilePath();
            if (!File.Exists(path)) return false;
            byte[] protectedBytes = File.ReadAllBytes(path);
            byte[] plain = ProtectedData.Unprotect(protectedBytes, null, DataProtectionScope.CurrentUser);
            string json = Encoding.UTF8.GetString(plain);
            var payload = JsonSerializer.Deserialize<CredentialPayload>(json);
            if (payload == null || string.IsNullOrWhiteSpace(payload.AuthToken)) return false;
            Username = payload.Username;
            AuthToken = payload.AuthToken;
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static void ClearCredentialsSecure()
    {
        try
        {
            Username = null;
            AuthToken = null;
            string path = GetCredentialsFilePath();
            if (File.Exists(path)) File.Delete(path);
        }
        catch { }
    }

    // Custom theme structure
    public class CustomTheme
    {
        public string Name { get; set; } = "Custom";
        public Color BackColor { get; set; } = Color.White;
        public Color ForeColor { get; set; } = Color.Black;
        public Color PanelColor { get; set; } = Color.White;
        public Color ButtonColor { get; set; } = Color.LightGray;
        public Color LabelColor { get; set; } = Color.Black;
        public Font? MainFont { get; set; }
    }

    private class CredentialPayload
    {
        public string Username { get; set; } = string.Empty;
        public string AuthToken { get; set; } = string.Empty;
    }
}
