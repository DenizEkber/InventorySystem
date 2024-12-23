using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DATABASE.Repositories;
using InventorySystem.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorySystem.UI.FormPanel
{
    public class BuyProductPanel : Panel
    {
        private readonly IStockService _stockService;
        private readonly ICurrentStockService _currentStockService;
        private readonly IProductService _productService;
        private readonly IWarehouseService _warehouseService;

        private List<StockGridViewModel> _stockGridViewModels;
        private UserDto _currentUser;
        private List<WarehouseDto> _userWarehouses;
        private WarehouseDto _selectedWarehouse;

        private List<SelectedProductModel> _selectedProducts;

        private FlowLayoutPanel flpProducts; // Ürün kartlarının bulunduğu panel
        private FlowLayoutPanel flpSelectedProducts; // Seçilen ürünlerin listesi
        private Button btnAddToCart;
        private Label lblTotalPrice;
        private TextBox txtQuantity;
        private ListBox lstProducts;

        private ComboBox cbWarehouses;

        ProductRepository _productRepository;
        CurrentStockRepository _currentStockRepository;



        public BuyProductPanel(
            IStockService stockService,
            IProductService productService,
            ICurrentStockService currentStockService,
            IWarehouseService warehouseService)
        {
            _stockService = stockService;
            _productService = productService;
            _currentStockService = currentStockService;
            _warehouseService = warehouseService;
            _selectedProducts = new List<SelectedProductModel>();
            InitializeComponents();
            //_productRepository = productRepository;
            //_currentStockRepository = currentStockRepository;
        }
        public async void SetUser(UserDto user)
        {
            _currentUser = user;
            await LoadWarehousesAsync();
            await LoadProductsAsync();
        }
        public async Task LoadWarehousesAsync()
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

                cbWarehouses.SelectedIndex = -1; // Bu, ComboBox'ta hiçbir öğenin seçilmemesini sağlar
                _selectedWarehouse =  _userWarehouses[0];
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error while loading warehouses: {ex.Message}");
            }
        }

        private void CbWarehouses_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbWarehouses.SelectedItem is WarehouseDto selectedWarehouse)
            {
                _selectedWarehouse = selectedWarehouse;
            }
        }
        private void InitializeComponents()
        {
            this.Dock = DockStyle.Fill;
            this.Padding = new Padding(10);

            // Ana düzen
            var mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 3,
                AutoSize = true,
                Padding = new Padding(10),
            };
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60)); // Ürün kartları için geniş sütun
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40)); // Seçilen ürünler için dar sütun
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 70)); // Üst kısım
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 30)); // Alt kısım

            // Ürün Kartları İçin Panel
            flpProducts = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                BackColor = Color.WhiteSmoke,
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Seçilen Ürünler Paneli
            flpSelectedProducts = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                BackColor = Color.LightYellow,
                Padding = new Padding(5),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Depo Seçimi ComboBox
            cbWarehouses = new ComboBox
            {
                Dock = DockStyle.Top,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Arial", 10, FontStyle.Regular),
            };
            cbWarehouses.SelectedIndexChanged += CbWarehouses_SelectedIndexChanged;

            // Miktar Girişi
            txtQuantity = new TextBox
            {
                Dock = DockStyle.Top,
                Width = 100,
                PlaceholderText = "Quantity",
                Font = new Font("Arial", 10, FontStyle.Regular),
            };

            // Sepete Ekle Butonu
            btnAddToCart = new Button
            {
                Text = "Add to Cart",
                Dock = DockStyle.Bottom,
                Height = 50,
                Font = new Font("Arial", 14, FontStyle.Bold),
                BackColor = Color.DarkGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
            };
            btnAddToCart.Click += BtnAddToCart_Click;

            // Toplam Fiyat Etiketi
            lblTotalPrice = new Label
            {
                Text = "Total Price: $0.00",
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Padding = new Padding(5)
            };

            // Ürünler ve Depo Seçimlerini Üst Satıra Ekleyin
            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(5)
            };
            topPanel.Controls.Add(cbWarehouses);
            topPanel.Controls.Add(txtQuantity);

            /*lstProducts = new ListBox
            {
                Dock = DockStyle.Top,
                Height = 200,
                Font = new Font("Arial", 10, FontStyle.Regular),
            };
            lstProducts.SelectedIndexChanged += LstProducts_SelectedIndexChanged;*/

            // Panel veya ana düzeninize ekleyin
           // mainLayout.Controls.Add(lstProducts, 1, 0); // Sağ üst kısma ekleme


            // Ana düzenin sıralanması
            mainLayout.Controls.Add(flpProducts, 0, 0); // Sol: Ürün Kartları
            mainLayout.SetRowSpan(flpProducts, 3); // Tüm satırları kapla
            mainLayout.Controls.Add(topPanel, 1, 0); // Sağ: Depo ve Miktar
            mainLayout.Controls.Add(flpSelectedProducts, 1, 1); // Sağ: Seçilen Ürünler
            mainLayout.Controls.Add(lblTotalPrice, 1, 2); // Sağ Alt: Toplam Fiyat
            mainLayout.Controls.Add(btnAddToCart, 1, 2); // Sağ Alt: Sepete Ekle

            this.Controls.Add(mainLayout);
        }
        private void LstProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedProduct = lstProducts.SelectedItem as StockGridViewModel;

            if (selectedProduct != null)
            {
                txtQuantity.Text = selectedProduct.Quantity.ToString(); // Varsayılan miktarı göster
            }
        }


        private async Task LoadProductsAsync()
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
                            ProductID = product.Id,
                            ProductName = product.Name,
                            Description = product.ProductDescription,
                            Barcode = product.BarCode,
                            Quantity = stock.Quantity,
                            Price = stock.Price,
                            StockType = stock.StockType.ToString()
                        });

                    }
                }

                foreach (var product in _stockGridViewModels)
                {
                    flpProducts.Controls.Add(CreateProductCard(product));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ürün yüklenirken bir hata oluştu: {ex.Message}");
            }
        }


        private Panel CreateProductCard(StockGridViewModel product)
        {
            var panel = new Panel
            {
                Size = new Size(200, 300),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(5),
                BackColor = Color.LightGray
            };

            var lblName = new Label
            {
                Text = product.ProductName,
                Dock = DockStyle.Top,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 12, FontStyle.Bold),
                Height = 30
            };

            var lblPrice = new Label
            {
                Text = $"Price: ${product.Price:F2}",
                Dock = DockStyle.Bottom,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Arial", 10, FontStyle.Regular),
                Height = 20
            };

            var btnAdd = new Button
            {
                Text = "Add",
                Dock = DockStyle.Bottom,
                Height = 30
            };
            btnAdd.Click += (s, e) => AddProductToSelection(product);

            var pictureBox = new PictureBox
            {
                ImageLocation = "C:\\Users\\LENOVO\\Pictures\\gaming-profile-3jx3qzg330o2qepp.jpg", // Görsel URL'si
                Dock = DockStyle.Fill,
                SizeMode = PictureBoxSizeMode.Zoom
            };

            panel.Controls.Add(lblName);
            panel.Controls.Add(pictureBox);
            panel.Controls.Add(lblPrice);
            panel.Controls.Add(btnAdd);

            return panel;
        }

        private void AddProductToSelection(StockGridViewModel product)
        {
            if (product == null)
            {
                MessageBox.Show("Ürün bilgisi eksik.");
                return;
            }

            if (_selectedProducts.Any(p => p.ProductID == product.ProductID))
            {
                MessageBox.Show("Bu ürün zaten seçildi. Miktarı güncellemek için aşağıdaki listeden düzenleyin.");
                return;
            }

            // Ürünü listeye ekle
            var selectedProductPanel = CreateSelectedProductPanel(product);
            flpSelectedProducts.Controls.Add(selectedProductPanel);

            _selectedProducts.Add(new SelectedProductModel
            {
                ProductID = product.ProductID,
                Name = product.ProductName,
                Price = product.Price,
                Quantity = 1 // Varsayılan miktar
            });

            UpdateTotalPrice();
        }


        private Panel CreateSelectedProductPanel(StockGridViewModel product)
        {
            var panel = new Panel
            {
                Size = new Size(flpSelectedProducts.Width - 20, 50),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White
            };

            var lblName = new Label
            {
                Text = product.ProductName,
                Dock = DockStyle.Left,
                Width = 150,
                TextAlign = ContentAlignment.MiddleLeft
            };

            var txtQuantity = new TextBox
            {
                Text = "1",
                Dock = DockStyle.Left,
                Width = 50
            };
            txtQuantity.TextChanged += (s, e) =>
            {
                if (int.TryParse(txtQuantity.Text, out int quantity))
                {
                    var selected = _selectedProducts.First(p => p.ProductID == product.ProductID);
                    selected.Quantity = quantity;
                    UpdateTotalPrice();
                }
            };

            var lblPrice = new Label
            {
                Text = $"$0.00",
                Dock = DockStyle.Right,
                Width = 80,
                TextAlign = ContentAlignment.MiddleRight
            };

            panel.Controls.Add(lblName);
            panel.Controls.Add(txtQuantity);
            panel.Controls.Add(lblPrice);

            return panel;
        }

        private void UpdateTotalPrice()
        {
            var totalPrice = _selectedProducts.Sum(p => p.Price * p.Quantity);
            lblTotalPrice.Text = $"Total Price: ${totalPrice:F2}";
        }

        private async void BtnAddToCart_Click(object sender, EventArgs e)
        {
            // Seçilen ürün ve stok bilgileri
            //var selectedProduct = lstProducts.SelectedItem as StockGridViewModel;

            if (_selectedProducts.Any())
            {
                // Seçilen miktar
                /*int quantity = int.Parse(txtQuantity.Text);

                if (quantity > selectedProduct.Quantity)
                {
                    MessageBox.Show("Yeterli stok yok.");
                    return;
                }

                // Sepete ekleme işlemi
                var cartItem = new CartItemDto
                {
                    ProductId = selectedProduct.ProductId,
                    Name = selectedProduct.Name,
                    Price = selectedProduct.Price,
                    Quantity = quantity,
                    TotalPrice = selectedProduct.Price * quantity
                };

                ShoppingCart.Add(cartItem);

                MessageBox.Show($"{selectedProduct.Name} sepete eklendi.");*/
                foreach (var product in _selectedProducts)
                {
                    if (product.Quantity <= 0)
                    {
                        MessageBox.Show($"Please enter a valid quantity for {product.Name}.");
                        return;
                    }

                    var allProducts = (await _productService.GetAllAsync()).ToDictionary(p => p.Id, p => p.Name);

                    var stockDto = (await _stockService.GetAllAsync())
                        .FirstOrDefault(s =>
                            allProducts.ContainsKey(s.ProductID) &&
                            allProducts[s.ProductID] == product.Name);

                    if (stockDto == null)
                    {
                        MessageBox.Show($"Stock not found for product: {product.Name}");
                        return;
                    }

                    await _stockService.RequestPurchaseAsync(stockDto, _selectedWarehouse.WarehouseID, product.Quantity);
                    await _stockService.UpdateCurrentStockAsync();
                }

                MessageBox.Show("All selected products have been sent!");
                _selectedProducts.Clear();
                flpSelectedProducts.Controls.Clear();
                UpdateTotalPrice();
                /*var allProducts = (await _productService.GetAllAsync()).ToDictionary(p => p.Id, p => p.Name);

                var stockDto = (await _stockService.GetAllAsync())
                    .FirstOrDefault(s =>
                        allProducts.ContainsKey(s.ProductID) &&
                        allProducts[s.ProductID] == selectedProduct.ProductName);

                if (stockDto == null)
                {
                    MessageBox.Show("Selected stock not found.");
                    return;
                }
                int quantity = int.Parse(txtQuantity.Text);

                if (quantity > selectedProduct.Quantity)
                {
                    MessageBox.Show("Yeterli stok yok.");
                    return;
                }

                await _stockService.RequestPurchaseAsync(stockDto, _selectedWarehouse.WarehouseID, quantity);

                MessageBox.Show("Purchase request sent!");
                await LoadProductsAsync();*/
            }
            else
            {
                MessageBox.Show("Lütfen bir ürün seçin.");
            }
        }



    }

    // Ürün kartlarını temsil eden model
    public class ProductCardModel
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; }
    }

    // Seçilen ürünleri temsil eden model
    public class SelectedProductModel
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
    public class CartItemDto
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }

    public static class ShoppingCart
    {
        public static List<CartItemDto> Items { get; } = new List<CartItemDto>();

        public static void Add(CartItemDto item)
        {
            Items.Add(item);
        }
    }


}
