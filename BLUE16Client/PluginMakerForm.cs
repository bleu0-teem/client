using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BLUE16Client
{
    public class PluginMakerForm : Form
    {
        private RadioButton rbLauncher;
        private RadioButton rbClientMod;
        private TextBox tbId;
        private TextBox tbName;
        private TextBox tbVersion;
        private TextBox tbAuthor;
        private TextBox tbDescription;
        private TextBox tbNamespace;
        private TextBox tbClassName;
        private TextBox tbOutDir;
        private Button btnBrowseOut;
        private Button btnGenerate;
        private Button btnBuild;
        private Button btnInstall;
        private Label lblStatus;

        private string? _generatedProjectDir;

        public PluginMakerForm()
        {
            this.Text = "Plugin Maker";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(680, 480);

            int x1 = 20, x2 = 160, w = 460, y = 20, h = 24, gap = 8;

            rbLauncher = new RadioButton { Left = x1, Top = y, Width = 150, Text = "Launcher Plugin", Checked = true };
            rbClientMod = new RadioButton { Left = x1 + 160, Top = y, Width = 180, Text = "Client Mod" };
            y += 36;

            Controls.Add(new Label { Left = x1, Top = y + 4, Width = 130, Text = "ID (reverse DNS):" });
            tbId = new TextBox { Left = x2, Top = y, Width = w, Text = "com.example.myplugin" };
            y += h + gap;

            Controls.Add(new Label { Left = x1, Top = y + 4, Width = 130, Text = "Name:" });
            tbName = new TextBox { Left = x2, Top = y, Width = w, Text = "My Plugin" };
            y += h + gap;

            Controls.Add(new Label { Left = x1, Top = y + 4, Width = 130, Text = "Version:" });
            tbVersion = new TextBox { Left = x2, Top = y, Width = w, Text = "1.0.0" };
            y += h + gap;

            Controls.Add(new Label { Left = x1, Top = y + 4, Width = 130, Text = "Author:" });
            tbAuthor = new TextBox { Left = x2, Top = y, Width = w, Text = Environment.UserName };
            y += h + gap;

            Controls.Add(new Label { Left = x1, Top = y + 4, Width = 130, Text = "Description:" });
            tbDescription = new TextBox { Left = x2, Top = y, Width = w, Text = "Sample plugin/mod" };
            y += h + gap;

            Controls.Add(new Label { Left = x1, Top = y + 4, Width = 130, Text = "Namespace:" });
            tbNamespace = new TextBox { Left = x2, Top = y, Width = w, Text = "MyCompany.Plugins" };
            y += h + gap;

            Controls.Add(new Label { Left = x1, Top = y + 4, Width = 130, Text = "Class Name:" });
            tbClassName = new TextBox { Left = x2, Top = y, Width = w, Text = "MyPlugin" };
            y += h + 16;

            Controls.Add(new Label { Left = x1, Top = y + 4, Width = 130, Text = "Output Folder:" });
            tbOutDir = new TextBox { Left = x2, Top = y, Width = w - 90 };
            btnBrowseOut = new Button { Left = x2 + w - 80, Top = y - 1, Width = 80, Height = h + 2, Text = "Browse" };
            y += h + 16;

            btnGenerate = new Button { Left = x1, Top = y, Width = 140, Text = "Generate Project" };
            btnBuild = new Button { Left = x1 + 150, Top = y, Width = 120, Text = "Build" };
            btnInstall = new Button { Left = x1 + 280, Top = y, Width = 180, Text = "Install to Plugins/Mods" };
            lblStatus = new Label { Left = x1, Top = y + 40, Width = 640, Height = 60, Text = "Ready" };

            Controls.AddRange(new Control[] { rbLauncher, rbClientMod, tbId, tbName, tbVersion, tbAuthor, tbDescription, tbNamespace, tbClassName, tbOutDir, btnBrowseOut, btnGenerate, btnBuild, btnInstall, lblStatus });

            ApplyTheme();

            btnBrowseOut.Click += (s, e) =>
            {
                using var fbd = new FolderBrowserDialog();
                if (fbd.ShowDialog() == DialogResult.OK)
                    tbOutDir.Text = fbd.SelectedPath;
            };

            btnGenerate.Click += (s, e) => GenerateProject();
            btnBuild.Click += (s, e) => BuildProject();
            btnInstall.Click += (s, e) => InstallBuiltDll();
        }

        private void ApplyTheme()
        {
            if (SettingsStore.CurrentCustomTheme != null)
            {
                var t = SettingsStore.CurrentCustomTheme;
                this.BackColor = t.BackColor;
                foreach (Control c in this.Controls)
                {
                    if (c is Label) c.ForeColor = t.LabelColor; else c.ForeColor = t.ForeColor;
                    if (c is Button btn) { btn.BackColor = t.ButtonColor; btn.ForeColor = t.ForeColor; }
                }
                return;
            }
            if (SettingsStore.DarkMode)
            {
                this.BackColor = Color.FromArgb(32, 32, 32);
                foreach (Control c in this.Controls)
                {
                    c.ForeColor = Color.White;
                    if (c is Button btn) { btn.BackColor = Color.FromArgb(60, 60, 60); btn.ForeColor = Color.White; }
                }
            }
        }

        private void GenerateProject()
        {
            lblStatus.Text = "Generating project...";
            try
            {
                if (string.IsNullOrWhiteSpace(tbOutDir.Text))
                    throw new Exception("Please choose an output folder.");
                string projName = Sanitize(tbClassName.Text);
                if (string.IsNullOrWhiteSpace(projName)) projName = "MyPlugin";
                string root = Path.Combine(tbOutDir.Text, projName);
                Directory.CreateDirectory(root);

                // Create csproj targeting net8.0, classlib
                string csproj =
                    "<Project Sdk=\"Microsoft.NET.Sdk\">\n" +
                    "  <PropertyGroup>\n" +
                    "    <TargetFramework>net8.0</TargetFramework>\n" +
                    "    <ImplicitUsings>enable</ImplicitUsings>\n" +
                    "    <Nullable>enable</Nullable>\n" +
                    "    <OutputType>Library</OutputType>\n" +
                    "  </PropertyGroup>\n" +
                    "  <ItemGroup>\n" +
                    "    <ProjectReference Include=\"..\\..\\BLUE16Client\\BLUE16Client.csproj\" />\n" +
                    "  </ItemGroup>\n" +
                    "</Project>\n";
                File.WriteAllText(Path.Combine(root, projName + ".csproj"), csproj);

                // Generate code
                bool isLauncher = rbLauncher.Checked;
                string ns = string.IsNullOrWhiteSpace(tbNamespace.Text) ? "MyCompany.Plugins" : tbNamespace.Text.Trim();
                string className = Sanitize(tbClassName.Text);
                if (string.IsNullOrWhiteSpace(className)) className = "MyPlugin";
                string id = tbId.Text.Trim();
                string name = tbName.Text.Trim();
                string version = tbVersion.Text.Trim();
                string author = tbAuthor.Text.Trim();
                string? desc = tbDescription.Text.Trim();

                string code = isLauncher ? GenerateLauncherPlugin(ns, className, id, name, version, author, desc)
                                         : GenerateClientMod(ns, className, id, name, version, author, desc);
                File.WriteAllText(Path.Combine(root, className + ".cs"), code);

                _generatedProjectDir = root;
                lblStatus.Text = $"Project generated at: {root}";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Error: " + ex.Message;
            }
        }

        private string GenerateLauncherPlugin(string ns, string className, string id, string name, string version, string author, string? desc)
        {
            return
                "using System;\n" +
                "using BLUE16Client;\n\n" +
                "namespace " + ns + "\n" +
                "{\n" +
                "    [Plugin(\"" + id + "\", \"" + Escape(name) + "\", \"" + Escape(version) + "\", \"" + Escape(author) + "\", \"" + Escape(desc) + "\")]\n" +
                "    public class " + className + " : IPlugin\n" +
                "    {\n" +
                "        public string Id => \"" + id + "\";\n" +
                "        public string Name => \"" + Escape(name) + "\";\n" +
                "        public string Version => \"" + Escape(version) + "\";\n" +
                "        public string Author => \"" + Escape(author) + "\";\n" +
                "        public string? Description => \"" + Escape(desc) + "\";\n\n" +
                "        private IPluginContext? _ctx;\n\n" +
                "        public void OnStartup(IPluginContext context)\n" +
                "        {\n" +
                "            _ctx = context;\n" +
                "            _ctx.Log(\"[" + className + "] OnStartup\");\n" +
                "        }\n\n" +
                "        public void BeforeLaunch(LaunchContext launchContext)\n" +
                "        {\n" +
                "            _ctx?.Log(\"[" + className + "] BeforeLaunch exe=\" + launchContext.ExecutablePath + \" args=\" + launchContext.Arguments);\n" +
                "        }\n\n" +
                "        public void AfterLaunch(LaunchContext launchContext, bool success, Exception? error)\n" +
                "        {\n" +
                "            _ctx?.Log(\"[" + className + "] AfterLaunch success=\" + success + \" error=\" + (error?.Message));\n" +
                "        }\n" +
                "    }\n" +
                "}\n";
        }

        private string GenerateClientMod(string ns, string className, string id, string name, string version, string author, string? desc)
        {
            return
                "using System;\n" +
                "using BLUE16Client;\n\n" +
                "namespace " + ns + "\n" +
                "{\n" +
                "    [ClientMod(\"" + id + "\", \"" + Escape(name) + "\", \"" + Escape(version) + "\", \"" + Escape(author) + "\", \"" + Escape(desc) + "\")]\n" +
                "    public class " + className + " : IClientMod\n" +
                "    {\n" +
                "        public string Id => \"" + id + "\";\n" +
                "        public string Name => \"" + Escape(name) + "\";\n" +
                "        public string Version => \"" + Escape(version) + "\";\n" +
                "        public string Author => \"" + Escape(author) + "\";\n" +
                "        public string? Description => \"" + Escape(desc) + "\";\n\n" +
                "        public void OnPreLaunch(ModContext context)\n" +
                "        {\n" +
                "            // Called before the Roblox client starts\n" +
                "        }\n\n" +
                "        public void OnPostLaunch(ModContext context, bool success, Exception? error)\n" +
                "        {\n" +
                "            // Called after the Roblox client is launched (or failed)\n" +
                "        }\n" +
                "    }\n" +
                "}\n";
        }

        private void BuildProject()
        {
            if (string.IsNullOrEmpty(_generatedProjectDir) || !Directory.Exists(_generatedProjectDir))
            {
                lblStatus.Text = "Generate a project first.";
                return;
            }
            try
            {
                lblStatus.Text = "Building project (dotnet build)...";
                var psi = new ProcessStartInfo("dotnet", "build \"" + _generatedProjectDir + "\"")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                var p = Process.Start(psi);
                p!.WaitForExit();
                var output = p.StandardOutput.ReadToEnd();
                var error = p.StandardError.ReadToEnd();
                lblStatus.Text = p.ExitCode == 0 ? "Build succeeded." : ("Build failed: " + error);
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Build error: " + ex.Message;
            }
        }

        private void InstallBuiltDll()
        {
            if (string.IsNullOrEmpty(_generatedProjectDir))
            {
                lblStatus.Text = "Generate and build the project first.";
                return;
            }
            try
            {
                // Try to find the newest dll under bin/**/ (Debug preferred)
                var bin = Path.Combine(_generatedProjectDir, "bin");
                if (!Directory.Exists(bin)) throw new Exception("No bin folder found. Build first.");
                var dll = Directory.EnumerateFiles(bin, "*.dll", SearchOption.AllDirectories)
                    .OrderByDescending(File.GetLastWriteTimeUtc)
                    .FirstOrDefault();
                if (dll == null) throw new Exception("No DLL found in bin. Build first.");

                string destDir = rbLauncher.Checked
                    ? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Plugins")
                    : Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Mods");
                Directory.CreateDirectory(destDir);
                var dest = Path.Combine(destDir, Path.GetFileName(dll));
                File.Copy(dll, dest, true);
                lblStatus.Text = $"Installed to: {dest}";
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Install error: " + ex.Message;
            }
        }

        private static string Sanitize(string input)
        {
            var sb = new StringBuilder();
            foreach (char c in input)
            {
                if (char.IsLetterOrDigit(c) || c == '_') sb.Append(c);
            }
            var s = sb.ToString();
            return string.IsNullOrWhiteSpace(s) ? "MyPlugin" : s;
        }
        private static string Escape(string? input) => (input ?? string.Empty).Replace("\"", "\\\"");
    }
}
