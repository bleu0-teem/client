namespace BLUE16Client
{
    partial class ServerList
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            listBox1 = new ListBox();
            createServerButton = new Button();
            SuspendLayout();
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.Location = new Point(12, 72);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(962, 324);
            listBox1.TabIndex = 0;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // createServerButton
            // 
            createServerButton.Location = new Point(12, 420);
            createServerButton.Name = "createServerButton";
            createServerButton.Size = new Size(962, 36);
            createServerButton.TabIndex = 1;
            createServerButton.Text = "Create your own server";
            createServerButton.UseVisualStyleBackColor = true;
            createServerButton.Click += createServerButton_Click;
            // 
            // ServerList
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(986, 470);
            Controls.Add(createServerButton);
            Controls.Add(listBox1);
            FormBorderStyle = FormBorderStyle.Fixed3D;
            Name = "ServerList";
            Text = "Select Server";
            ResumeLayout(false);
        }

        #endregion

        private ListBox listBox1;
        private Button createServerButton;
    }
}