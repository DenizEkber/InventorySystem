using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorySystem
{
    public class BuyProductPane : Panel
    {
        private readonly IStockService _stockService;
        private readonly ICurrentStockService _currentStockService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;

        private List<StockGridViewModel> _stockGridViewModels;
        private UserDto _currentUser;
        private List<WarehouseDto> _userWarehouses;
        private WarehouseDto _selectedWarehouse;

        private DataGridView dgvStocks;
        private ComboBox cbWarehouses;
        private Button btnRequestPurchase;
        private TextBox txtQuantity;
        private Label lblQuantity;
        private Label lblWarehouse;

        public BuyProductPane(
            IStockService stockService,
            IProductService productService,
            ICurrentStockService currentStockService,
            IWarehouseService warehouseService)
        {
            _stockService = stockService;
            _productService = productService;
            _currentStockService = currentStockService;
            _warehouseService = warehouseService;
            InitializeComponents();
        }

        public async void SetUser(UserDto user)
        {
            _currentUser = user;
            await LoadWarehousesAsync();
            await LoadStocksAsync();
        }

        private async Task LoadWarehousesAsync()
        {
            try
            {
                _userWarehouses = (await _warehouseService.GetAllAsync())
                    .Where(w => w.UserID == _currentUser.UserID)
                    .ToList();

                if (_userWarehouses == null || !_userWarehouses.Any())
                {
                    MessageBox.Show("No warehouses available for this user.");
                    return;
                }

                cbWarehouses.DataSource = _userWarehouses;
                cbWarehouses.DisplayMember = "WarehouseName";
                cbWarehouses.ValueMember = "WarehouseID";

                /* cbWarehouses.SelectedIndex = 0;
                 _selectedWarehouse = _userWarehouses[0];*/
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while loading warehouses: {ex.Message}");
            }
        }

        private async Task LoadStocksAsync()
        {
            try
            {
                var stocks = (await _currentStockService.GetAllAsync()).ToList();

                _stockGridViewModels = new List<StockGridViewModel>();

                foreach (var stock in stocks)
                {
                    var product = await _productService.GetByIdAsync(stock.ProductID);

                    if (product != null && stock.StockType == StockType.Supplier)
                    {
                        _stockGridViewModels.Add(new StockGridViewModel
                        {
                            ProductName = product.Name,
                            Description = product.ProductDescription,
                            Barcode = product.BarCode,
                            Quantity = stock.Quantity,
                            Price = stock.Price,
                            StockType = stock.StockType.ToString()
                        });
                    }
                }

                if (this.IsHandleCreated) // Formun handle'ının oluşturulup oluşturulmadığını kontrol et
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        dgvStocks.DataSource = null;
                        dgvStocks.DataSource = _stockGridViewModels;
                    }));
                }
                else
                {
                    this.HandleCreated += (s, e) =>
                    {
                        Invoke((MethodInvoker)(() =>
                        {
                            dgvStocks.DataSource = null;
                            dgvStocks.DataSource = _stockGridViewModels;
                        }));
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while loading stocks: {ex.Message}");
            }
        }

        private void InitializeComponents()
        {
            this.Size = new Size(Parent?.Width ?? 800, Parent?.Height ?? 600);
            this.Dock = DockStyle.Fill;

            var mainPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                Padding = new Padding(10),
                AutoSize = true
            };

            dgvStocks = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ReadOnly = true
            };
            dgvStocks.CellClick += DgvStocks_CellClick;

            lblWarehouse = new Label
            {
                Text = "Select Warehouse:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            cbWarehouses = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cbWarehouses.SelectedIndexChanged += CbWarehouses_SelectedIndexChanged;

            lblQuantity = new Label
            {
                Text = "Quantity:",
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            txtQuantity = new TextBox
            {
                Dock = DockStyle.Fill
            };

            btnRequestPurchase = new Button
            {
                Text = "Request Purchase",
                Dock = DockStyle.Fill
            };
            btnRequestPurchase.Click += BtnRequestPurchase_Click;

            mainPanel.Controls.Add(dgvStocks, 0, 0);
            mainPanel.SetColumnSpan(dgvStocks, 2);
            mainPanel.Controls.Add(lblWarehouse, 0, 1);
            mainPanel.Controls.Add(cbWarehouses, 1, 1);
            mainPanel.Controls.Add(lblQuantity, 0, 2);
            mainPanel.Controls.Add(txtQuantity, 1, 2);
            mainPanel.Controls.Add(btnRequestPurchase, 1, 3);

            this.Controls.Add(mainPanel);
        }

        private void CbWarehouses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbWarehouses.SelectedItem is WarehouseDto selectedWarehouse)
            {
                _selectedWarehouse = selectedWarehouse;
            }
        }

        private void DgvStocks_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var selectedRow = dgvStocks.Rows[e.RowIndex];
                var selectedStock = (StockGridViewModel)selectedRow.DataBoundItem;

                if (selectedStock != null)
                {
                    txtQuantity.Text = "1"; // Default quantity for selected stock
                }
            }
        }

        private async void BtnRequestPurchase_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvStocks.SelectedRows.Count == 0)
                {
                    MessageBox.Show("Please select a stock item to purchase.");
                    return;
                }

                if (!int.TryParse(txtQuantity.Text, out int quantity) || quantity <= 0)
                {
                    MessageBox.Show("Please enter a valid quantity.");
                    return;
                }
                if (_selectedWarehouse == null)
                {
                    MessageBox.Show("Please select a warehouse.");
                    return;
                }

                var selectedStock = (StockGridViewModel)dgvStocks.SelectedRows[0].DataBoundItem;

                var allProducts = (await _productService.GetAllAsync()).ToDictionary(p => p.Id, p => p.Name);

                var stockDto = (await _stockService.GetAllAsync())
                    .FirstOrDefault(s =>
                        allProducts.ContainsKey(s.ProductID) &&
                        allProducts[s.ProductID] == selectedStock.ProductName);

                if (stockDto == null)
                {
                    MessageBox.Show("Selected stock not found.");
                    return;
                }

                await _stockService.RequestPurchaseAsync(stockDto, _selectedWarehouse.WarehouseID, quantity);

                MessageBox.Show("Purchase request sent!");
                await LoadStocksAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
    }
}