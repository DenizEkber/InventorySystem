using InventorySystem.CORE.Interfaces;
using InventorySystem.CORE.Services;
using InventorySystem.DATABASE.CodeFirst.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.UI.FormPanel
{
    public class InventoryPanel : Panel
    {
        private SearchPanel searchPanel;
        private ResultPanel resultPanel;
        private CurrentStockSortSearchService _service;
        private ICategoryService _categoryService;
        Role _role;

        public InventoryPanel(CurrentStockSortSearchService service, Role role, ICategoryService categoryService)
        {
            _role = role;
            _categoryService = categoryService;
            _service = service;
            InitializeComponents();
        }
        private StockType ConvertToStockType(Role role)
        {
            switch (role)
            {
                case Role.Supplier:
                    return StockType.Supplier;
                case Role.Warehouse:
                    return StockType.Warehouse;
                default:
                    throw new ArgumentException("Geçersiz rol türü", nameof(role));
            }
        }


        private void InitializeComponents()
        {
            // Ana panel ayarları
            this.Size = new Size(Parent?.Width ?? 800, Parent?.Height ?? 600);
            this.Dock = DockStyle.Fill;
            this.BackColor = Color.FromArgb(33, 33, 33); // Arka plan için koyu renk (örneğin siyah tonları)

            // SearchPanel oluştur
            this.searchPanel = new SearchPanel(ConvertToStockType(_role), _categoryService, _role==Role.Supplier ? true:false)
            {
                Width = (int)(1920 * 0.25),  // Panelin sol kenarında %25 genişlik
                Height = 1000 - 40,        // Panelin yüksekliği, alt-üst boşluklarla
                Location = new Point(20, 20),     // Sol ve üstten 20 piksel boşluk
                Padding = new Padding(15),        // İç kenarlara boşluk
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.FromArgb(40, 40, 40), // Koyu gri arka plan
            };

            // ResultPanel oluştur
            this.resultPanel = new ResultPanel
            {
                Width = 1920 - searchPanel.Width - 60, // Geri kalan genişlik
                Height = 1000 - 40,                  // Yükseklik
                Location = new Point(searchPanel.Width + 40, 20), // SearchPanel'in sağında
                Padding = new Padding(15),
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(45, 45, 45), // Hafif açık gri arka plan
            };

            // Panelleri ana panele ekle
            this.Controls.Add(searchPanel);
            this.Controls.Add(resultPanel);

            // SearchPanel'in SearchRequested eventine abone ol
            searchPanel.SearchRequested += OnSearchRequested;
        }


        private async void OnSearchRequested(object sender, SearchEventArgs e)
        {
            // Perform the search with the provided parameters
            var results = await _service.SearchAndSortAsync(e.IsSupplierView, e.CategoryId, e.ProductName, e.MinPrice, e.MaxPrice, e.StockType, e.SortBy, e.Ascending);

            // Display the results in the result panel
            resultPanel.DisplayResults(results);
        }
    }

}
