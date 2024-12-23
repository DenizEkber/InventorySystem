using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Threading.Tasks;
using InventorySystem.Helpers.Email;
using InventorySystem.DTO.Models;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace InventorySystem.UI.FormPanel
{
    public class RegisterPanel : Panel
    {
        private readonly IUserService _userService;
        private readonly Action<Control> _showPanel;
        private Panel _verificationPanel;

        public RegisterPanel(IUserService userService, Action<Control> showPanel)
        {
            _userService = userService;
            _showPanel = showPanel;
            InitializeComponents();
        }

        private void InitializeComponents()
        {
            this.Location = new Point(300, 200);
            this.Size = new Size(400, 400);
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Visible = false;

            var labelName = new Label { Text = "Name:", Location = new Point(30, 30), AutoSize = true };
            var textBoxName = new TextBox { Location = new Point(150, 30), Width = 200 };

            var labelEmail = new Label { Text = "Email:", Location = new Point(30, 70), AutoSize = true };
            var textBoxEmail = new TextBox { Location = new Point(150, 70), Width = 200 };

            var labelPassword = new Label { Text = "Password:", Location = new Point(30, 110), AutoSize = true };
            var textBoxPassword = new TextBox { Location = new Point(150, 110), Width = 200, PasswordChar = '*' };

            var labelConfirmPassword = new Label { Text = "Confirm Password:", Location = new Point(30, 150), AutoSize = true };
            var textBoxConfirmPassword = new TextBox { Location = new Point(150, 150), Width = 200, PasswordChar = '*' };

            var labelRole = new Label { Text = "Role:", Location = new Point(30, 190), AutoSize = true };
            var comboBoxRole = new ComboBox
            {
                Location = new Point(150, 190),
                Width = 200,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            comboBoxRole.Items.AddRange(new[] { "Supplier", "Customer" });

            var buttonRegister = new Button { Text = "Register", Location = new Point(150, 230), Width = 100 };
            buttonRegister.Click += async (s, e) => await RegisterUserAsync(textBoxName.Text, textBoxEmail.Text, textBoxPassword.Text, textBoxConfirmPassword.Text, comboBoxRole.SelectedItem?.ToString() == "Customer" ? "Warehouse" : "Supplier");

            this.Controls.Add(labelName);
            this.Controls.Add(textBoxName);
            this.Controls.Add(labelEmail);
            this.Controls.Add(textBoxEmail);
            this.Controls.Add(labelPassword);
            this.Controls.Add(textBoxPassword);
            this.Controls.Add(labelConfirmPassword);
            this.Controls.Add(textBoxConfirmPassword);
            this.Controls.Add(labelRole);
            this.Controls.Add(comboBoxRole);
            this.Controls.Add(buttonRegister);
        }

        private async Task RegisterUserAsync(string name, string email, string password, string confirmPassword, string role)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword) ||
                string.IsNullOrWhiteSpace(role))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            email = email.Trim();
            email = email.ToLower(); 

            if (!IsValidEmail(email))
            {
                MessageBox.Show("Invalid email format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            password = password.Trim();
            if (password.Length < 8)
            {
                MessageBox.Show("Password must be at least 8 characters long.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var users = await _userService.GetAllAsync();
                if (users.Any(u => u.Email == email))
                {
                    MessageBox.Show("Email is already registered.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var verificationCode = GenerateVerificationCode();
                MailSender.SendEmailAsync(email, "Your Verification code", $"{verificationCode}");
                MessageBox.Show($"Verification code sent to {email}. ${verificationCode}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ShowVerificationPanel(verificationCode, name, email, password, role);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ShowVerificationPanel(string verificationCode, string name, string email, string password, string role)
        {
            if (_verificationPanel == null)
            {
                _verificationPanel = new Panel
                {
                    Location = new Point(0, 0),
                    Size = this.Size,
                    BorderStyle = BorderStyle.FixedSingle,
                    Visible = false
                };

                var labelCode = new Label { Text = "Enter Verification Code:", Location = new Point(30, 30), AutoSize = true };
                var textBoxCode = new TextBox { Location = new Point(30, 60), Width = 200 };

                var buttonVerify = new Button { Text = "Verify", Location = new Point(30, 100), Width = 100 };
                buttonVerify.Click += async (s, e) => await VerifyCodeAsync(textBoxCode.Text, verificationCode, name, email, password, role);

                _verificationPanel.Controls.Add(labelCode);
                _verificationPanel.Controls.Add(textBoxCode);
                _verificationPanel.Controls.Add(buttonVerify);
                this.Controls.Add(_verificationPanel);
            }

            // Show only the verification panel
            foreach (Control control in this.Controls)
            {
                control.Visible = control == _verificationPanel;
            }
        }

        private async Task VerifyCodeAsync(string inputCode, string correctCode, string name, string email, string password, string role)
        {
            if (inputCode != correctCode)
            {
                MessageBox.Show("Invalid verification code.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                var hashedPassword = HashPassword(password);
                var newUser = new UserDto
                {
                    Name = name,
                    Email = email,
                    Password = hashedPassword,
                    Role = Enum.Parse<Role>(role)
                };

                await _userService.AddAsync(newUser);
                MessageBox.Show("Registration successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Optionally, navigate to the login screen
                // _showPanel(new LoginPanel(_userService, _showPanel));

                ResetRegisterPanel();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetRegisterPanel()
        {
            foreach (Control control in this.Controls)
            {
                control.Visible = control != _verificationPanel;
            }
            _verificationPanel.Visible = false;
        }

        private string GenerateVerificationCode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
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
            // Regex for simple email format validation
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }
    }
}
