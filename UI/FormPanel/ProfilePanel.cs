using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using InventorySystem.DTO.Models;
using InventorySystem.CORE.Interfaces;
using InventorySystem.CORE.Services;
using InventorySystem.Helpers;
using InventorySystem.DATABASE.CodeFirst.Entities;

namespace InventorySystem.UI.FormPanel
{
    public class ProfilePanel : Panel
    {
        private UserDto _currentUser;
        private IUserService _service;
        private IWarehouseService _warehouseService;
        private PictureBox _profilePicture;
        private Button _addWarehouseButton;

        public event EventHandler ProfileUpdated;

        public ProfilePanel(UserDto currentUser, IUserService service, IWarehouseService warehouseService)
        {
            _currentUser = currentUser;
            _service = service;
            _warehouseService = warehouseService;
            InitializeUI();
            
        }

        private async void InitializeUI()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(30, 30, 30);

            Label titleLabel = new Label
            {
                Text = "Profile",
                Font = new Font("Segoe UI", 24, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(20, 20)
            };
            this.Controls.Add(titleLabel);

            _profilePicture = new PictureBox
            {
                Size = new Size(120, 120),
                Location = new Point(20, 70),
                BackColor = Color.Gray,
                BorderStyle = BorderStyle.FixedSingle,
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand
            };
            await SomeEventHandler();
            _profilePicture.Click += ProfilePicture_Click;
            this.Controls.Add(_profilePicture);

            Label usernameLabel = new Label
            {
                Text = $"Username: {_currentUser.Name}",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(160, 70)
            };
            this.Controls.Add(usernameLabel);

            Label emailLabel = new Label
            {
                Text = $"Email: {_currentUser.Email}",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(160, 110)
            };
            this.Controls.Add(emailLabel);

            Label roleLabel = new Label
            {
                Text = $"Role: {_currentUser.Role}",
                Font = new Font("Segoe UI", 14),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(160, 150)
            };
            this.Controls.Add(roleLabel);

            Button updateButton = new Button
            {
                Text = "Update Profile",
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(180, 40),
                Location = new Point(20, 220)
            };
            updateButton.FlatAppearance.BorderSize = 0;
            updateButton.Click += UpdateButton_Click;
            this.Controls.Add(updateButton);
            var RoleType = _currentUser.Role == Role.Warehouse ? "Warehouse" : "Supplier";
            if (RoleType.Equals("WareHouse", StringComparison.OrdinalIgnoreCase))
            {
                _addWarehouseButton = new Button
                {
                    Text = "Add Warehouse",
                    Font = new Font("Segoe UI", 12),
                    BackColor = Color.FromArgb(50, 50, 50),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(180, 40),
                    Location = new Point(20, 280)
                };
                _addWarehouseButton.FlatAppearance.BorderSize = 0;
                _addWarehouseButton.Click += AddWarehouseButton_Click;
                this.Controls.Add(_addWarehouseButton);
            }
        }

        private async Task<Image> LoadProfileImage()
        {
            string defaultImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "images", "Poke.jpg");
            string root = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "wwwroot");
            try
            {
                var userDetail = await _service.GetByIdUserDetailAsync(_currentUser.UserID).ConfigureAwait(false);
                if(userDetail == null)
                {
                    return File.Exists(defaultImagePath) ? Image.FromFile(defaultImagePath) : null;
                }

                if (string.IsNullOrEmpty(userDetail.ProfileImageUrl) || !File.Exists(root))
                {
                    return File.Exists(defaultImagePath) ? Image.FromFile(defaultImagePath) : null;
                }
                root += userDetail.ProfileImageUrl;
                return Image.FromFile(root);
            }
            catch
            {
                return File.Exists(defaultImagePath) ? Image.FromFile(defaultImagePath) : null;
            }
        }

        private async Task SomeEventHandler()
        {
            _profilePicture.Image = await LoadProfileImage();
        }

        private async void ProfilePicture_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        string imagePath = await FileUploadHelper.UploadFileAsync(openFileDialog.FileName, "Images");
                        await _service.UpdateUserDetailAsync(_currentUser.UserID, imagePath);
                        string defaultImagePath = Path.Combine(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.FullName, "wwwroot");
                        defaultImagePath += imagePath;
                        _profilePicture.Image = Image.FromFile(defaultImagePath);
                        MessageBox.Show("Profile photo updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"An error occurred while saving the profile photo: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            UpdateProfile();
            OnProfileUpdated();
        }

        private void UpdateProfile()
        {
            MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AddWarehouseButton_Click(object sender, EventArgs e)
        {
            AddWareHouseInfo warehouseInfo = new AddWareHouseInfo(_warehouseService, _currentUser)
            {
                Dock = DockStyle.Fill
            };
            Form warehouseForm = new Form
            {
                Text = "Add Warehouse",
                ForeColor = Color.Gray,
                Size = new Size(600, 600)
            };
            warehouseForm.Controls.Add(warehouseInfo);
            warehouseForm.ShowDialog();
        }

        protected virtual void OnProfileUpdated()
        {
            ProfileUpdated?.Invoke(this, EventArgs.Empty);
        }
    }
}
