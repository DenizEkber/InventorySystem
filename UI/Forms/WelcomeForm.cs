using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.UI.FormPanel;
using System.Security.Cryptography;
using System.Text;

namespace InventorySystem.UI.Forms
{
    public partial class WelcomeForm : Form
    {
        private readonly IUserService _userService;
        private LoginPanel _loginPanel;
        private RegisterPanel _registerPanel;
        private Label labelWelcome;

        public WelcomeForm(IUserService userService)
        {
            _userService = userService;
            InitializeComponents();
            ShowPanel(_loginPanel);
        }

        private void InitializeComponents()
        {
            // Form settings
            this.Text = "Welcome";
            this.ClientSize = new Size(1000, 800);
            this.StartPosition = FormStartPosition.CenterScreen;

            // Welcome Label
            labelWelcome = new Label
            {
                Text = "Welcome!",
                Font = new Font("Arial", 45, FontStyle.Bold),
                ForeColor = Color.Black,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(labelWelcome);

            // Login Button
            var btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(400, 100),
                Size = new Size(175, 60)
            };
            btnLogin.Click += (s, e) => ShowPanel(_loginPanel);

            // Register Button
            var btnRegister = new Button
            {
                Text = "Register",
                Location = new Point(575, 100),
                Size = new Size(175, 60)
            };
            btnRegister.Click += (s, e) => ShowPanel(_registerPanel);

            // Initialize panels
            _loginPanel = new LoginPanel(_userService, this, ShowPanel);
            _registerPanel = new RegisterPanel(_userService, ShowPanel);

            // Add Controls to Form
            this.Controls.Add(btnLogin);
            this.Controls.Add(btnRegister);
            this.Controls.Add(_loginPanel);
            this.Controls.Add(_registerPanel);

            // Default panel
            ShowPanel(_loginPanel);
        }

        private void ShowPanel(Control panel)
        {
            _loginPanel.Visible = false;
            _registerPanel.Visible = false;
            panel.Visible = true;
        }
    }
}
