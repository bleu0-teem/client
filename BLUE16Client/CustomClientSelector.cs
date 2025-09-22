using System;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using System.Linq;

namespace BLUE16Client
{
    public partial class CustomClientSelector : Form
    {
        public CustomClientInfo? SelectedClient { get; private set; }

        public CustomClientSelector()
        {
            InitializeComponent();
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            ApplyTheme();
            this.Font = new Font("Ubuntu", 10F, FontStyle.Regular);
            this.Text = "Select Custom Client";
            this.StartPosition = FormStartPosition.CenterParent;
            LoadClients();
        }

        private void InitializeComponent()
        {
            this.listBoxClients = new ListBox();
            this.buttonAdd = new Button();
            this.buttonEdit = new Button();
            this.buttonRemove = new Button();
            this.buttonLaunch = new Button();
            this.buttonTest = new Button();
            this.buttonOK = new Button();
            this.buttonCancel = new Button();
            this.labelStatus = new Label();

            // listBoxClients
            this.listBoxClients.Location = new Point(12, 12);
            this.listBoxClients.Size = new Size(350, 200);
            this.listBoxClients.DrawMode = DrawMode.OwnerDrawFixed;
            this.listBoxClients.ItemHeight = 24;
            this.listBoxClients.DrawItem += ListBoxClients_DrawItem;
            this.listBoxClients.SelectedIndexChanged += ListBoxClients_SelectedIndexChanged;
            this.listBoxClients.DoubleClick += ListBoxClients_DoubleClick;
            this.listBoxClients.KeyDown += ListBoxClients_KeyDown;

            // buttonAdd
            this.buttonAdd.Location = new Point(12, 220);
            this.buttonAdd.Size = new Size(70, 30);
            this.buttonAdd.Text = "Add";
            this.buttonAdd.Click += ButtonAdd_Click;

            // buttonEdit
            this.buttonEdit.Location = new Point(88, 220);
            this.buttonEdit.Size = new Size(70, 30);
            this.buttonEdit.Text = "Edit";
            this.buttonEdit.Click += ButtonEdit_Click;

            // buttonRemove
            this.buttonRemove.Location = new Point(164, 220);
            this.buttonRemove.Size = new Size(70, 30);
            this.buttonRemove.Text = "Remove";
            this.buttonRemove.Click += ButtonRemove_Click;

            // buttonTest
            this.buttonTest.Location = new Point(240, 220);
            this.buttonTest.Size = new Size(70, 30);
            this.buttonTest.Text = "Test";
            this.buttonTest.Click += ButtonTest_Click;

            // buttonLaunch
            this.buttonLaunch.Location = new Point(12, 260);
            this.buttonLaunch.Size = new Size(100, 30);
            this.buttonLaunch.Text = "Launch";
            this.buttonLaunch.Click += ButtonLaunch_Click;

            // buttonOK
            this.buttonOK.Location = new Point(200, 260);
            this.buttonOK.Size = new Size(80, 30);
            this.buttonOK.Text = "OK";
            this.buttonOK.Click += ButtonOK_Click;

            // buttonCancel
            this.buttonCancel.Location = new Point(286, 260);
            this.buttonCancel.Size = new Size(80, 30);
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.Click += ButtonCancel_Click;

            // labelStatus
            this.labelStatus.AutoSize = true;
            this.labelStatus.Location = new Point(12, 300);
            this.labelStatus.Size = new Size(350, 20);
            this.labelStatus.Text = "Ready";

            // CustomClientSelector
            this.ClientSize = new Size(378, 330);
            this.Controls.AddRange(new Control[] {
                this.listBoxClients,
                this.buttonAdd, this.buttonEdit, this.buttonRemove, this.buttonTest,
                this.buttonLaunch, this.buttonOK, this.buttonCancel,
                this.labelStatus
            });
        }

