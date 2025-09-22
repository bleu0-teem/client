using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace BLUE16Client
{
    public static class ClientModManager
    {
        public class ModInfo
        {
            public string Id { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string Version { get; set; } = string.Empty;
            public string Author { get; set; } = string.Empty;
            public string? Description { get; set; }
            public bool Enabled { get; set; } = true;
            public IClientMod? Instance { get; set; }
        }

        private static readonly List<ModInfo> _mods = new List<ModInfo>();
        public static IReadOnlyList<ModInfo> Mods => _mods;

        private static string AppDataDir
        {
            get
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "BLUE16Client");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }

        private static string ModsDir
        {
            get
            {
                var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods");
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                return dir;
            }
        }

        private static string StateFile => Path.Combine(AppDataDir, "mods.json");

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
            _mods.Clear();
            var state = LoadState();

            foreach (var dll in Directory.EnumerateFiles(ModsDir, "*.dll", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    var asm = Assembly.LoadFrom(dll);
                    foreach (var t in asm.GetTypes())
                    {
                        if (!typeof(IClientMod).IsAssignableFrom(t) || t.IsAbstract) continue;
                        if (Activator.CreateInstance(t) is IClientMod mod)
                        {
                            var meta = t.GetCustomAttribute<ClientModAttribute>();
                            var info = new ModInfo
                            {
                                Id = meta?.Id ?? mod.Id,
                                Name = meta?.Name ?? mod.Name,
                                Version = meta?.Version ?? mod.Version,
                                Author = meta?.Author ?? mod.Author,
                                Description = meta?.Description ?? mod.Description,
                                Enabled = state.Enabled.TryGetValue(mod.Id, out var en) ? en : true,
                                Instance = mod
                            };
                            _mods.Add(info);
                        }
                    }
                }
                catch (Exception ex)
                {
                    try { File.AppendAllText(Path.Combine(AppDataDir, "mods_errors.log"), $"[{DateTime.Now}] {dll}: {ex}\n"); } catch { }
                }
            }
        }

        public static void SetEnabled(string modId, bool enabled)
        {
            var m = _mods.FirstOrDefault(x => x.Id == modId);
            if (m != null)
            {
                m.Enabled = enabled;
                PersistEnabled();
            }
        }

        private static void PersistEnabled()
        {
            var state = new PersistedState();
            foreach (var m in _mods)
            {
                state.Enabled[m.Id] = m.Enabled;
            }
            SaveState(state);
        }

        public static void OnPreLaunch(ModContext ctx)
        {
            foreach (var m in _mods.Where(x => x.Enabled && x.Instance != null))
            {
                SafeInvoke(() => m.Instance!.OnPreLaunch(ctx));
            }
        }

        public static void OnPostLaunch(ModContext ctx, bool success, Exception? error)
        {
            foreach (var m in _mods.Where(x => x.Enabled && x.Instance != null))
            {
                SafeInvoke(() => m.Instance!.OnPostLaunch(ctx, success, error));
            }
        }

        private static void SafeInvoke(Action action)
        {
            try { action(); }
            catch (Exception ex)
            {
                try { File.AppendAllText(Path.Combine(AppDataDir, "mods_errors.log"), $"[{DateTime.Now}] {ex}\n"); } catch { }
            }
        }
    }
}
