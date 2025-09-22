using System;
using System.Drawing;
using System.Windows.Forms;

namespace BLUE16Client
{
    public class ThemeBuilderForm : Form
    {
        private TextBox nameBox;
        private Button pickBackColorBtn;
        private Button pickForeColorBtn;
        private Button pickPanelColorBtn;
        private Button pickButtonColorBtn;
        private Button pickLabelColorBtn;
        private Button pickFontBtn;
        private Panel previewPanel;
        private Button saveBtn;
        private Button cancelBtn;

        private SettingsStore.CustomTheme workingTheme = new SettingsStore.CustomTheme();

        public ThemeBuilderForm()
        {
            this.Text = "Theme Builder";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.ClientSize = new Size(520, 360);

            nameBox = new TextBox { Left = 20, Top = 20, Width = 200, Text = workingTheme.Name };
            var nameLbl = new Label { Left = 20, Top = 0 + 8, Text = "Theme Name:" };

            pickBackColorBtn = new Button { Left = 20, Top = 60, Width = 180, Text = "Background Color" };
            pickForeColorBtn = new Button { Left = 20, Top = 100, Width = 180, Text = "Foreground Color" };
            pickPanelColorBtn = new Button { Left = 20, Top = 140, Width = 180, Text = "Panel Color" };
            pickButtonColorBtn = new Button { Left = 20, Top = 180, Width = 180, Text = "Button Color" };
            pickLabelColorBtn = new Button { Left = 20, Top = 220, Width = 180, Text = "Label Color" };
            pickFontBtn = new Button { Left = 20, Top = 260, Width = 180, Text = "Pick Font" };

            previewPanel = new Panel { Left = 220, Top = 20, Width = 280, Height = 240, BorderStyle = BorderStyle.FixedSingle };
            saveBtn = new Button { Left = 320, Top = 280, Width = 80, Text = "Save" };
            cancelBtn = new Button { Left = 420, Top = 280, Width = 80, Text = "Cancel" };

            pickBackColorBtn.Click += (s, e) => PickColor(c => workingTheme.BackColor = c);
            pickForeColorBtn.Click += (s, e) => PickColor(c => workingTheme.ForeColor = c);
            pickPanelColorBtn.Click += (s, e) => PickColor(c => workingTheme.PanelColor = c);
            pickButtonColorBtn.Click += (s, e) => PickColor(c => workingTheme.ButtonColor = c);
            pickLabelColorBtn.Click += (s, e) => PickColor(c => workingTheme.LabelColor = c);
            pickFontBtn.Click += (s, e) =>
            {
                using (var fd = new FontDialog())
                {
                    if (workingTheme.MainFont != null)
                    {
                        fd.Font = workingTheme.MainFont;
                    }
                    if (fd.ShowDialog() == DialogResult.OK)
                    {
                        workingTheme.MainFont = fd.Font;
                        UpdatePreview();
                    }
                }
            };

            saveBtn.Click += (s, e) =>
            {
                workingTheme.Name = string.IsNullOrWhiteSpace(nameBox.Text) ? "Custom" : nameBox.Text.Trim();
                SettingsStore.CurrentCustomTheme = workingTheme;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };
            cancelBtn.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            this.Controls.Add(nameLbl);
            this.Controls.Add(nameBox);
            this.Controls.Add(pickBackColorBtn);
            this.Controls.Add(pickForeColorBtn);
            this.Controls.Add(pickPanelColorBtn);
            this.Controls.Add(pickButtonColorBtn);
            this.Controls.Add(pickLabelColorBtn);
            this.Controls.Add(pickFontBtn);
            this.Controls.Add(previewPanel);
            this.Controls.Add(saveBtn);
            this.Controls.Add(cancelBtn);

            ApplyThemeToSelf();
            UpdatePreview();
        }

        private void PickColor(Action<Color> setter)
        {
            using (var cd = new ColorDialog())
            {
                if (cd.ShowDialog() == DialogResult.OK)
                {
                    setter(cd.Color);
                    UpdatePreview();
                }
            }
        }

        private void UpdatePreview()
        {
            previewPanel.BackColor = workingTheme.PanelColor;
            previewPanel.Controls.Clear();
            var lbl = new Label { Left = 12, Top = 12, Text = "Preview Label", ForeColor = workingTheme.LabelColor };
            var txt = new TextBox { Left = 12, Top = 40, Width = 200, BackColor = workingTheme.PanelColor, ForeColor = workingTheme.ForeColor, Text = "Preview Text" };
            var btn = new Button { Left = 12, Top = 80, Width = 140, Text = "Preview Button", BackColor = workingTheme.ButtonColor, ForeColor = workingTheme.ForeColor };
            if (workingTheme.MainFont != null)
            {
                lbl.Font = workingTheme.MainFont;
                txt.Font = workingTheme.MainFont;
                btn.Font = workingTheme.MainFont;
            }
            previewPanel.Controls.Add(lbl);
            previewPanel.Controls.Add(txt);
            previewPanel.Controls.Add(btn);
            this.BackColor = workingTheme.BackColor;
        }

        private void ApplyThemeToSelf()
        {
            if (SettingsStore.CurrentCustomTheme != null)
            {
                // Start from existing theme values
                workingTheme = new SettingsStore.CustomTheme
                {
                    Name = SettingsStore.CurrentCustomTheme.Name,
                    BackColor = SettingsStore.CurrentCustomTheme.BackColor,
                    ForeColor = SettingsStore.CurrentCustomTheme.ForeColor,
                    PanelColor = SettingsStore.CurrentCustomTheme.PanelColor,
                    ButtonColor = SettingsStore.CurrentCustomTheme.ButtonColor,
                    LabelColor = SettingsStore.CurrentCustomTheme.LabelColor,
                    MainFont = SettingsStore.CurrentCustomTheme.MainFont
                };
            }
            else if (SettingsStore.DarkMode)
            {
                workingTheme.BackColor = Color.FromArgb(32, 32, 32);
                workingTheme.ForeColor = Color.White;
                workingTheme.PanelColor = Color.FromArgb(45, 45, 45);
                workingTheme.ButtonColor = Color.FromArgb(60, 60, 60);
                workingTheme.LabelColor = Color.White;
            }
            else
            {
                workingTheme.BackColor = SystemColors.Control;
                workingTheme.ForeColor = Color.Black;
                workingTheme.PanelColor = Color.White;
                workingTheme.ButtonColor = SystemColors.Control;
                workingTheme.LabelColor = Color.Black;
            }
        }
    }
}