        private void ApplyTheme()
        {
            if (SettingsStore.DarkMode)
            {
                this.BackColor = Color.FromArgb(32, 32, 32);
                this.ForeColor = Color.White;
                listBoxClients.BackColor = Color.FromArgb(45, 45, 45);
                listBoxClients.ForeColor = Color.White;
                foreach (Control c in this.Controls)
                {
                    if (c is Label)
                        c.ForeColor = Color.White;
                    else if (c is Button)
                    {
                        c.BackColor = Color.FromArgb(60, 60, 60);
                        c.ForeColor = Color.White;
                    }
                }
            }
            else
            {
                this.BackColor = SystemColors.Control;
                this.ForeColor = Color.Black;
                listBoxClients.BackColor = Color.White;
                listBoxClients.ForeColor = Color.Black;
                foreach (Control c in this.Controls)
                {
                    if (c is Label)
                        c.ForeColor = Color.Black;
                    else if (c is Button)
                    {
                        c.BackColor = SystemColors.Control;
                        c.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void LoadClients()
        {
            listBoxClients.Items.Clear();
            foreach (var client in CustomClientManager.CustomClients)
            {
                listBoxClients.Items.Add(client);
            }
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            bool hasSelection = listBoxClients.SelectedItem != null;
            buttonEdit.Enabled = hasSelection;
            buttonRemove.Enabled = hasSelection;
            buttonTest.Enabled = hasSelection;
            buttonLaunch.Enabled = hasSelection;
            buttonOK.Enabled = hasSelection;
        }

        private void ListBoxClients_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            e.DrawBackground();
            var client = listBoxClients.Items[e.Index] as CustomClientInfo;
            if (client != null)
            {
                var font = new Font("Ubuntu", 10F, FontStyle.Regular);
                var bounds = e.Bounds;
                var g = e.Graphics;

                // Draw client name
                Brush nameBrush = SettingsStore.DarkMode ? Brushes.White : Brushes.Black;
                g.DrawString(client.Name, font, nameBrush, bounds.Left + 5, bounds.Top + 3);

                // Draw status with color
                Color statusColor = client.IsSupported ? Color.Green : Color.Red;
                string statusText = client.IsSupported ? "Supported" : "Unsupported";
                using (Brush statusBrush = new SolidBrush(statusColor))
                {
                    g.DrawString(statusText, font, statusBrush, bounds.Right - 80, bounds.Top + 3);
                }

                // Draw error message if unsupported
                if (!client.IsSupported && !string.IsNullOrEmpty(client.ErrorMessage))
                {
                    using (Brush errorBrush = new SolidBrush(Color.OrangeRed))
                    {
                        var errorFont = new Font("Ubuntu", 8F, FontStyle.Italic);
                        g.DrawString(client.ErrorMessage, errorFont, errorBrush, bounds.Left + 5, bounds.Top + 16);
                    }
                }
            }
            e.DrawFocusRectangle();
        }

        private void ListBoxClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateButtonStates();
            var client = listBoxClients.SelectedItem as CustomClientInfo;
            if (client != null)
            {
                if (client.IsSupported)
                {
                    labelStatus.Text = $"Selected: {client.Name} (Supported)";
                    labelStatus.ForeColor = Color.Green;
                }
                else
                {
                    labelStatus.Text = $"Selected: {client.Name} (Unsupported - {client.ErrorMessage})";
                    labelStatus.ForeColor = Color.Red;
                }
            }
            else
            {
                labelStatus.Text = "Ready";
                labelStatus.ForeColor = SettingsStore.DarkMode ? Color.White : Color.Black;
            }
        }

        private void ListBoxClients_DoubleClick(object sender, EventArgs e)
        {
            if (listBoxClients.SelectedItem != null)
            {
                ButtonOK_Click(sender, e);
            }
        }

        private void ListBoxClients_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && listBoxClients.SelectedItem != null)
            {
                ButtonOK_Click(sender, e);
                e.Handled = true;
            }
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            using (var form = new CustomClientForm())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    LoadClients();
                }
            }
        }

        private void ButtonEdit_Click(object sender, EventArgs e)
        {
            if (listBoxClients.SelectedItem is CustomClientInfo client)
            {
                using (var form = new CustomClientForm(client))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadClients();
                    }
                }
            }
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            if (listBoxClients.SelectedItem is CustomClientInfo client)
            {
                var result = MessageBox.Show($"Are you sure you want to remove '{client.Name}'?", "Confirm Removal", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    CustomClientManager.RemoveCustomClient(client);
                    LoadClients();
                }
            }
        }

        private async void ButtonTest_Click(object sender, EventArgs e)
        {
            if (listBoxClients.SelectedItem is CustomClientInfo client)
            {
                labelStatus.Text = "Testing client...";
                labelStatus.ForeColor = Color.Orange;
                this.Enabled = false;

                bool success = await CustomClientManager.TestCustomClientAsync(client);

                this.Enabled = true;
                LoadClients(); // Refresh to show updated status

                if (success)
                {
                    labelStatus.Text = $"Client '{client.Name}' test successful!";
                    labelStatus.ForeColor = Color.Green;
                }
                else
                {
                    labelStatus.Text = $"Client '{client.Name}' test failed: {client.ErrorMessage}";
                    labelStatus.ForeColor = Color.Red;
                }
            }
        }

        private async void ButtonLaunch_Click(object sender, EventArgs e)
        {
            if (listBoxClients.SelectedItem is CustomClientInfo client)
            {
                labelStatus.Text = "Launching client...";
                labelStatus.ForeColor = Color.Orange;
                this.Enabled = false;

                bool success = await CustomClientManager.LaunchCustomClientAsync(client);

                this.Enabled = true;
                if (success)
                {
                    labelStatus.Text = $"Client '{client.Name}' launched successfully!";
                    labelStatus.ForeColor = Color.Green;
                }
                else
                {
                    labelStatus.Text = $"Failed to launch client '{client.Name}'";
                    labelStatus.ForeColor = Color.Red;
                }
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            if (listBoxClients.SelectedItem is CustomClientInfo client)
            {
                if (!client.IsSupported)
                {
                    MessageBox.Show($"Cannot select unsupported client: {client.ErrorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                SelectedClient = client;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Control declarations
        private ListBox listBoxClients;
        private Button buttonAdd;
        private Button buttonEdit;
        private Button buttonRemove;
        private Button buttonTest;
        private Button buttonLaunch;
        private Button buttonOK;
        private Button buttonCancel;
        private Label labelStatus;
    }
}
