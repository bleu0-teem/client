using System;

namespace BLUE16Client
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class ClientModAttribute : Attribute
    {
        public string Id { get; }
        public string Name { get; }
        public string Version { get; }
        public string Author { get; }
        public string? Description { get; }
        public ClientModAttribute(string id, string name, string version, string author, string? description = null)
        {
            Id = id; Name = name; Version = version; Author = author; Description = description;
        }
    }

    public interface IClientMod
    {
        string Id { get; }
        string Name { get; }
        string Version { get; }
        string Author { get; }
        string? Description { get; }

        // Called before the Roblox client process is started
        void OnPreLaunch(ModContext context);
        // Called after the Roblox client process is started (or failed)
        void OnPostLaunch(ModContext context, bool success, Exception? error);
    }

    public class ModContext
    {
        public string? WorkingDirectory { get; set; }
        public string? ExecutablePath { get; set; }
        public string? Arguments { get; set; }
        public VersionList.VersionInfo? Version { get; set; }
        public CustomClientInfo? CustomClient { get; set; }
        public string? Server { get; set; }
    }
}
