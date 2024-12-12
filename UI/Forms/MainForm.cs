using InventorySystem.CORE.Interfaces;
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
    public partial class MainForm : Form
    {

        private System.Windows.Forms.Button btnProducts;
        private System.Windows.Forms.Button btnSuppliers;
        private System.Windows.Forms.Button btnStockTransactions;

        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;
        private readonly IStockTransactionService _stockTransactionService;

        public MainForm(IProductService productService,
            ISupplierService supplierService,
            IStockTransactionService stockTransactionService)
        {

            _productService = productService;
            _supplierService = supplierService;
            _stockTransactionService = stockTransactionService;

            Component();
        }

        private void Component()
        {
            this.btnProducts = new System.Windows.Forms.Button();
            this.btnSuppliers = new System.Windows.Forms.Button();
            this.btnStockTransactions = new System.Windows.Forms.Button();

            // btnProducts
            this.btnProducts.Location = new System.Drawing.Point(50, 50);
            this.btnProducts.Size = new System.Drawing.Size(200, 50);
            this.btnProducts.Text = "Manage Products";
            this.btnProducts.Click += new System.EventHandler(this.btnProducts_Click);

            // btnSuppliers
            this.btnSuppliers.Location = new System.Drawing.Point(50, 120);
            this.btnSuppliers.Size = new System.Drawing.Size(200, 50);
            this.btnSuppliers.Text = "Manage Suppliers";
            this.btnSuppliers.Click += new System.EventHandler(this.btnSuppliers_Click);

            // btnStockTransactions
            this.btnStockTransactions.Location = new System.Drawing.Point(50, 190);
            this.btnStockTransactions.Size = new System.Drawing.Size(200, 50);
            this.btnStockTransactions.Text = "Manage Stock Transactions";
            this.btnStockTransactions.Click += new System.EventHandler(this.btnStockTransactions_Click);

            // MainForm
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.btnProducts);
            this.Controls.Add(this.btnSuppliers);
            this.Controls.Add(this.btnStockTransactions);
            this.Text = "Inventory Management";
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            var productForm = new ProductForm(_productService);
            productForm.Show();
        }

        private void btnSuppliers_Click(object sender, EventArgs e)
        {
            var supplierForm = new SupplierForm(_supplierService);
            supplierForm.Show();
        }

        private void btnStockTransactions_Click(object sender, EventArgs e)
        {
            var stockTransactionForm = new StockTransactionForm(_stockTransactionService, _productService);
            stockTransactionForm.Show();
        }
    }
}
