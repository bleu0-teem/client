using System;

namespace BLUE16Client
{
    // Metadata attribute a plugin can use (optional convenience)
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class PluginAttribute : Attribute
    {
        public string Id { get; }
        public string Name { get; }
        public string Version { get; }
        public string Author { get; }
        public string? Description { get; }
        public PluginAttribute(string id, string name, string version, string author, string? description = null)
        {
            Id = id; Name = name; Version = version; Author = author; Description = description;
        }
    }

    // The minimum contract a plugin must implement
    public interface IPlugin
    {
        string Id { get; }
        string Name { get; }
        string Version { get; }
        string Author { get; }
        string? Description { get; }

        void OnStartup(IPluginContext context);
        void BeforeLaunch(LaunchContext launchContext);
        void AfterLaunch(LaunchContext launchContext, bool success, Exception? error);
    }

    // Context passed to plugins at startup
    public interface IPluginContext
    {
        string AppDataDirectory { get; }
        string PluginsDirectory { get; }
        void Log(string message);
    }

    // Launch-specific information for hooks
    public class LaunchContext
    {
        public VersionList.VersionInfo? Version { get; set; }
        public CustomClientInfo? CustomClient { get; set; }
        public string? Server { get; set; }
        public string? WorkingDirectory { get; set; }
        public string? ExecutablePath { get; set; }
        public string? Arguments { get; set; }
    }
}
