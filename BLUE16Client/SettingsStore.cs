public static class SettingsStore
{
    public static bool DarkMode = false;
    public static string Language = "English";
    public static string? VersionsFolder { get; set; } = Path.Combine(Application.StartupPath, "Versions");
}
