using System;
using System.Drawing;
using System.Windows.Forms;

namespace BLUE16Client
{
    public class DarkMessageBox : Form
    {
        public DarkMessageBox(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            this.Text = caption;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(350, 140);
            this.BackColor = SettingsStore.DarkMode ? Color.FromArgb(32, 32, 32) : SystemColors.Control;

            var label = new Label
            {
                Text = text,
                AutoSize = false,
                Size = new Size(320, 60),
                Location = new Point(15, 15),
                ForeColor = SettingsStore.DarkMode ? Color.White : Color.Black,
                BackColor = Color.Transparent
            };
            this.Controls.Add(label);

            var okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(130, 90),
                Size = new Size(90, 30),
                BackColor = SettingsStore.DarkMode ? Color.FromArgb(60, 60, 60) : SystemColors.Control,
                ForeColor = SettingsStore.DarkMode ? Color.White : Color.Black
            };
            this.Controls.Add(okButton);
            this.AcceptButton = okButton;
        }

        public static DialogResult Show(string text, string caption = "", MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None)
        {
            using (var box = new DarkMessageBox(text, caption, buttons, icon))
            {
                return box.ShowDialog();
            }
        }
    }
}
