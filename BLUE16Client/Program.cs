namespace BLUE16Client
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            // Attempt to load saved credentials securely
            SettingsStore.TryLoadCredentialsSecure();
            Application.Run(new Home());
        }
    }
}