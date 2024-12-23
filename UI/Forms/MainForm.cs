using System.Drawing.Drawing2D;
using InventorySystem.CORE.Services;
using InventorySystem.UI.FormPanel;
using InventorySystem.DTO.Models;
using InventorySystem.DATABASE.CodeFirst.Entities;
using Microsoft.Extensions.DependencyInjection;
using InventorySystem.CORE.Interfaces;

namespace InventorySystem.UI.Forms
{
    public partial class MainForm : Form
    {
        private UserDto _currentUser;
        private List<Button> navigationButtons = new List<Button>();
        private List<Control> panels = new List<Control>();
        private CurrentStockSortSearchService _service;
        private ISupplierService _supplierService;
        private IWarehouseService _warehouseService;
        private ICategoryService _categoryService;
        private IDashboardService _dashboardService;
        private IUserService _userService;
        private IServiceScope _currentScope;

        public MainForm(CurrentStockSortSearchService service, ICategoryService categoryService, ISupplierService supplierService, IWarehouseService warehouseService, IDashboardService dashboardService, IUserService userService)
        {
            _service = service;
            _categoryService = categoryService;
            _supplierService = supplierService;
            _warehouseService = warehouseService;
            _dashboardService = dashboardService;
            _userService = userService;
            InitializeComponent();
            
        }

        public void SetUser(UserDto user)
        {
            _currentUser = user;
            SetupUI();
        }

        private async void SetupUI()
        {
            this.Text = "Inventar";
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.WindowState = FormWindowState.Normal;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Bounds = Screen.PrimaryScreen.Bounds;

            Panel customTitleBar = new Panel
            {
                Size = new Size(this.Width, 40),
                BackColor = Color.FromArgb(40, 40, 40),
                Dock = DockStyle.Top
            };

            Button closeButton = new Button
            {
                Text = "X",
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(40, 30),
                Location = new Point(this.Width - 50, 5)
            };
            closeButton.Click += (s, e) => this.Close();
            customTitleBar.Controls.Add(closeButton);

            Button minimizeButton = new Button
            {
                Text = "_",
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Size = new Size(40, 30),
                Location = new Point(this.Width - 100, 5)
            };
            minimizeButton.Click += (s, e) => this.WindowState = FormWindowState.Minimized;
            customTitleBar.Controls.Add(minimizeButton);

            if (await IsFirstLogin())
            {
                OpenFirstLoginPanel();
            }
            else
            {
                await InitializeMainUI();
            }

            this.Controls.Add(customTitleBar);
            
        }

