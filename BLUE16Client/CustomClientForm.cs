using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;

namespace BLUE16Client
{
    public partial class CustomClientForm : Form
    {
        private CustomClientInfo? _editingClient;
        private bool _isEditMode = false;

        public CustomClientForm()
        {
            InitializeComponent();
            ApplyTheme();
            this.Font = new Font("Ubuntu", 10F, FontStyle.Regular);
            this.Text = "Add Custom Client";
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        public CustomClientForm(CustomClientInfo client) : this()
        {
            _editingClient = client;
            _isEditMode = true;
            this.Text = "Edit Custom Client";
            LoadClientData();
        }

        private void InitializeComponent()
        {
            this.labelName = new Label();
            this.labelRccs = new Label();
            this.labelClient = new Label();
            this.labelArgs = new Label();
            this.labelServer = new Label();
            this.labelServerArgs = new Label();
            this.textBoxName = new TextBox();
            this.textBoxRccs = new TextBox();
            this.textBoxClient = new TextBox();
            this.textBoxArgs = new TextBox();
            this.textBoxServer = new TextBox();
            this.textBoxServerArgs = new TextBox();
            this.buttonBrowseRccs = new Button();
            this.buttonBrowseClient = new Button();
            this.buttonBrowseServer = new Button();
            this.checkBoxAutoLaunch = new CheckBox();
            this.buttonTest = new Button();
            this.buttonSave = new Button();
            this.buttonCancel = new Button();
            this.labelStatus = new Label();

            // labelName
            this.labelName.AutoSize = true;
            this.labelName.Location = new Point(12, 15);
            this.labelName.Size = new Size(80, 20);
            this.labelName.Text = "Client Name:";

            // textBoxName
            this.textBoxName.Location = new Point(98, 12);
            this.textBoxName.Size = new Size(250, 23);

            // labelRccs
            this.labelRccs.AutoSize = true;
            this.labelRccs.Location = new Point(12, 45);
            this.labelRccs.Size = new Size(80, 20);
            this.labelRccs.Text = "RCCS Path:";

            // textBoxRccs
            this.textBoxRccs.Location = new Point(98, 42);
            this.textBoxRccs.Size = new Size(200, 23);

            // buttonBrowseRccs
            this.buttonBrowseRccs.Location = new Point(304, 41);
            this.buttonBrowseRccs.Size = new Size(44, 25);
            this.buttonBrowseRccs.Text = "...";
            this.buttonBrowseRccs.Click += ButtonBrowseRccs_Click;

            // labelClient
            this.labelClient.AutoSize = true;
            this.labelClient.Location = new Point(12, 75);
            this.labelClient.Size = new Size(80, 20);
            this.labelClient.Text = "Client Path:";

            // textBoxClient
            this.textBoxClient.Location = new Point(98, 72);
            this.textBoxClient.Size = new Size(200, 23);

            // buttonBrowseClient
            this.buttonBrowseClient.Location = new Point(304, 71);
            this.buttonBrowseClient.Size = new Size(44, 25);
            this.buttonBrowseClient.Text = "...";
            this.buttonBrowseClient.Click += ButtonBrowseClient_Click;

            // labelArgs
            this.labelArgs.AutoSize = true;
            this.labelArgs.Location = new Point(12, 105);
            this.labelArgs.Size = new Size(80, 20);
            this.labelArgs.Text = "Arguments:";

            // textBoxArgs
            this.textBoxArgs.Location = new Point(98, 102);
            this.textBoxArgs.Size = new Size(250, 23);

            // checkBoxAutoLaunch
            this.checkBoxAutoLaunch.AutoSize = true;
            this.checkBoxAutoLaunch.Location = new Point(12, 135);
            this.checkBoxAutoLaunch.Size = new Size(150, 20);
            this.checkBoxAutoLaunch.Text = "Auto-launch server";
            this.checkBoxAutoLaunch.Checked = true;
            this.checkBoxAutoLaunch.CheckedChanged += CheckBoxAutoLaunch_CheckedChanged;

            // labelServer
            this.labelServer.AutoSize = true;
            this.labelServer.Location = new Point(12, 165);
            this.labelServer.Size = new Size(80, 20);
            this.labelServer.Text = "Server Path:";

            // textBoxServer
            this.textBoxServer.Location = new Point(98, 162);
            this.textBoxServer.Size = new Size(200, 23);

            // buttonBrowseServer
            this.buttonBrowseServer.Location = new Point(304, 161);
            this.buttonBrowseServer.Size = new Size(44, 25);
            this.buttonBrowseServer.Text = "...";
            this.buttonBrowseServer.Click += ButtonBrowseServer_Click;

            // labelServerArgs
            this.labelServerArgs.AutoSize = true;
            this.labelServerArgs.Location = new Point(12, 195);
            this.labelServerArgs.Size = new Size(80, 20);
            this.labelServerArgs.Text = "Server Args:";

            // textBoxServerArgs
            this.textBoxServerArgs.Location = new Point(98, 192);
            this.textBoxServerArgs.Size = new Size(250, 23);

            // buttonTest
            this.buttonTest.Location = new Point(12, 230);
            this.buttonTest.Size = new Size(80, 30);
            this.buttonTest.Text = "Test";
            this.buttonTest.Click += ButtonTest_Click;

            // buttonSave
            this.buttonSave.Location = new Point(200, 230);
            this.buttonSave.Size = new Size(80, 30);
            this.buttonSave.Text = "Save";
            this.buttonSave.Click += ButtonSave_Click;

            // buttonCancel
            this.buttonCancel.Location = new Point(286, 230);
            this.buttonCancel.Size = new Size(80, 30);
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += ButtonCancel_Click;

            // labelStatus
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new Point(12, 270);
            this.labelStatus.Size = new Size(350, 20);
            this.labelStatus.Text = "Ready";

            // CustomClientForm
            this.ClientSize = new Size(378, 300);
            this.Controls.AddRange(new Control[] {
                this.labelName, this.textBoxName,
                this.labelRccs, this.textBoxRccs, this.buttonBrowseRccs,
                this.labelClient, this.textBoxClient, this.buttonBrowseClient,
                this.labelArgs, this.textBoxArgs,
                this.checkBoxAutoLaunch,
                this.labelServer, this.textBoxServer, this.buttonBrowseServer,
                this.labelServerArgs, this.textBoxServerArgs,
                this.buttonTest, this.buttonSave, this.buttonCancel,
                this.labelStatus
            });
        }

        private void ApplyTheme()
        {
            if (SettingsStore.DarkMode)
            {
                this.BackColor = Color.FromArgb(32, 32, 32);
                this.ForeColor = Color.White;
                foreach (Control c in this.Controls)
                {
                    if (c is Label)
                        c.ForeColor = Color.White;
                    else if (c is TextBox)
                    {
                        c.BackColor = Color.FromArgb(45, 45, 45);
                        c.ForeColor = Color.White;
                    }
                    else if (c is Button)
                    {
                        c.BackColor = Color.FromArgb(60, 60, 60);
                        c.ForeColor = Color.White;
                    }
                    else if (c is CheckBox)
                    {
                        c.ForeColor = Color.White;
                    }
                }
            }
            else
            {
                this.BackColor = SystemColors.Control;
                this.ForeColor = Color.Black;
                foreach (Control c in this.Controls)
                {
                    if (c is Label)
                        c.ForeColor = Color.Black;
                    else if (c is TextBox)
                    {
                        c.BackColor = Color.White;
                        c.ForeColor = Color.Black;
                    }
                    else if (c is Button)
                    {
                        c.BackColor = SystemColors.Control;
                        c.ForeColor = Color.Black;
                    }
                    else if (c is CheckBox)
                    {
                        c.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void LoadClientData()
        {
            if (_editingClient != null)
            {
                textBoxName.Text = _editingClient.Name;
                textBoxRccs.Text = _editingClient.RccsPath;
                textBoxClient.Text = _editingClient.ClientPath;
                textBoxArgs.Text = _editingClient.LaunchArguments;
                checkBoxAutoLaunch.Checked = _editingClient.AutoLaunchServer;
                textBoxServer.Text = _editingClient.ServerPath ?? "";
                textBoxServerArgs.Text = _editingClient.ServerArguments ?? "";
                
                UpdateServerControls();
            }
        }

        private void UpdateServerControls()
        {
            bool enabled = checkBoxAutoLaunch.Checked;
            labelServer.Enabled = enabled;
            textBoxServer.Enabled = enabled;
            buttonBrowseServer.Enabled = enabled;
            labelServerArgs.Enabled = enabled;
            textBoxServerArgs.Enabled = enabled;
        }

        private void ButtonBrowseRccs_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
                dialog.Title = "Select RCCS Executable";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxRccs.Text = dialog.FileName;
                }
            }
        }

        private void ButtonBrowseClient_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
                dialog.Title = "Select Roblox Client Executable";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxClient.Text = dialog.FileName;
                }
            }
        }

