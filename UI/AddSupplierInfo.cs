using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DTO.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorySystem.UI
{
    public partial class AddSupplierInfo : UserControl
    {
        private readonly ISupplierService _supplierService;
        private UserDto _currentUser;


        public Label lblSupplierContactInfo, lblSupplierAddress;
        public TextBox txtSupplierContactInfo, txtSupplierAddress;

        private Button btnSaveSupplierInfo;

        public AddSupplierInfo(ISupplierService supplierService, UserDto currentUser)
        {
            _supplierService = supplierService;
            _currentUser = currentUser;
            Component();
        }
        private void Component()
        {
            lblSupplierContactInfo = new Label { Text = "Supplier Contact Info:", Location = new System.Drawing.Point(20, 50) };
            txtSupplierContactInfo = new TextBox { Location = new System.Drawing.Point(200, 50), Width = 200 };

            lblSupplierAddress = new Label { Text = "Supplier Address:", Location = new System.Drawing.Point(20, 90) };
            txtSupplierAddress = new TextBox { Location = new System.Drawing.Point(200, 90), Width = 200 };

            btnSaveSupplierInfo = new Button { Text = "Save Supplier Info", Location = new System.Drawing.Point(200, 130) };
            btnSaveSupplierInfo.Click += SaveSupplierInfo_Click;

            this.Controls.Add(lblSupplierContactInfo);
            this.Controls.Add(txtSupplierContactInfo);
            this.Controls.Add(lblSupplierAddress);
            this.Controls.Add(txtSupplierAddress);
            this.Controls.Add(btnSaveSupplierInfo);
        }
        public event EventHandler InfoSaved;

        private void SaveSupplierInfo_Click(object sender, EventArgs e)
        {
            var supplierDto = new SupplierDto
            {
                Name = _currentUser.Name,
                ContactInfo = txtSupplierContactInfo.Text,
                SupplierAddress = txtSupplierAddress.Text,
                UserID = _currentUser.UserID,
            };

            try
            {
                _supplierService.AddAsync(supplierDto);
                MessageBox.Show("Supplier information saved successfully.");
                InfoSaved?.Invoke(this, EventArgs.Empty); // Olayı tetikle
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving supplier info: {ex.Message}");
            }
        }

    }
}
