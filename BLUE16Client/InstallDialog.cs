using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace BLUE16Client
{
    public partial class InstallDialog : Form
    {
        private ProgressBar progressBar;
        private Label label;
        public InstallDialog()
        {
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Text = "Installing...";
            this.Width = 400;
            this.Height = 120;
            progressBar = new ProgressBar { Style = ProgressBarStyle.Continuous, Minimum = 0, Maximum = 100, Value = 0, Width = 350, Left = 20, Top = 20 };
            label = new Label { Text = "Installing files, please wait...", AutoSize = true, Left = 20, Top = 55 };
            Controls.Add(progressBar);
            Controls.Add(label);
        }
        public async Task StartInstallAsync()
        {
            for (int i = 0; i <= 100; i += 5)
            {
                progressBar.Value = i;
                await Task.Delay(50);
            }
            label.Text = "Installation complete!";
            await Task.Delay(700);
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
