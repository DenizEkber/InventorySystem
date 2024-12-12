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
    public partial class ProductForm : Form
    {
        private readonly IProductService _productService;

        private System.Windows.Forms.DataGridView dgvProducts;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.TextBox txtSKU;
        private System.Windows.Forms.TextBox txtPrice;
        private System.Windows.Forms.TextBox txtStockLevel;
        private System.Windows.Forms.TextBox txtMinimumStock;
        private System.Windows.Forms.TextBox txtLocation;
        private System.Windows.Forms.Button btnAddProduct;
        private System.Windows.Forms.Button btnDeleteProduct;
        public ProductForm(IProductService productService)
        {
            _productService = productService;
            Component();
        }


        private void Component()
        {
            this.dgvProducts = new System.Windows.Forms.DataGridView();
            this.txtProductName = new System.Windows.Forms.TextBox();
            this.txtSKU = new System.Windows.Forms.TextBox();
            this.txtPrice = new System.Windows.Forms.TextBox();
            this.txtStockLevel = new System.Windows.Forms.TextBox();
            this.txtMinimumStock = new System.Windows.Forms.TextBox();
            this.txtLocation = new System.Windows.Forms.TextBox();
            this.btnAddProduct = new System.Windows.Forms.Button();
            this.btnDeleteProduct = new System.Windows.Forms.Button();

            // dgvProducts
            this.dgvProducts.Location = new System.Drawing.Point(20, 20);
            this.dgvProducts.Size = new System.Drawing.Size(600, 200);

            // txtProductName
            this.txtProductName.Location = new System.Drawing.Point(20, 240);
            this.txtProductName.Size = new System.Drawing.Size(150, 20);
            this.txtProductName.PlaceholderText = "Product Name";

            // txtSKU
            this.txtSKU.Location = new System.Drawing.Point(180, 240);
            this.txtSKU.Size = new System.Drawing.Size(150, 20);
            this.txtSKU.PlaceholderText = "SKU";

            // txtPrice
            this.txtPrice.Location = new System.Drawing.Point(340, 240);
            this.txtPrice.Size = new System.Drawing.Size(100, 20);
            this.txtPrice.PlaceholderText = "Price";

            // txtStockLevel
            this.txtStockLevel.Location = new System.Drawing.Point(20, 270);
            this.txtStockLevel.Size = new System.Drawing.Size(100, 20);
            this.txtStockLevel.PlaceholderText = "Stock Level";

            // txtMinimumStock
            this.txtMinimumStock.Location = new System.Drawing.Point(130, 270);
            this.txtMinimumStock.Size = new System.Drawing.Size(100, 20);
            this.txtMinimumStock.PlaceholderText = "Minimum Stock";

            // txtLocation
            this.txtLocation.Location = new System.Drawing.Point(240, 270);
            this.txtLocation.Size = new System.Drawing.Size(200, 20);
            this.txtLocation.PlaceholderText = "Location";

            // btnAddProduct
            this.btnAddProduct.Location = new System.Drawing.Point(460, 240);
            this.btnAddProduct.Size = new System.Drawing.Size(150, 30);
            this.btnAddProduct.Text = "Add Product";
            this.btnAddProduct.Click += new System.EventHandler(this.btnAddProduct_Click);

            // btnDeleteProduct
            this.btnDeleteProduct.Location = new System.Drawing.Point(460, 280);
            this.btnDeleteProduct.Size = new System.Drawing.Size(150, 30);
            this.btnDeleteProduct.Text = "Delete Product";
            this.btnDeleteProduct.Click += new System.EventHandler(this.btnDeleteProduct_Click);

            // ProductForm
            this.ClientSize = new System.Drawing.Size(650, 350);
            this.Controls.Add(this.dgvProducts);
            this.Controls.Add(this.txtProductName);
            this.Controls.Add(this.txtSKU);
            this.Controls.Add(this.txtPrice);
            this.Controls.Add(this.txtStockLevel);
            this.Controls.Add(this.txtMinimumStock);
            this.Controls.Add(this.txtLocation);
            this.Controls.Add(this.btnAddProduct);
            this.Controls.Add(this.btnDeleteProduct);
            this.Text = "Manage Products";
        }

        private async void ProductForm_Load(object sender, EventArgs e)
        {
            var products = await _productService.GetAllProductsAsync();
            dgvProducts.DataSource = products;
        }

        private async void btnAddProduct_Click(object sender, EventArgs e)
        {
            var product = new Product
            {
                Name = txtProductName.Text,
                SKU = txtSKU.Text,
                Price = decimal.Parse(txtPrice.Text),
                StockLevel = int.Parse(txtStockLevel.Text),
                MinimumStock = int.Parse(txtMinimumStock.Text),
                Location = txtLocation.Text
            };

            await _productService.AddProductAsync(product);
            MessageBox.Show("Product added successfully.");
            await ReloadData();
        }

        private async void btnDeleteProduct_Click(object sender, EventArgs e)
        {
            if (dgvProducts.SelectedRows.Count > 0)
            {
                var id = (int)dgvProducts.SelectedRows[0].Cells["Id"].Value;
                await _productService.DeleteProductAsync(id);
                MessageBox.Show("Product deleted successfully.");
                await ReloadData();
            }
        }

        private async Task ReloadData()
        {
            dgvProducts.DataSource = await _productService.GetAllProductsAsync();
        }
    }
}
