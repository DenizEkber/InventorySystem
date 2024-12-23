using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventorySystem.UI.FormPanel
{
    public class SearchPanel : Panel
    {
        private ComboBox cmbCategory; // Kategori ComboBox'ı
        private TextBox txtProductName;
        private TextBox txtMinPrice;
        private TextBox txtMaxPrice;
        private ComboBox cmbSortBy;
        private RadioButton rbAscending;
        private RadioButton rbDescending;
        private Button btnSearch;
        private bool _isSupplier;

        public event EventHandler<SearchEventArgs> SearchRequested;
        private readonly StockType _stockType;
        private readonly ICategoryService _categoryService;

        public SearchPanel(StockType stockType, ICategoryService categoryService, bool isSuplier)
        {
            _isSupplier = isSuplier;
            _stockType = stockType;
            _categoryService = categoryService;
            InitializeComponents();
            LoadCategoriesAsync();
        }

        private void InitializeComponents()
        {
            // Initialize controls
            cmbCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
            txtProductName = new TextBox();
            txtMinPrice = new TextBox();
            txtMaxPrice = new TextBox();
            cmbSortBy = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Items = { "ProductName", "Price", "StockType" } };
            rbAscending = new RadioButton { Text = "Ascending", Checked = true };
            rbDescending = new RadioButton { Text = "Descending" };
            btnSearch = new Button { Text = "Search" };

            // Set up TableLayoutPanel
            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 6,
                AutoSize = true,
                Padding = new Padding(10)
            };

            // Add controls to layout
            layout.Controls.Add(new Label { Text = "Category", AutoSize = true }, 0, 0);
            layout.Controls.Add(cmbCategory, 1, 0);
            layout.Controls.Add(new Label { Text = "Product Name", AutoSize = true }, 0, 1);
            layout.Controls.Add(txtProductName, 1, 1);
            layout.Controls.Add(new Label { Text = "Min Price", AutoSize = true }, 0, 2);
            layout.Controls.Add(txtMinPrice, 1, 2);
            layout.Controls.Add(new Label { Text = "Max Price", AutoSize = true }, 0, 3);
            layout.Controls.Add(txtMaxPrice, 1, 3);

            // Sorting options (Radio buttons)
            FlowLayoutPanel sortingPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            sortingPanel.Controls.Add(rbAscending);
            sortingPanel.Controls.Add(rbDescending);
            layout.Controls.Add(new Label { Text = "Sort Order", AutoSize = true }, 0, 4);
            layout.Controls.Add(sortingPanel, 1, 4);

            // Search button
            FlowLayoutPanel buttonPanel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true };
            buttonPanel.Controls.Add(btnSearch);
            layout.Controls.Add(buttonPanel, 1, 5);

            // Add event handler for search button
            btnSearch.Click += BtnSearch_Click;

            // Add layout to the panel
            this.Controls.Add(layout);

            // Panel appearance
            this.BackColor = Color.LightGray;
        }

        private async void LoadCategoriesAsync()
        {
            try
            {
                var categories = await _categoryService.GetAllCategoriesAsync();

                // UI thread üzerinde çalıştığından emin olmak için Invoke kullanıyoruz
                if (cmbCategory.InvokeRequired)
                {
                    cmbCategory.Invoke(new Action(() =>
                    {
                        cmbCategory.DataSource = categories;
                        cmbCategory.DisplayMember = "Name";
                        cmbCategory.ValueMember = "Id";
                    }));
                }
                else
                {
                    cmbCategory.DataSource = categories;
                    cmbCategory.DisplayMember = "Name";
                    cmbCategory.ValueMember = "Id";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kategori yüklenirken bir hata oluştu: {ex.Message}");
            }
        }


        private void BtnSearch_Click(object sender, EventArgs e)
        {
            var categoryId = cmbCategory.SelectedValue != null ? (int?)cmbCategory.SelectedValue : null;
            var productName = txtProductName.Text;
            var minPrice = string.IsNullOrEmpty(txtMinPrice.Text) ? (decimal?)null : decimal.Parse(txtMinPrice.Text);
            var maxPrice = string.IsNullOrEmpty(txtMaxPrice.Text) ? (decimal?)null : decimal.Parse(txtMaxPrice.Text);
            var sortBy = cmbSortBy.SelectedItem?.ToString();
            var ascending = rbAscending.Checked;

            // Raise the SearchRequested event with the current search parameters
            SearchRequested?.Invoke(this, new SearchEventArgs(_isSupplier, categoryId, productName, minPrice, maxPrice, _stockType, sortBy, ascending));
        }
    }

    public class SearchEventArgs : EventArgs
    {
        public bool IsSupplierView { get; }
        public int? CategoryId { get; }
        public string ProductName { get; }
        public decimal? MinPrice { get; }
        public decimal? MaxPrice { get; }
        public StockType? StockType { get; }
        public string SortBy { get; }
        public bool Ascending { get; }

        public SearchEventArgs(bool isSupplierView, int? categoryId, string productName, decimal? minPrice, decimal? maxPrice, StockType? stockType, string sortBy, bool ascending)
        {
            IsSupplierView = isSupplierView;
            CategoryId = categoryId;
            ProductName = productName;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            StockType = stockType;
            SortBy = sortBy;
            Ascending = ascending;
        }
    }
}
