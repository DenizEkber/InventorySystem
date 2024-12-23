using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DTO.Models;

namespace InventorySystem.UI
{
    public partial class AddWareHouseInfo : UserControl
    {
        private readonly IWarehouseService _warehouseService;
        private UserDto _currentUser;


        public Label lblWarehouseName, lblWarehouseContactInfo, lblWarehouseAddress, lblWarehouseColumn, lblWarehouseRow, lblWarehouseDeep, lblWarehouseRefrigerated;
        public TextBox txtWareHouseName, txtWarehouseContactInfo, txtWarehouseAddress, txtWarehouseColumn, txtWarehouseRow, txtWarehouseDeep;
        public RadioButton rbtnWarehouseIsRefrigerated;

        private Button btnSaveWarehouseInfo;
        public AddWareHouseInfo(IWarehouseService warehouseService, UserDto currentUser)
        {
            _warehouseService = warehouseService;
            _currentUser = currentUser;
            Component();
        }
        private void Component()
        {
            lblWarehouseName = new Label { Text = "Warehouse Name:", Location = new System.Drawing.Point(20, 50) };
            txtWareHouseName = new TextBox { Location = new System.Drawing.Point(150, 50), Width = 200 };

            lblWarehouseContactInfo = new Label { Text = "Contact Info:", Location = new System.Drawing.Point(20, 90) };
            txtWarehouseContactInfo = new TextBox { Location = new System.Drawing.Point(150, 90), Width = 200 };

            lblWarehouseAddress = new Label { Text = "Address:", Location = new System.Drawing.Point(20, 130) };
            txtWarehouseAddress = new TextBox { Location = new System.Drawing.Point(150, 130), Width = 200 };

            lblWarehouseRefrigerated = new Label { Text = "Is Refrigerated:", Location = new System.Drawing.Point(20, 170) };
            rbtnWarehouseIsRefrigerated = new RadioButton
            {
                Location = new System.Drawing.Point(150, 170),
                Width = 100,
                Text = "Yes",
                Checked = false // Default to not refrigerated
            };

            btnSaveWarehouseInfo = new Button { Text = "Save Warehouse Info", Location = new System.Drawing.Point(150, 210) };
            btnSaveWarehouseInfo.Click += SaveWarehouseInfo_Click;

            this.Controls.Add(lblWarehouseName);
            this.Controls.Add(txtWareHouseName);
            this.Controls.Add(lblWarehouseContactInfo);
            this.Controls.Add(txtWarehouseContactInfo);
            this.Controls.Add(lblWarehouseAddress);
            this.Controls.Add(txtWarehouseAddress);
            this.Controls.Add(lblWarehouseRefrigerated);
            this.Controls.Add(rbtnWarehouseIsRefrigerated);
            this.Controls.Add(btnSaveWarehouseInfo);
        }
        public event EventHandler InfoSaved;

        private void SaveWarehouseInfo_Click(object sender, EventArgs e)
        {
            var warehouseDto = new WarehouseDto
            {
                WarehouseName = txtWareHouseName.Text,
                IsRefrigerated = rbtnWarehouseIsRefrigerated.Checked,
                ContactInfo = txtWarehouseContactInfo.Text,
                Address = txtWarehouseAddress.Text,
                UserID = _currentUser.UserID,
            };

            try
            {
                _warehouseService.AddAsync(warehouseDto);
                MessageBox.Show("Warehouse information saved successfully.");
                InfoSaved?.Invoke(this, EventArgs.Empty); // Olayı tetikle
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving warehouse info: {ex.Message}");
            }
        }

    }
}
