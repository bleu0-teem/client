namespace BLUE16Client
{
    partial class SettingsForm
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }
        private void InitializeComponent()
        {
            checkBoxDarkMode = new CheckBox();
            comboBoxLanguage = new ComboBox();
            labelLanguage = new Label();
            buttonSave = new Button();
            checkBoxDiscordRpc = new CheckBox();
            comboBoxDomain = new ComboBox();
            labelDomain = new Label();
            comboBoxTheme = new ComboBox();
            buttonImportTheme = new Button();
            buttonExportTheme = new Button();
            labelTheme = new Label();
            SuspendLayout();
            // 
            // checkBoxDarkMode
            // 
            checkBoxDarkMode.AutoSize = true;
            checkBoxDarkMode.Location = new Point(19, 9);
            checkBoxDarkMode.Name = "checkBoxDarkMode";
            checkBoxDarkMode.RightToLeft = RightToLeft.Yes;
            checkBoxDarkMode.Size = new Size(105, 24);
            checkBoxDarkMode.TabIndex = 0;
            checkBoxDarkMode.Text = "Dark Mode";
            checkBoxDarkMode.UseVisualStyleBackColor = true;
            // 
            // comboBoxLanguage
            // 
            comboBoxLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxLanguage.FormattingEnabled = true;
            comboBoxLanguage.Items.AddRange(new object[] { "English", "Polski", "Deutsch" });
            comboBoxLanguage.Location = new Point(19, 89);
            comboBoxLanguage.Name = "comboBoxLanguage";
            comboBoxLanguage.Size = new Size(151, 28);
            comboBoxLanguage.TabIndex = 1;
            // 
            // labelLanguage
            // 
            labelLanguage.AutoSize = true;
            labelLanguage.Location = new Point(19, 66);
            labelLanguage.Name = "labelLanguage";
            labelLanguage.Size = new Size(77, 20);
            labelLanguage.TabIndex = 2;
            labelLanguage.Text = "Language:";
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(130, 321);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(94, 29);
            buttonSave.TabIndex = 3;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // checkBoxDiscordRpc
            // 
            checkBoxDiscordRpc.AutoSize = true;
            checkBoxDiscordRpc.Checked = true;
            checkBoxDiscordRpc.CheckState = CheckState.Checked;
            checkBoxDiscordRpc.Location = new Point(19, 39);
            checkBoxDiscordRpc.Name = "checkBoxDiscordRpc";
            checkBoxDiscordRpc.RightToLeft = RightToLeft.Yes;
            checkBoxDiscordRpc.Size = new Size(225, 24);
            checkBoxDiscordRpc.TabIndex = 4;
            checkBoxDiscordRpc.Text = "Enable Discord Rich Presence";
            checkBoxDiscordRpc.UseVisualStyleBackColor = true;
            // 
            // comboBoxDomain
            // 
            comboBoxDomain.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxDomain.FormattingEnabled = true;
            comboBoxDomain.Items.AddRange(new object[] { "github.com", "blue16.site", "Custom..." });
            comboBoxDomain.Location = new Point(19, 143);
            comboBoxDomain.Name = "comboBoxDomain";
            comboBoxDomain.Size = new Size(151, 28);
            comboBoxDomain.TabIndex = 5;
            // 
            // labelDomain
            // 
            labelDomain.AutoSize = true;
            labelDomain.Location = new Point(19, 120);
            labelDomain.Name = "labelDomain";
            labelDomain.Size = new Size(110, 20);
            labelDomain.TabIndex = 6;
            labelDomain.Text = "Server Domain:";
            // 
            // comboBoxTheme
            // 
            comboBoxTheme.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBoxTheme.FormattingEnabled = true;
            comboBoxTheme.Items.AddRange(new object[] { "Default", "Dark", "Custom..." });
            comboBoxTheme.Location = new Point(19, 259);
            comboBoxTheme.Name = "comboBoxTheme";
            comboBoxTheme.Size = new Size(151, 28);
            comboBoxTheme.TabIndex = 7;
            // 
            // buttonImportTheme
            // 
            buttonImportTheme.Location = new Point(180, 259);
            buttonImportTheme.Name = "buttonImportTheme";
            buttonImportTheme.Size = new Size(90, 28);
            buttonImportTheme.TabIndex = 9;
            buttonImportTheme.Text = "Import";
            buttonImportTheme.UseVisualStyleBackColor = true;
            // 
            // buttonExportTheme
            // 
            buttonExportTheme.Location = new Point(280, 259);
            buttonExportTheme.Name = "buttonExportTheme";
            buttonExportTheme.Size = new Size(90, 28);
            buttonExportTheme.TabIndex = 10;
            buttonExportTheme.Text = "Export";
            buttonExportTheme.UseVisualStyleBackColor = true;
            // 
            // labelTheme
            // 
            labelTheme.AutoSize = true;
            labelTheme.Location = new Point(19, 236);
            labelTheme.Name = "labelTheme";
            labelTheme.Size = new Size(89, 20);
            labelTheme.TabIndex = 8;
            labelTheme.Text = "App Theme:";
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(381, 362);
            Controls.Add(buttonSave);
            Controls.Add(labelLanguage);
            Controls.Add(comboBoxLanguage);
            Controls.Add(checkBoxDarkMode);
            Controls.Add(checkBoxDiscordRpc);
            Controls.Add(comboBoxDomain);
            Controls.Add(labelDomain);
            Controls.Add(comboBoxTheme);
            Controls.Add(labelTheme);
            Controls.Add(buttonImportTheme);
            Controls.Add(buttonExportTheme);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsForm";
            Text = "Settings";
            Load += SettingsForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }
        private System.Windows.Forms.CheckBox checkBoxDarkMode;
        private System.Windows.Forms.ComboBox comboBoxLanguage;
        private System.Windows.Forms.Label labelLanguage;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.CheckBox checkBoxDiscordRpc;
        private System.Windows.Forms.ComboBox comboBoxDomain;
        private System.Windows.Forms.Label labelDomain;
        private System.Windows.Forms.ComboBox comboBoxTheme;
        private System.Windows.Forms.Button buttonImportTheme;
        private System.Windows.Forms.Button buttonExportTheme;
        private System.Windows.Forms.Label labelTheme;
    }
}
