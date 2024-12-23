using InventorySystem.CORE.Interfaces;
using InventorySystem.DTO.Models;
using LiveChartsCore.SkiaSharpView.WinForms;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using InventorySystem.DATABASE.CodeFirst.Entities;

namespace InventorySystem.UI.FormPanel
{
    public class DashboardPanel : Panel
    {
        private readonly IDashboardService _dashboardService;
        private UserDto _currentDto;
        private ComboBox _warehouseComboBox;
        private Panel _contentPanel;

        public DashboardPanel(IDashboardService dashboardService, UserDto currentDto)
        {
            _dashboardService = dashboardService;
            _currentDto = currentDto;
            InitializeDashboardAsync();
        }

        private async void InitializeDashboardAsync()
        {
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(30, 30, 30);

            _warehouseComboBox = new ComboBox
            {
                Location = new Point(20, 20),
                Size = new Size(300, 30),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            // Eğer kullanıcı Supplier rolündeyse, ComboBox'ı gizleyelim
            if (_currentDto.Role == Role.Supplier)
            {
                _warehouseComboBox.Visible = false; // ComboBox'ı gizle
            }
            else
            {
                _warehouseComboBox.SelectedIndexChanged += async (s, e) => await OnWarehouseSelected();
            }
            this.Controls.Add(_warehouseComboBox);

            _contentPanel = new Panel
            {
                Location = new Point(20, 60),
                Size = new Size(this.Width - 40, this.Height - 80),
                Dock = DockStyle.Fill,
                AutoScroll = true
            };
            this.Controls.Add(_contentPanel);

            string userRole = _currentDto.Role switch
            {
                Role.Supplier => "Supplier",
                Role.Warehouse => "Warehouse",
                _ => throw new ArgumentException("Invalid role")
            };

            var dashboardData = await _dashboardService.GetDashboardDataAsync(userRole, _currentDto.UserID);

            if (userRole == "Warehouse")
            {
                _warehouseComboBox.Items.AddRange(dashboardData.WarehouseData.Select(w => w.WarehouseName).ToArray());
                if (_warehouseComboBox.Items.Count > 0)
                    _warehouseComboBox.SelectedIndex = 0;
            }
            else if (userRole == "Supplier")
            {
                DisplaySupplierData(dashboardData.SupplierData);
            }
        }
        public async Task RefreshDashboardAsync()
        {
            // Eski verileri temizle
            _warehouseComboBox.Items.Clear();
            _contentPanel.Controls.Clear();

            string userRole = _currentDto.Role switch
            {
                Role.Supplier => "Supplier",
                Role.Warehouse => "Warehouse",
                _ => throw new ArgumentException("Invalid role")
            };

            var dashboardData = await _dashboardService.GetDashboardDataAsync(userRole, _currentDto.UserID);

            if (userRole == "Warehouse")
            {
                // Yeni verileri ekle
                _warehouseComboBox.Items.AddRange(dashboardData.WarehouseData.Select(w => w.WarehouseName).ToArray());
                if (_warehouseComboBox.Items.Count > 0)
                    _warehouseComboBox.SelectedIndex = 0;
            }
            else if (userRole == "Supplier")
            {
                DisplaySupplierData(dashboardData.SupplierData);
            }
        }


        private async Task OnWarehouseSelected()
        {
            var selectedWarehouse = _warehouseComboBox.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(selectedWarehouse)) return;

            var dashboardData = await _dashboardService.GetDashboardDataAsync("Warehouse", _currentDto.UserID);
            var selectedData = dashboardData.WarehouseData.FirstOrDefault(w => w.WarehouseName == selectedWarehouse);

            if (selectedData != null)
            {
                _contentPanel.Controls.Clear();
                var kpiPanel = CreateKPIPanel(selectedData);
                kpiPanel.Location = new Point(0, 60);
                _contentPanel.Controls.Add(kpiPanel);

                var categoryChart = CreatePieChart($"Category Distribution: {selectedData.WarehouseName}", selectedData.CategoryDistribution);
                categoryChart.Location = new Point(0, 240);
                _contentPanel.Controls.Add(categoryChart);

                var stockChart = CreateBarChart("Stock Distribution", selectedData.StockDistribution);
                stockChart.Location = new Point(0, 510);
                _contentPanel.Controls.Add(stockChart);
            }
        }

        private void DisplaySupplierData(dynamic data)
        {
            var kpiPanel = CreateKPIPanel(data);
            kpiPanel.Location = new Point(0, 0);
            _contentPanel.Controls.Add(kpiPanel);

            var categoryChart = CreatePieChart("Category Distribution", data.CategoryDistribution);
            categoryChart.Location = new Point(0, 180);
            _contentPanel.Controls.Add(categoryChart);

            var stockChart = CreateBarChart("Stock Distribution", data.StockDistribution);
            stockChart.Location = new Point(0, 450);
            _contentPanel.Controls.Add(stockChart);
        }

        private Panel CreateKPIPanel(dynamic data)
        {
            var panel = new Panel
            {
                Size = new Size(600, 150),
                BackColor = Color.FromArgb(50, 50, 50)
            };

            var totalProductsLabel = new Label
            {
                Text = $"Total Products: {data.TotalProducts}",
                Size = new Size(500, 40),
                ForeColor = Color.White,
                Font = new Font("Arial", 12),
                Location = new Point(20, 20)
            };

            var totalStockLabel = new Label
            {
                Text = $"Total Stock Quantity: {data.TotalStockQuantity}",
                Size = new Size(500, 40),
                ForeColor = Color.White,
                Font = new Font("Arial", 12),
                Location = new Point(20, 70)
            };

            var totalStockValueLabel = new Label
            {
                Text = $"Total Stock Value: ${data.TotalStockValue:F2}",
                Size = new Size(500, 40),
                ForeColor = Color.White,
                Font = new Font("Arial", 12),
                Location = new Point(20, 120)
            };

            panel.Controls.Add(totalProductsLabel);
            panel.Controls.Add(totalStockLabel);
            panel.Controls.Add(totalStockValueLabel);

            return panel;
        }

        private PieChart CreatePieChart(string title, List<KeyValuePair<string, double>> data)
        {
            var pieSeries = data.Select(item => new LiveChartsCore.SkiaSharpView.PieSeries<double>
            {
                Name = item.Key,
                Values = new[] { item.Value },
                Fill = new SolidColorPaint(SKColors.CornflowerBlue)
            }).ToList();

            return new PieChart
            {
                Series = pieSeries,
                Size = new Size(400, 250),
                BackColor = Color.FromArgb(40, 40, 40)
            };
        }

        private CartesianChart CreateBarChart(string title, List<KeyValuePair<string, double>> data)
        {
            var barSeries = new LiveChartsCore.SkiaSharpView.ColumnSeries<double>
            {
                Values = data.Select(d => d.Value).ToArray(),
                Fill = new SolidColorPaint(SKColors.SteelBlue)
            };

            return new CartesianChart
            {
                Series = new[] { barSeries },
                XAxes = new[] { new Axis { Labels = data.Select(d => d.Key).ToArray() } },
                Size = new Size(600, 250),
                BackColor = Color.FromArgb(40, 40, 40)
            };
        }
    }
}
