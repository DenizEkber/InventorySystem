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
    public partial class StockTransactionForm : Form
    {
        private readonly IStockTransactionService _stockTransactionService;
        private readonly IProductService _productService;

        private System.Windows.Forms.ComboBox cbProducts;
        private System.Windows.Forms.ComboBox cbTransactionType;
        private System.Windows.Forms.TextBox txtQuantity;
        private System.Windows.Forms.Button btnAddTransaction;

        public StockTransactionForm(IStockTransactionService stockTransactionService, IProductService productService)
        {
            _stockTransactionService = stockTransactionService;
            _productService = productService;
            InitializeComponent();
        }

        private async void StockTransactionForm_Load(object sender, EventArgs e)
        {
            cbProducts.DataSource = await _productService.GetAllProductsAsync();
            cbProducts.DisplayMember = "Name";
            cbProducts.ValueMember = "Id";
        }

        private async void btnAddTransaction_Click(object sender, EventArgs e)
        {
            var transaction = new StockTransaction
            {
                ProductId = (int)cbProducts.SelectedValue,
                Quantity = int.Parse(txtQuantity.Text),
                TransactionDate = DateTime.Now,
                TransactionType = cbTransactionType.SelectedItem.ToString()
            };

            await _stockTransactionService.AddStockTransactionAsync(transaction);
            MessageBox.Show("Transaction added successfully.");
        }
    }
}