        private void OpenFirstLoginPanel()
        {
            if (_currentUser.Role == Role.Supplier)
            {
                try
                {
                    var addSupplierInfo = new AddSupplierInfo(_supplierService, _currentUser)
                    {
                        Dock = DockStyle.Fill
                    };

                    addSupplierInfo.InfoSaved += async (s, e) => await OnInfoSaved();
                    ShowPanel(addSupplierInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading Supplier panel: {ex.Message}");
                }
            }
            else if (_currentUser.Role == Role.Warehouse)
            {
                try
                {
                    var addWarehouseInfo = new AddWareHouseInfo(_warehouseService, _currentUser)
                    {
                        Dock = DockStyle.Fill
                    };

                    addWarehouseInfo.InfoSaved += async (s, e) => await OnInfoSaved();
                    ShowPanel(addWarehouseInfo);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading Warehouse panel: {ex.Message}");
                }
            }
        }

        private async Task OnInfoSaved()
        {
            MessageBox.Show("Information saved! Initializing main UI...");
            await InitializeMainUI();
        }

        private async Task<bool> IsFirstLogin()
        {
            try
            {
                var hasSupplierInfo = await _supplierService.FindFirstAsync(s => s.UserID == _currentUser.UserID) != null;
                var hasWarehouseInfo = await _warehouseService.FindFirstAsync(s => s.UserID == _currentUser.UserID) != null;
                return !(hasSupplierInfo || hasWarehouseInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking first login: {ex.Message}");
                return false;
            }
        }

        private async Task InitializeMainUI()
        {
            try
            {
                Panel navigationPanel = new Panel
                {
                    Size = new Size(this.Width, 60),
                    BackColor = Color.FromArgb(50, 50, 50),
                    Dock = DockStyle.Top
                };

                this.Controls.Add(navigationPanel);
                _currentScope = Program.Host.Services.CreateScope();

                await SetupPanelsByRole(navigationPanel);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing UI: {ex.Message}");
            }
        }

        private async Task SetupPanelsByRole(Panel navigationPanel)
        {
            int navOffset = 20;

            Panel dashboardPanel = new DashboardPanel(_dashboardService, _currentUser)
            {
                Dock = DockStyle.Fill
            };
            panels.Add(dashboardPanel);
            Button dashboardButton = CreateNavigationButton("Dashboard", dashboardPanel);
            dashboardButton.Location = new Point(navOffset, 10);
            navigationPanel.Controls.Add(dashboardButton);
            navOffset += 130;

            // Inventory Panel
            Panel inventoryPanel = new InventoryPanel(_service, _currentUser.Role, _categoryService)
            {
                Dock = DockStyle.Fill
            };
            panels.Add(inventoryPanel);
            Button inventoryButton = CreateNavigationButton("Inventory", inventoryPanel);
            inventoryButton.Location = new Point(navOffset, 10);
            navigationPanel.Controls.Add(inventoryButton);
            navOffset += 130;

            // Supplier (Sales) Panel
            if (_currentUser.Role == Role.Supplier)
            {
                var salesPanel = Program.Host.Services.GetRequiredService<SalePanel>();
                await salesPanel.SetSupplier(_currentUser);
                salesPanel.Dock = DockStyle.Fill;
                panels.Add(salesPanel);
                Button salesButton = CreateNavigationButton("Sales", salesPanel);
                salesButton.Location = new Point(navOffset, 10);
                navigationPanel.Controls.Add(salesButton);
                navOffset += 130;
            }

            // Warehouse (Buys) Panel
            if (_currentUser.Role == Role.Warehouse)
            {
                var buysPanel = Program.Host.Services.GetRequiredService<BuyProductPanel>();///
                buysPanel.SetUser(_currentUser);
                buysPanel.Dock = DockStyle.Fill;
                panels.Add(buysPanel);
                Button buysButton = CreateNavigationButton("Buys", buysPanel);
                buysButton.Location = new Point(navOffset, 10);
                navigationPanel.Controls.Add(buysButton);
                navOffset += 130;
            }

            // Profile Panel
            Panel profilePanel = new ProfilePanel(_currentUser, _userService,_warehouseService)
            {
                Dock = DockStyle.Fill
            };
            panels.Add(profilePanel);
            Button profileButton = CreateNavigationButton("Profile", profilePanel);
            profileButton.Anchor = Anchor = AnchorStyles.Top | AnchorStyles.Right;
            profileButton.Location = new Point(navigationPanel.Width - profileButton.Width - 20, 10);
            navigationPanel.Controls.Add(profileButton);
            navOffset += 130;
            //ShowPanel(profilePanel);

        }

        private Button CreateNavigationButton(string text, Control targetPanel)
        {
            Button button = new Button
            {
                Text = text,
                BackColor = Color.FromArgb(50, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 40),
                Margin = new Padding(10)
            };

            button.FlatAppearance.BorderSize = 0;
            button.Click += async (s, e) => await OnNavigationButtonClick(button, targetPanel);

            return button;
        }

        private Button previousButton = null;

        private async Task OnNavigationButtonClick(Button clickedButton, Control targetPanel)
        {
            if (previousButton != null)
            {
                previousButton.BackColor = Color.FromArgb(50, 50, 50);
                previousButton.ForeColor = Color.White;
            }

            clickedButton.BackColor = Color.FromArgb(100, 100, 100);
            clickedButton.ForeColor = Color.Black;

            if (targetPanel is DashboardPanel dashboardPanel)
            {
                if (this.InvokeRequired)
                {
                    // UI thread'ine geri dönüyoruz
                    this.Invoke(new Action(async () => await dashboardPanel.RefreshDashboardAsync()));
                }
                else
                {
                    // UI thread'indeyiz, direkt çağırabiliriz
                    await dashboardPanel.RefreshDashboardAsync();
                }
            }
            if (targetPanel is BuyProductPanel buyProductPanel)
            {
                await buyProductPanel.LoadWarehousesAsync();
            }

            targetPanel.Refresh();
            ShowPanel(targetPanel);

            previousButton = clickedButton;
        }

        private void ShowPanel(Control panelToShow)
        {
            foreach (Control panel in panels)
            {
                panel.Visible = false;
            }
            
            if (!this.Controls.Contains(panelToShow))
            {
                this.Controls.Add(panelToShow);
            }

            panelToShow.Visible = true;
            panelToShow.BringToFront();
        }
    }
}
