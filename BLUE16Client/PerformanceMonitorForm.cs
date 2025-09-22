using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace BLUE16Client
{
    public class PerformanceMonitorForm : Form
    {
        private Label cpuTotalLabel;
        private Label cpuAppLabel;
        private Label memLabel;
        private System.Windows.Forms.Timer timer;
        private PerformanceCounter? cpuTotalCounter;
        private DateTime lastCpuSampleTime = DateTime.UtcNow;
        private TimeSpan lastProcCpu = Process.GetCurrentProcess().TotalProcessorTime;

        public PerformanceMonitorForm()
        {
            Text = "Performance Monitor";
            Width = 380;
            Height = 160;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            StartPosition = FormStartPosition.CenterParent;

            var panel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12),
            };

            var title = new Label
            {
                Text = "Performance",
                Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold),
                AutoSize = true,
                Top = 6,
                Left = 6
            };

            cpuTotalLabel = new Label { Text = "CPU Total: -- %", AutoSize = true, Top = 40, Left = 6 };
            cpuAppLabel = new Label { Text = "CPU App: -- %", AutoSize = true, Top = 65, Left = 6 };
            memLabel = new Label { Text = "Memory: --\nManaged: --", AutoSize = true, Top = 90, Left = 6 };

            panel.Controls.Add(title);
            panel.Controls.Add(cpuTotalLabel);
            panel.Controls.Add(cpuAppLabel);
            panel.Controls.Add(memLabel);
            Controls.Add(panel);

            try
            {
                cpuTotalCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
                _ = cpuTotalCounter.NextValue(); // prime
            }
            catch
            {
                // Ignore if not available
            }

            timer = new System.Windows.Forms.Timer { Interval = 1000 };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            try
            {
                timer.Stop();
                timer.Tick -= Timer_Tick;
                cpuTotalCounter?.Dispose();
            }
            catch { }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            try
            {
                var now = DateTime.UtcNow;
                var proc = Process.GetCurrentProcess();

                var newProcCpu = proc.TotalProcessorTime;
                var cpuMs = (newProcCpu - lastProcCpu).TotalMilliseconds;
                var wallMs = (now - lastCpuSampleTime).TotalMilliseconds;
                var cpuApp = wallMs > 0 ? Math.Min(100.0, Math.Max(0.0, (cpuMs / wallMs) * 100.0 / Environment.ProcessorCount)) : 0.0;
                lastProcCpu = newProcCpu;
                lastCpuSampleTime = now;

                float cpuTotal = 0f;
                if (cpuTotalCounter != null)
                {
                    cpuTotal = cpuTotalCounter.NextValue();
                }

                long working = proc.WorkingSet64;
                long managed = GC.GetTotalMemory(false);

                cpuAppLabel.Text = $"CPU App: {cpuApp:0.0} %";
                cpuTotalLabel.Text = $"CPU Total: {cpuTotal:0.0} %";
                memLabel.Text = $"Memory: {FormatBytes(working)}\nManaged: {FormatBytes(managed)}";
            }
            catch
            {
                // ignore
            }
        }

        private static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024.0;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