        private void ButtonBrowseServer_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "Executable Files (*.exe)|*.exe|All Files (*.*)|*.*";
                dialog.Title = "Select Server Executable";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxServer.Text = dialog.FileName;
                }
            }
        }

        private void CheckBoxAutoLaunch_CheckedChanged(object sender, EventArgs e)
        {
            UpdateServerControls();
        }

        private async void ButtonTest_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxRccs.Text) ||
                string.IsNullOrWhiteSpace(textBoxClient.Text))
            {
                MessageBox.Show("Please fill in the required fields (Name, RCCS Path, Client Path).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var testClient = new CustomClientInfo
            {
                Name = textBoxName.Text,
                RccsPath = textBoxRccs.Text,
                ClientPath = textBoxClient.Text,
                LaunchArguments = textBoxArgs.Text,
                AutoLaunchServer = checkBoxAutoLaunch.Checked,
                ServerPath = checkBoxAutoLaunch.Checked ? textBoxServer.Text : null,
                ServerArguments = checkBoxAutoLaunch.Checked ? textBoxServerArgs.Text : null
            };

            labelStatus.Text = "Testing client...";
            labelStatus.ForeColor = Color.Orange;
            this.Enabled = false;

            bool success = await CustomClientManager.TestCustomClientAsync(testClient);

            this.Enabled = true;
            if (success)
            {
                labelStatus.Text = "Client test successful!";
                labelStatus.ForeColor = Color.Green;
            }
            else
            {
                labelStatus.Text = $"Client test failed: {testClient.ErrorMessage}";
                labelStatus.ForeColor = Color.Red;
            }
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxName.Text) ||
                string.IsNullOrWhiteSpace(textBoxRccs.Text) ||
                string.IsNullOrWhiteSpace(textBoxClient.Text))
            {
                MessageBox.Show("Please fill in the required fields (Name, RCCS Path, Client Path).", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_isEditMode && _editingClient != null)
            {
                _editingClient.Name = textBoxName.Text;
                _editingClient.RccsPath = textBoxRccs.Text;
                _editingClient.ClientPath = textBoxClient.Text;
                _editingClient.LaunchArguments = textBoxArgs.Text;
                _editingClient.AutoLaunchServer = checkBoxAutoLaunch.Checked;
                _editingClient.ServerPath = checkBoxAutoLaunch.Checked ? textBoxServer.Text : null;
                _editingClient.ServerArguments = checkBoxAutoLaunch.Checked ? textBoxServerArgs.Text : null;

                CustomClientManager.UpdateCustomClient(_editingClient);
            }
            else
            {
                var newClient = new CustomClientInfo
                {
                    Name = textBoxName.Text,
                    RccsPath = textBoxRccs.Text,
                    ClientPath = textBoxClient.Text,
                    LaunchArguments = textBoxArgs.Text,
                    AutoLaunchServer = checkBoxAutoLaunch.Checked,
                    ServerPath = checkBoxAutoLaunch.Checked ? textBoxServer.Text : null,
                    ServerArguments = checkBoxAutoLaunch.Checked ? textBoxServerArgs.Text : null
                };

                CustomClientManager.AddCustomClient(newClient);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Control declarations
        private Label labelName;
        private Label labelRccs;
        private Label labelClient;
        private Label labelArgs;
        private Label labelServer;
        private Label labelServerArgs;
        private TextBox textBoxName;
        private TextBox textBoxRccs;
        private TextBox textBoxClient;
        private TextBox textBoxArgs;
        private TextBox textBoxServer;
        private TextBox textBoxServerArgs;
        private Button buttonBrowseRccs;
        private Button buttonBrowseClient;
        private Button buttonBrowseServer;
        private CheckBox checkBoxAutoLaunch;
        private Button buttonTest;
        private Button buttonSave;
        private Button buttonCancel;
        private Label labelStatus;
    }
}
