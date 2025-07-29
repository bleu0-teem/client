namespace BLUE16Client
{
    partial class Home
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Home));
            startgame = new Button();
            iconbutton = new Button();
            playbutton = new Button();
            profilebutton = new Button();
            settingsbutton = new Button();
            label1 = new Label();
            label2 = new Label();
            panel1 = new Panel();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            panel2 = new Panel();
            label6 = new Label();
            panel3 = new Panel();
            label7 = new Label();
            installProgressBar = new ProgressBar();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel3.SuspendLayout();
            SuspendLayout();
            // 
            // startgame
            // 
            startgame.FlatStyle = FlatStyle.Popup;
            startgame.Font = new Font("Segoe UI", 20F);
            startgame.Location = new Point(514, 359);
            startgame.Name = "startgame";
            startgame.Size = new Size(274, 79);
            startgame.TabIndex = 2;
            startgame.Text = "PLAY";
            startgame.UseVisualStyleBackColor = true;
            startgame.Click += button3_Click;
            // 
            // iconbutton
            // 
            iconbutton.FlatAppearance.BorderSize = 0;
            iconbutton.BackColor = Color.White;
            iconbutton.FlatStyle = FlatStyle.Flat;
            iconbutton.Location = new Point(-2, 0);
            iconbutton.Name = "iconbutton";
            iconbutton.Size = new Size(74, 65);
            iconbutton.TabIndex = 6;
            //iconbutton.Text = "icon";
            iconbutton.UseVisualStyleBackColor = true;
            // 
            // playbutton
            // 
            playbutton.FlatAppearance.BorderSize = 0;
            playbutton.FlatStyle = FlatStyle.Flat;
            playbutton.Location = new Point(-2, 65);
            playbutton.Name = "playbutton";
            playbutton.Size = new Size(74, 65);
            playbutton.TabIndex = 7;
            //playbutton.Text = "play";
            playbutton.UseVisualStyleBackColor = true;
            playbutton.BackColor = Color.White;
            // 
            // profilebutton
            // 
            profilebutton.FlatAppearance.BorderSize = 0;
            profilebutton.FlatStyle = FlatStyle.Flat;
            profilebutton.BackColor = Color.White;
            // Smooth only the bottom-right corner after layout
            profilebutton.Paint += (s, e) => {
                int radius = 18;
                var rect = new System.Drawing.Rectangle(0, 0, profilebutton.Width, profilebutton.Height);
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    // Top-left
                    path.AddLine(rect.Left, rect.Top, rect.Right, rect.Top);
                    // Top-right
                    path.AddLine(rect.Right, rect.Top, rect.Right, rect.Bottom - radius);
                    // Bottom-right (rounded)
                    path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
                    // Bottom-left
                    path.AddLine(rect.Right - radius, rect.Bottom, rect.Left, rect.Bottom);
                    path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top);
                    path.CloseFigure();
                    profilebutton.Region = new System.Drawing.Region(path);
                }
            };
            profilebutton.Location = new Point(-2, 130);
            profilebutton.Name = "profilebutton";
            profilebutton.Size = new Size(74, 65);
            profilebutton.TabIndex = 8;
            //profilebutton.Text = "profile";
            profilebutton.UseVisualStyleBackColor = true;
            // 
            // settingsbutton
            // 
            settingsbutton.FlatAppearance.BorderSize = 0;
            settingsbutton.FlatStyle = FlatStyle.Flat;
            settingsbutton.BackColor = Color.White;
            settingsbutton.UseVisualStyleBackColor = false;
            // Smooth only the top-right corner after layout
            settingsbutton.Paint += (s, e) => {
                int radius = 18;
                var rect = new System.Drawing.Rectangle(0, 0, settingsbutton.Width, settingsbutton.Height);
                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    // Top-left
                    path.AddLine(rect.Left, rect.Top, rect.Right - radius, rect.Top);
                    // Top-right (rounded)
                    path.AddArc(rect.Right - radius * 2, rect.Top, radius * 2, radius * 2, 270, 90);
                    // Right side
                    path.AddLine(rect.Right, rect.Top + radius, rect.Right, rect.Bottom);
                    // Bottom
                    path.AddLine(rect.Right, rect.Bottom, rect.Left, rect.Bottom);
                    // Left side
                    path.AddLine(rect.Left, rect.Bottom, rect.Left, rect.Top);
                    path.CloseFigure();
                    settingsbutton.Region = new System.Drawing.Region(path);
                }
            };
            settingsbutton.Location = new Point(-2, 384);
            settingsbutton.Name = "settingsbutton";
            settingsbutton.Size = new Size(74, 69);
            settingsbutton.TabIndex = 9;
            // settingsbutton.Text = " ";
            settingsbutton.UseVisualStyleBackColor = true;
            settingsbutton.Click += settingsbutton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(291, 374);
            label1.Name = "label1";
            label1.Size = new Size(60, 20);
            label1.TabIndex = 11;
            label1.Text = "Version:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(514, 336);
            label2.Name = "label2";
            label2.Size = new Size(62, 20);
            label2.TabIndex = 12;
            label2.Text = "STATUS:";
            label2.Click += label2_Click;
            // 
            // panel1
            // 
            panel1.AutoScroll = true;
            panel1.BackColor = Color.White;
            panel1.Controls.Add(label3);
            panel1.Location = new Point(78, 29);
            panel1.Name = "panel1";
            panel1.Size = new Size(710, 304);
            panel1.TabIndex = 13;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(14, 10);
            label3.Name = "label3";
            label3.Size = new Size(282, 520);
            label3.TabIndex = 0;
            label3.Text = "";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(78, 6);
            label4.Name = "label4";
            label4.Size = new Size(308, 20);
            label4.TabIndex = 14;
            label4.Text = "Version Information";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(291, 408);
            label5.Name = "label5";
            label5.Size = new Size(53, 20);
            label5.TabIndex = 16;
            label5.Text = "Server:";
            // 
            // panel2
            // 
            panel2.BackColor = Color.White;
            panel2.Controls.Add(label6);
            panel2.Location = new Point(357, 405);
            panel2.Name = "panel2";
            panel2.Size = new Size(151, 28);
            panel2.TabIndex = 17;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(2, 4);
            label6.Name = "label6";
            label6.Size = new Size(100, 20);
            label6.TabIndex = 18;
            label6.Text = "Click to select";
            // 
            // panel3
            // 
            panel3.BackColor = Color.White;
            panel3.Controls.Add(label7);
            panel3.Location = new Point(357, 371);
            panel3.Name = "panel3";
            panel3.Size = new Size(151, 28);
            panel3.TabIndex = 18;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(2, 4);
            label7.Name = "label7";
            label7.Size = new Size(100, 20);
            label7.TabIndex = 18;
            label7.Text = "Click to select";
            // 
            // installProgressBar
            // 
            installProgressBar.Location = new Point(514, 445);
            installProgressBar.Name = "installProgressBar";
            installProgressBar.Size = new Size(274, 23);
            installProgressBar.TabIndex = 19;
            installProgressBar.Visible = false;
            // 
            // Home
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.Gainsboro;
            ClientSize = new Size(800, 450);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(panel1);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(settingsbutton);
            Controls.Add(profilebutton);
            Controls.Add(playbutton);
            Controls.Add(iconbutton);
            Controls.Add(startgame);
            Controls.Add(installProgressBar);
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = false;
            Name = "Home";
            Text = " ";
            Load += Home_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            panel3.ResumeLayout(false);
            panel3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
            // Set images for buttons from resources
            Size iconSize = new Size(45, 45);

            iconbutton.Image = new Bitmap(Properties.Resources.icon, iconSize);
            playbutton.Image = new Bitmap(Properties.Resources.play, iconSize);
            profilebutton.Image = new Bitmap(Properties.Resources.profile, iconSize);
            settingsbutton.Image = new Bitmap(Properties.Resources.settings, iconSize);

            // Center the image
            iconbutton.ImageAlign = ContentAlignment.MiddleCenter;
            playbutton.ImageAlign = ContentAlignment.MiddleCenter;
            profilebutton.ImageAlign = ContentAlignment.MiddleCenter;
            settingsbutton.ImageAlign = ContentAlignment.MiddleCenter;
        }

        #endregion
        private Button startgame;
        private Button iconbutton;
        private Button playbutton;
        private Button profilebutton;
        private Button settingsbutton;
        private Label label1;
        private Label label2;
        private Panel panel1;
        private Label label3;
        private Label label4;
        private Label label5;
        private Panel panel2;
        private Label label6;
        private Panel panel3;
        private Label label7;
        private ProgressBar installProgressBar;
    }
}
