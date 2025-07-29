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
            SuspendLayout();
            // 
            // checkBoxDarkMode
            // 
            checkBoxDarkMode.AutoSize = true;
            checkBoxDarkMode.Location = new Point(19, 22);
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
            comboBoxLanguage.Location = new Point(19, 72);
            comboBoxLanguage.Name = "comboBoxLanguage";
            comboBoxLanguage.Size = new Size(151, 28);
            comboBoxLanguage.TabIndex = 1;
            // 
            // labelLanguage
            // 
            labelLanguage.AutoSize = true;
            labelLanguage.Location = new Point(19, 49);
            labelLanguage.Name = "labelLanguage";
            labelLanguage.Size = new Size(77, 20);
            labelLanguage.TabIndex = 2;
            labelLanguage.Text = "Language:";
            // 
            // buttonSave
            // 
            buttonSave.Location = new Point(148, 182);
            buttonSave.Name = "buttonSave";
            buttonSave.Size = new Size(94, 29);
            buttonSave.TabIndex = 3;
            buttonSave.Text = "Save";
            buttonSave.UseVisualStyleBackColor = true;
            buttonSave.Click += buttonSave_Click;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(326, 236);
            Controls.Add(buttonSave);
            Controls.Add(labelLanguage);
            Controls.Add(comboBoxLanguage);
            Controls.Add(checkBoxDarkMode);
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
    }
}
