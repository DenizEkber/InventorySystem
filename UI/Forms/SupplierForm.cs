using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorySystem.UI.Forms
{
    public partial class SupplierForm : Form
    {
        private readonly ISupplierService _supplierService;

        private System.Windows.Forms.DataGridView dgvSuppliers;
        private System.Windows.Forms.TextBox txtSupplierName;
        private System.Windows.Forms.TextBox txtContactInfo;
        private System.Windows.Forms.Button btnAddSupplier;
        private System.Windows.Forms.Button btnDeleteSupplier;

        public SupplierForm(ISupplierService supplierService)
        {
            _supplierService = supplierService;
            Component();
        }

        private void Component()
        {
            this.dgvSuppliers = new System.Windows.Forms.DataGridView();
            this.txtSupplierName = new System.Windows.Forms.TextBox();
            this.txtContactInfo = new System.Windows.Forms.TextBox();
            this.btnAddSupplier = new System.Windows.Forms.Button();
            this.btnDeleteSupplier = new System.Windows.Forms.Button();

            // dgvSuppliers
            this.dgvSuppliers.Location = new System.Drawing.Point(20, 20);
            this.dgvSuppliers.Size = new System.Drawing.Size(600, 200);

            // txtSupplierName
            this.txtSupplierName.Location = new System.Drawing.Point(20, 240);
            this.txtSupplierName.Size = new System.Drawing.Size(200, 20);
            this.txtSupplierName.PlaceholderText = "Supplier Name";

            // txtContactInfo
            this.txtContactInfo.Location = new System.Drawing.Point(240, 240);
            this.txtContactInfo.Size = new System.Drawing.Size(200, 20);
            this.txtContactInfo.PlaceholderText = "Contact Info";

            // btnAddSupplier
            this.btnAddSupplier.Location = new System.Drawing.Point(460, 240);
            this.btnAddSupplier.Size = new System.Drawing.Size(150, 30);
            this.btnAddSupplier.Text = "Add Supplier";
            this.btnAddSupplier.Click += new System.EventHandler(this.btnAddSupplier_Click);

            // btnDeleteSupplier
            this.btnDeleteSupplier.Location = new System.Drawing.Point(460, 280);
            this.btnDeleteSupplier.Size = new System.Drawing.Size(150, 30);
            this.btnDeleteSupplier.Text = "Delete Supplier";
            this.btnDeleteSupplier.Click += new System.EventHandler(this.btnDeleteSupplier_Click);

            // SupplierForm
            this.ClientSize = new System.Drawing.Size(650, 350);
            this.Controls.Add(this.dgvSuppliers);
            this.Controls.Add(this.txtSupplierName);
            this.Controls.Add(this.txtContactInfo);
            this.Controls.Add(this.btnAddSupplier);
            this.Controls.Add(this.btnDeleteSupplier);
            this.Text = "Manage Suppliers";
        }

        private async void SupplierForm_Load(object sender, EventArgs e)
        {
            dgvSuppliers.DataSource = await _supplierService.GetAllSuppliersAsync();
        }

        private async void btnAddSupplier_Click(object sender, EventArgs e)
        {
            var supplier = new Supplier
            {
                Name = txtSupplierName.Text,
                ContactInfo = txtContactInfo.Text
            };

            await _supplierService.AddSupplierAsync(supplier);
            MessageBox.Show("Supplier added successfully.");
            await ReloadData();
        }

        private async Task ReloadData()
        {
            dgvSuppliers.DataSource = await _supplierService.GetAllSuppliersAsync();
        }

        private async void btnDeleteSupplier_Click(object sender, EventArgs e)
        {
            if (dgvSuppliers.CurrentRow != null)
            {
                var supplierId = (int)dgvSuppliers.CurrentRow.Cells["Id"].Value;
                await _supplierService.DeleteSupplierAsync(supplierId);
                MessageBox.Show("Supplier deleted successfully.");
                await ReloadData();
            }
            else
            {
                MessageBox.Show("Please select a supplier to delete.");
            }
        }

    }
}
