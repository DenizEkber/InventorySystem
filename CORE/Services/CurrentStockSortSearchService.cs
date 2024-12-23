using AutoMapper;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DATABASE.Repositories;
using InventorySystem.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySystem.CORE.Services
{
    // DTO class
    /*public class CurrentStockDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public StockType StockType { get; set; }
    }*/

    public class CurrentStockSortSearchService
    {
        private readonly CurrentStockRepository _currentStockRepository;
        private readonly WarehouseRepository _warehouseRepository;
        private readonly IMapper _mapper;

        public CurrentStockSortSearchService(CurrentStockRepository currentStockRepository, WarehouseRepository warehouseRepository, IMapper mapper)
        {
            _currentStockRepository = currentStockRepository;
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }

        // Search and Sort method
        public async Task<List<object>> SearchAndSortAsync(
            bool isSupplierView = false,
            int? categoryId = null,
            string productName = null,
            decimal? minPrice = null,
            decimal? maxPrice = null,
            StockType? stockType = null,
            string sortBy = "ProductID",
            bool ascending = true)
        {
            // Retrieve all current stocks
            var stocks = await _currentStockRepository.GetAllWithThingsAsync();

            // Filtering
            if (categoryId.HasValue)
            {
                stocks = stocks.Where(cs => cs.Product.CategoryId == categoryId.Value).ToList();
            }

            if (!string.IsNullOrEmpty(productName))
            {
                stocks = stocks.Where(cs => cs.Product.Name.Contains(productName, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (minPrice.HasValue)
            {
                stocks = stocks.Where(cs => cs.Price >= minPrice.Value).ToList();
            }

            if (maxPrice.HasValue)
            {
                stocks = stocks.Where(cs => cs.Price <= maxPrice.Value).ToList();
            }

            if (stockType.HasValue)
            {
                stocks = stocks.Where(cs => cs.StockType == stockType.Value).ToList();
            }

            // Sorting
            if (sortBy != null)
            {
                stocks = sortBy.ToLower() switch
                {
                    "productname" => ascending ? stocks.OrderBy(cs => cs.Product.Name).ToList() : stocks.OrderByDescending(cs => cs.Product.Name).ToList(),
                    "category" => ascending ? stocks.OrderBy(cs => cs.Product.CategoryId).ToList() : stocks.OrderByDescending(cs => cs.Product.CategoryId).ToList(),
                    "price" => ascending ? stocks.OrderBy(cs => cs.Price).ToList() : stocks.OrderByDescending(cs => cs.Price).ToList(),
                    "stocktype" => ascending ? stocks.OrderBy(cs => cs.StockType).ToList() : stocks.OrderByDescending(cs => cs.StockType).ToList(),
                    _ => ascending ? stocks.OrderBy(cs => cs.ProductID).ToList() : stocks.OrderByDescending(cs => cs.ProductID).ToList(),
                };
            }

            if (isSupplierView)
            {
                var mappedSupplierStocks = _mapper.Map<List<SupplierInventoryDataGridViewDto>>(stocks);
                return mappedSupplierStocks.Cast<object>().ToList();
            }
            else
            {
                var mappedStocks = _mapper.Map<List<InevntryDataGridViewDto>>(stocks);
                foreach (var mappedStock in mappedStocks)
                {
                    var warehouseId = mappedStock.SupplierOrdWarehouseId;
                    var warehouse = await _warehouseRepository.GetByIdAsync(warehouseId);


                    mappedStock.WarehouseName = warehouse?.WarehouseName ?? "Unknown";
                }

                return mappedStocks.Cast<object>().ToList();
            }
            
        }

    }
    public class SupplierInventoryDataGridViewDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    public class InevntryDataGridViewDto
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string WarehouseName { get; set; }
        public string SKU { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public DateTime LastUpdated { get; set; }

        // Arka planda kullanılacak, UI'da gösterilmeyecek
        [System.ComponentModel.Browsable(false)]
        public int SupplierOrdWarehouseId { get; set; }
    }
}
