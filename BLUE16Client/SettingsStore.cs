public static class SettingsStore
{
    public static bool DarkMode = false;
    public static string Language = "English";
    public static string? VersionsFolder { get; set; } = Path.Combine(Application.StartupPath, "Versions");
    public static bool EnableDiscordRpc = true;
    public static string ServerDomain { get; set; } = "github.com";

    public static CustomTheme? CurrentCustomTheme { get; set; }

    public static string GetResourceUrl(string path)
    {
        if (ServerDomain == "github.com")
            return $"https://raw.githubusercontent.com/blue16-team/blue16-web/refs/heads/main/www/{path.TrimStart('/')}";
        var domain = ServerDomain.TrimEnd('/');
        if (!domain.StartsWith("http")) domain = "https://" + domain;
        return $"{domain}/{path.TrimStart('/')}";
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
}
