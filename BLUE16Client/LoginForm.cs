using System;
using System.Windows.Forms;
using System.Drawing;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BLUE16Client
{
    public partial class LoginForm : Form
    {
        private TextBox tokenTextBox;
        private Button loginButton;
        private Label statusLabel;

        public LoginForm()
        {
            InitializeComponent();
            this.Text = "Login with Token";
            this.Size = new Size(400, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            var titleLabel = new Label
            {
                Text = "Enter your authentication token:",
                Location = new Point(20, 20),
                Size = new Size(360, 20),
                Font = new Font("Ubuntu", 10)
            };

            tokenTextBox = new TextBox
            {
                Location = new Point(20, 50),
                Size = new Size(340, 25),
                Font = new Font("Ubuntu", 10),
                UseSystemPasswordChar = true // Hide token like a password
            };

            loginButton = new Button
            {
                Text = "Login",
                Location = new Point(150, 90),
                Size = new Size(100, 30),
                Font = new Font("Ubuntu", 10)
            };

            statusLabel = new Label
            {
                Location = new Point(20, 130),
                Size = new Size(340, 20),
                Font = new Font("Ubuntu", 9),
                ForeColor = Color.Red
            };

            loginButton.Click += async (s, e) => await LoginButton_ClickAsync();

            this.Controls.AddRange(new Control[] { titleLabel, tokenTextBox, loginButton, statusLabel });
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            if (SettingsStore.CurrentCustomTheme != null)
            {
                var theme = SettingsStore.CurrentCustomTheme;
                this.BackColor = theme.BackColor;
                foreach (Control c in this.Controls)
                {
                    if (c is Label)
                        c.ForeColor = theme.LabelColor;
                    else if (c is Button)
                    {
                        c.BackColor = theme.ButtonColor;
                        c.ForeColor = theme.ForeColor;
                    }
                    else if (c is TextBox)
                    {
                        c.BackColor = theme.PanelColor;
                        c.ForeColor = theme.ForeColor;
                    }
                }
            }
            else if (SettingsStore.DarkMode)
            {
                this.BackColor = Color.FromArgb(32, 32, 32);
                foreach (Control c in this.Controls)
                {
                    if (c is Label && c != statusLabel)
                        c.ForeColor = Color.White;
                    else if (c is Button)
                    {
                        c.BackColor = Color.FromArgb(60, 60, 60);
                        c.ForeColor = Color.White;
                    }
                    else if (c is TextBox)
                    {
                        c.BackColor = Color.FromArgb(45, 45, 45);
                        c.ForeColor = Color.White;
                    }
                }
            }
            else
            {
                this.BackColor = SystemColors.Control;
                foreach (Control c in this.Controls)
                {
                    if (c is Label && c != statusLabel)
                        c.ForeColor = Color.Black;
                    else if (c is Button)
                    {
                        c.BackColor = SystemColors.Control;
                        c.ForeColor = Color.Black;
                    }
                    else if (c is TextBox)
                    {
                        c.BackColor = Color.White;
                        c.ForeColor = Color.Black;
                    }
                }
            }
        }

        private async Task LoginButton_ClickAsync()
        {
            string token = tokenTextBox.Text.Trim();
            if (string.IsNullOrEmpty(token))
            {
                statusLabel.Text = "Please enter a token";
                return;
            }

            loginButton.Enabled = false;
            statusLabel.Text = "Validating token...";
            statusLabel.ForeColor = Color.Orange;

            try
            {
                using var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                // Use the selected domain for authentication
                var authUrl = SettingsStore.GetResourceUrl("api/auth/validate");
                var response = await client.GetAsync(authUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<AuthResponse>(json);

                    if (result?.success == true)
                    {
                        SettingsStore.AuthToken = token;
                        SettingsStore.Username = result.username;

                        statusLabel.Text = "Login successful!";
                        statusLabel.ForeColor = Color.Green;

                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        statusLabel.Text = result?.message ?? "Invalid response from server";
                        statusLabel.ForeColor = Color.Red;
                    }
                }
                else
                {
                    statusLabel.Text = $"Auth failed: {response.StatusCode}";
                    statusLabel.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                statusLabel.Text = $"Error: {ex.Message}";
                statusLabel.ForeColor = Color.Red;
            }
            finally
            {
                loginButton.Enabled = true;
            }
        }

        private class AuthResponse
        {
            public bool success { get; set; }
            public string? message { get; set; }
            public string? username { get; set; }
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {

        }
    }
}