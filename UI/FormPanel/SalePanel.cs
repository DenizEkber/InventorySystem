using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DTO.Models;
using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorySystem.UI.FormPanel
{
    public class SalePanel : Panel
    {
        private readonly IStockService _stockService;
        private readonly ICurrentStockService _currentStockService;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IStockTransactionService _stockTransactionService;
        private readonly ISupplierService _supplierService;

        private UserDto _currentUser;
        private SupplierDto _supplierDto;

        private TextBox txtProductName;
        private TextBox txtBarCode;
        private TextBox txtDescription;
        private TextBox txtPackedWeight;
        private TextBox txtPackedWidth;
        private TextBox txtPackedHeight;
        private TextBox txtPackedDepth;
        private CheckBox chkRefrigerated;
        private TextBox txtQuantity;
        private TextBox txtPrice;
        private Button btnSave;
        private ComboBox cmbCategory;

        public SalePanel(IStockService stockService, IProductService productService,
                         ICategoryService categoryService, IStockTransactionService stockTransactionService,
                         ISupplierService supplierService, ICurrentStockService currentStockService)
        {
            _stockService = stockService;
            _productService = productService;
            _categoryService = categoryService;
            _stockTransactionService = stockTransactionService;
            _supplierService = supplierService;
            _currentStockService = currentStockService;

            InitializeComponents();
            this.ParentChanged += AddProductToStockPanel_ParentChanged;
            
        }

        private async void AddProductToStockPanel_ParentChanged(object sender, EventArgs e)
        {
            if (this.Parent != null)
            {
                await LoadDataAsync();
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();
                cmbCategory.DataSource = categories;
                cmbCategory.DisplayMember = "Name";
                cmbCategory.ValueMember = "Id";

                if (_supplierDto == null)
                {
                    MessageBox.Show("Supplier information is missing.");
                    this.Parent.Controls.Remove(this);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories: {ex.Message}");
            }
        }

        public async Task SetSupplier(UserDto currentUser)
        {
            _currentUser = currentUser;
            var suppliers = await _supplierService.GetAllAsync();
            _supplierDto = suppliers.FirstOrDefault(w => w.UserID == _currentUser.UserID);
        }


        private void InitializeComponents()
        {
            int verticalSpacing = 50;
            int yOffset = 20;

            this.AutoScroll = true;
            this.BackColor = Color.White; // Light background for a modern look.
            this.Size = new Size(400, 650);

            // Product Name Input
            txtProductName = CreateTextBox("Product Name", ref yOffset);

            // Bar Code Input
            txtBarCode = CreateTextBox("Bar Code", ref yOffset);

            // Description Input
            txtDescription = CreateTextBox("Description", ref yOffset);

            // Packed Dimensions
            txtPackedWeight = CreateTextBox("Packed Weight (kg)", ref yOffset);
            txtPackedWidth = CreateTextBox("Packed Width (cm)", ref yOffset);
            txtPackedHeight = CreateTextBox("Packed Height (cm)", ref yOffset);
            txtPackedDepth = CreateTextBox("Packed Depth (cm)", ref yOffset);

            // Refrigerated Checkbox
            chkRefrigerated = new CheckBox
            {
                Location = new Point(20, yOffset),
                Text = "Refrigerated",
                Size = new Size(350, 30),
                Font = new Font("Segoe UI", 10),
                BackColor = Color.Transparent
            };
            this.Controls.Add(chkRefrigerated);
            yOffset += verticalSpacing;

            // Category Dropdown
            cmbCategory = new ComboBox
            {
                Location = new Point(20, yOffset),
                Size = new Size(350, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(new Label
            {
                Text = "Category",
                Location = new Point(20, yOffset - 30),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            });
            this.Controls.Add(cmbCategory);
            yOffset += verticalSpacing;

            // Quantity Input
            txtQuantity = CreateTextBox("Quantity", ref yOffset);

            // Price Input
            txtPrice = CreateTextBox("Price", ref yOffset);

            // Save Button
            btnSave = new Button
            {
                Location = new Point(20, yOffset),
                Size = new Size(350, 40),
                Text = "Add to Stock",
                BackColor = Color.FromArgb(0, 123, 255), // Modern blue button.
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.MouseEnter += (s, e) => btnSave.BackColor = Color.FromArgb(0, 105, 217);
            btnSave.MouseLeave += (s, e) => btnSave.BackColor = Color.FromArgb(0, 123, 255);
            btnSave.Click += btnSave_Click;
            this.Controls.Add(btnSave);
        }


        private TextBox CreateTextBox(string placeholder, ref int yOffset)
        {
            var textBox = new TextBox
            {
                Location = new Point(20, yOffset),
                Size = new Size(250, 20),
                PlaceholderText = placeholder
            };
            this.Controls.Add(new Label { Text = placeholder, Location = new Point(20, yOffset - 20), AutoSize = true });
            this.Controls.Add(textBox);
            yOffset += 40;
            return textBox;
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtProductName.Text) ||
                    string.IsNullOrWhiteSpace(txtBarCode.Text) ||
                    !decimal.TryParse(txtPackedWeight.Text, out decimal packedWeight) ||
                    !decimal.TryParse(txtPackedWidth.Text, out decimal packedWidth) ||
                    !decimal.TryParse(txtPackedHeight.Text, out decimal packedHeight) ||
                    !decimal.TryParse(txtPackedDepth.Text, out decimal packedDepth) ||
                    cmbCategory.SelectedValue == null ||
                    !int.TryParse(txtQuantity.Text, out int quantity) ||
                    !decimal.TryParse(txtPrice.Text, out decimal price))
                {
                    MessageBox.Show("Please ensure all fields are filled correctly.");
                    return;
                }

                if (_supplierDto == null)
                {
                    MessageBox.Show("Supplier information is missing.");
                    return;
                }

                var product = new ProductDto
                {
                    Name = txtProductName.Text,
                    BarCode = txtBarCode.Text,
                    ProductDescription = txtDescription.Text,
                    PackedWeight = packedWeight,
                    PackedWidth = packedWidth,
                    PackedHeight = packedHeight,
                    PackedDepth = packedDepth,
                    Refrigerated = chkRefrigerated.Checked,
                    CategoryId = (int)cmbCategory.SelectedValue
                };

                var savedProductId = await _productService.AddAsync(product);
                var stock = new StockDto
                {
                    ProductID = savedProductId,
                    SupplierOrdWarehouseId = _supplierDto.Id,
                    Quantity = quantity,
                    Price = price,
                    SKU = GenerateSKU(),
                    StockType = StockType.Supplier,
                    LastUpdated = DateTime.Now
                };

                await _stockService.AddAsync(stock);
                await _currentStockService.AddAsync(stock);

                MessageBox.Show("Product added to stock successfully.");
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private void ClearInputs()
        {
            txtProductName.Clear();
            txtBarCode.Clear();
            txtDescription.Clear();
            txtPackedWeight.Clear();
            txtPackedWidth.Clear();
            txtPackedHeight.Clear();
            txtPackedDepth.Clear();
            txtQuantity.Clear();
            txtPrice.Clear();
            chkRefrigerated.Checked = false;
            cmbCategory.SelectedIndex = -1;
        }

        private string GenerateSKU() => $"SKU-{DateTime.Now.Ticks}";
    }
}
