using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace BLUE16Client
{
    public static class PluginManager
    {
        public class PluginInfo
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Version { get; set; } = string.Empty;
            public string Author { get; set; } = string.Empty;
            public string? Description { get; set; }
            public bool Enabled { get; set; } = true;
            public IPlugin? Instance { get; set; }
        }

        private static readonly List<PluginInfo> _plugins = new List<PluginInfo>();
        public static IReadOnlyList<PluginInfo> Plugins => _plugins;

        private static string AppDataDir
        {
            get
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BLUE16Client");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }

        private static string PluginsDir
        {
            get
            {
                var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }

        private static string StateFile => Path.Combine(AppDataDir, "plugins.json");

        private class PersistedState
        {
            public Dictionary<string, bool> Enabled { get; set; } = new Dictionary<string, bool>();
        }

        private static PersistedState LoadState()
        {
            try
            {
                if (File.Exists(StateFile))
                {
                    var json = File.ReadAllText(StateFile);
                    return JsonSerializer.Deserialize<PersistedState>(json) ?? new PersistedState();
                }
            }
            catch { }
            return new PersistedState();
        }

        private static void SaveState(PersistedState state)
        {
            try
            {
                var json = JsonSerializer.Serialize(state, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(StateFile, json);
            }
            catch { }
        }

        public static void Initialize()
        {
            _plugins.Clear();
            var state = LoadState();

            foreach (var dll in Directory.EnumerateFiles(PluginsDir, "*.dll", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var asm = Assembly.LoadFrom(dll);
                    foreach (var t in asm.GetTypes())
                    {
                        if (!typeof(IPlugin).IsAssignableFrom(t) || t.IsAbstract) continue;
                        if (Activator.CreateInstance(t) is IPlugin plugin)
                        {
                            var meta = t.GetCustomAttribute<PluginAttribute>();
                            var info = new PluginInfo
                            {
                                Id = meta?.Id ?? plugin.Id,
                                Name = meta?.Name ?? plugin.Name,
                                Version = meta?.Version ?? plugin.Version,
                                Author = meta?.Author ?? plugin.Author,
                                Description = meta?.Description ?? plugin.Description,
                                Enabled = state.Enabled.TryGetValue(plugin.Id, out var en) ? en : true,
                                Instance = plugin
                            };
                            _plugins.Add(info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Best-effort: log to a simple file
                    try { File.AppendAllText(Path.Combine(AppDataDir, "plugin_errors.log"), $"[{DateTime.Now}] {dll}: {ex}\n"); } catch { }
                }
            }

            // Call OnStartup for enabled plugins
            var ctx = new DefaultPluginContext(AppDataDir, PluginsDir);
            foreach (var p in _plugins.Where(p => p.Enabled && p.Instance != null))
            {
                SafeInvoke(() => p.Instance!.OnStartup(ctx));
            }
        }

        public static void SetEnabled(string pluginId, bool enabled)
        {
            var p = _plugins.FirstOrDefault(x => x.Id == pluginId);
            if (p != null)
            {
                p.Enabled = enabled;
                PersistEnabled();
            }
        }

        private static void PersistEnabled()
        {
            var state = new PersistedState();
            foreach (var p in _plugins)
            {
                state.Enabled[p.Id] = p.Enabled;
            }
            SaveState(state);
        }

        public static void BeforeLaunch(LaunchContext ctx)
        {
            foreach (var p in _plugins.Where(p => p.Enabled && p.Instance != null))
            {
                SafeInvoke(() => p.Instance!.BeforeLaunch(ctx));
            }
        }

        public static void AfterLaunch(LaunchContext ctx, bool success, Exception? error)
        {
            foreach (var p in _plugins.Where(p => p.Enabled && p.Instance != null))
            {
                SafeInvoke(() => p.Instance!.AfterLaunch(ctx, success, error));
            }
        }

        private static void SafeInvoke(Action action)
        {
            try { action(); }
            catch (Exception ex)
            {
                try { File.AppendAllText(Path.Combine(AppDataDir, "plugin_errors.log"), $"[{DateTime.Now}] {ex}\n"); } catch { }
            }
        }

        private class DefaultPluginContext : IPluginContext
        {
            public string AppDataDirectory { get; }
            public string PluginsDirectory { get; }
            public DefaultPluginContext(string appData, string plugins)
            {
                AppDataDirectory = appData; PluginsDirectory = plugins;
            }
            public void Log(string message)
            {
                try { File.AppendAllText(Path.Combine(AppDataDirectory, "plugin_log.txt"), $"[{DateTime.Now}] {message}\n"); } catch { }
            }
        }
    }
}
