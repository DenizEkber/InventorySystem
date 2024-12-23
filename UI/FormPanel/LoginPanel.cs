using InventorySystem.CORE.Interfaces;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using InventorySystem.DTO.Models;
using InventorySystem.UI.Forms;
using Microsoft.Extensions.DependencyInjection;

namespace InventorySystem.UI.FormPanel
{
    public class LoginPanel : Panel
    {
        private readonly Form _parentForm; // Store reference to the parent form
        private readonly IUserService _userService;
        private readonly Action<Control> _showPanel;

        public LoginPanel(IUserService userService, Form parentForm, Action<Control> showPanel)
        {
            _userService = userService;
            _parentForm = parentForm;
            _showPanel = showPanel;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Location = new Point(300, 200);
            this.Size = new Size(400, 300);
            this.BorderStyle = BorderStyle.FixedSingle;

            var labelUsername = new Label { Text = "Email:", Location = new Point(30, 30), AutoSize = true };
            var textBoxUsername = new TextBox { Location = new Point(150, 30), Width = 200 };

            var labelPassword = new Label { Text = "Password:", Location = new Point(30, 70), AutoSize = true };
            var textBoxPassword = new TextBox { Location = new Point(150, 70), Width = 200, PasswordChar = '*' };

            var buttonLogin = new Button { Text = "Login", Location = new Point(150, 110), Width = 100 };
            buttonLogin.Click += async (s, e) => await LoginUserAsync(textBoxUsername.Text, textBoxPassword.Text);

            this.Controls.Add(labelUsername);
            this.Controls.Add(textBoxUsername);
            this.Controls.Add(labelPassword);
            this.Controls.Add(textBoxPassword);
            this.Controls.Add(buttonLogin);
        }

        private async Task LoginUserAsync(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Email and password cannot be empty.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            email = email.Trim();
            email = email.ToLower();
            if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid email format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var users = await _userService.GetAllAsync();
                var hashedPassword = HashPassword(password);
                var validUser = users.FirstOrDefault(u => u.Email == email && u.Password == hashedPassword);

                if (validUser != null)
                {
                    UserDto userDto = new UserDto
                    {
                        UserID = validUser.UserID,
                        Email = validUser.Email,
                        Name = validUser.Name,
                        Role = validUser.Role, // Include other necessary fields
                    };
                    MessageBox.Show("Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Create scope and get MainForm via dependency injection
                    using var scope = Program.Host.Services.CreateScope();
                    var serviceProvider = scope.ServiceProvider;

                    var mainForm = serviceProvider.GetRequiredService<MainForm>();
                    mainForm.SetUser(userDto);

                    // Hide the login form before showing the main form
                    Application.OpenForms[0].Hide(); // Hide the login form

                    mainForm.ShowDialog();
                    _parentForm.Close();
                }
                else
                {
                    MessageBox.Show("Invalid email or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private bool IsValidEmail(string email)
        {
            // Simple regex for email validation
            var emailRegex = new System.Text.RegularExpressions.Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
    }
}
