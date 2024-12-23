using AutoMapper;
using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DATABASE.Repositories;
using InventorySystem.DTO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventorySystem.CORE.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly ProductRepository _productRepository;
        private readonly StockRepository _stockRepository;
        private readonly CurrentStockRepository _currentStockRepository;
        private readonly CategoryRepository _categoryRepository;
        private readonly WarehouseRepository _warehouseRepository;
        private readonly IMapper _mapper;

        public DashboardService(
            ProductRepository productRepository,
            StockRepository stockRepository,
            CurrentStockRepository currentStockRepository,
            CategoryRepository categoryRepository,
            WarehouseRepository warehouseRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _stockRepository = stockRepository;
            _currentStockRepository = currentStockRepository;
            _categoryRepository = categoryRepository;
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }

        public async Task<DashboardDto> GetDashboardDataAsync(string userRole, int? userId = null)
        {
            var today = DateTime.Today;
            var dashboardDto = new DashboardDto();

            if (userRole == "Warehouse" && userId.HasValue)
            {
                // Kullanıcıya ait tüm warehouse'ları alın
                var warehouses = await _warehouseRepository.FindAll(w => w.UserID == userId.Value);

                foreach (var warehouse in warehouses)
                {
                    // Her bir warehouse için filtreleme
                    var totalProductsQuery = _productRepository.GetAll()
                        .Where(p => _currentStockRepository.GetAll()
                            .Any(cs => cs.ProductID == p.Id &&
                                       cs.SupplierOrdWarehouseId == warehouse.WarehouseID &&
                                       cs.StockType == StockType.Warehouse));

                    var currentStocksQuery = _currentStockRepository.GetAllWithProduct()
                        .Where(cs => cs.SupplierOrdWarehouseId == warehouse.WarehouseID && cs.StockType == StockType.Warehouse);

                    var dailyTransactionsQuery = _stockRepository.GetStockTransactionsByDate(today, today.AddDays(1))
                        .Where(st => st.From == warehouse.WarehouseID || st.To == warehouse.WarehouseID);

                    // Verileri hesaplama
                    try
                    {
                        var totalProducts = await totalProductsQuery.CountAsync();
                        var currentStocks = await currentStocksQuery.ToListAsync();
                        var totalStockQuantity = currentStocks.Sum(cs => cs.Quantity);
                        var totalStockValue = currentStocks.Sum(cs => cs.Quantity * cs.Price);
                    


                    var categories = await _categoryRepository.GetAllAsync();
                    var categoryStockData = categories.Select(category => new CategoryStockDto
                    {
                        CategoryName = category.Name,
                        TotalQuantity = currentStocks
                            .Where(cs => cs.Product != null && cs.Product.CategoryId == category.Id)
                            .Sum(cs => cs.Quantity)
                    }).ToList();

                    var categoryDistribution = categoryStockData
                        .Where(c => c.TotalQuantity > 0)
                        .Select(c => new KeyValuePair<string, double>(c.CategoryName, c.TotalQuantity))
                        .ToList();

                    var stockDistribution = currentStocks
                        .Where(cs => cs.Quantity > 0)
                        .GroupBy(cs => cs.Product.Name)
                        .Select(g => new KeyValuePair<string, double>(g.Key, g.Sum(cs => cs.Quantity)))
                        .ToList();

                    var dailyTransactions = await dailyTransactionsQuery.ToListAsync();
                    var dailyAddedProducts = dailyTransactions.Count(tx => tx.TransactionType == TransactionType.TransferIn);

                    // Warehouse verisini ekleme
                    dashboardDto.WarehouseData.Add(new WarehouseDashboardDto
                    {
                        WarehouseName = warehouse.WarehouseName,
                        TotalProducts = totalProducts,
                        TotalStockQuantity = totalStockQuantity,
                        TotalStockValue = totalStockValue,
                        CategoryStockData = categoryStockData,
                        DailyAddedProducts = dailyAddedProducts,
                        CategoryDistribution = categoryDistribution,
                        StockDistribution = stockDistribution
                    });
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: {ex.Message}");
                    }
                }
            }
            else if (userRole == "Supplier" && userId.HasValue)
            {
                // Supplier verileri
                var totalProductsQuery = _productRepository.GetAll()
                    .Where(p => _currentStockRepository.GetAll()
                        .Any(cs => cs.ProductID == p.Id &&
                                   cs.SupplierOrdWarehouseId == userId.Value &&
                                   cs.StockType == StockType.Supplier));

                var currentStocksQuery = _currentStockRepository.GetAllWithProduct()
                    .Where(cs => cs.SupplierOrdWarehouseId == userId.Value && cs.StockType == StockType.Supplier);

                var dailyTransactionsQuery = _stockRepository.GetStockTransactionsByDate(today, today.AddDays(1))
                    .Where(st => st.From == userId.Value || st.To == userId.Value);

                var totalProducts = await totalProductsQuery.CountAsync();
                var currentStocks = await currentStocksQuery.ToListAsync();
                var totalStockQuantity = currentStocks.Sum(cs => cs.Quantity);
                var totalStockValue = currentStocks.Sum(cs => cs.Quantity * cs.Price);

                var categories = await _categoryRepository.GetAllAsync();
                var categoryStockData = categories.Select(category => new CategoryStockDto
                {
                    CategoryName = category.Name,
                    TotalQuantity = currentStocks
                        .Where(cs => cs.Product != null && cs.Product.CategoryId == category.Id)
                        .Sum(cs => cs.Quantity)
                }).ToList();

                var categoryDistribution = categoryStockData
                    .Where(c => c.TotalQuantity > 0)
                    .Select(c => new KeyValuePair<string, double>(c.CategoryName, c.TotalQuantity))
                    .ToList();

                var stockDistribution = currentStocks
                    .Where(cs => cs.Quantity > 0)
                    .GroupBy(cs => cs.Product.Name)
                    .Select(g => new KeyValuePair<string, double>(g.Key, g.Sum(cs => cs.Quantity)))
                    .ToList();

                var dailyTransactions = await dailyTransactionsQuery.ToListAsync();
                var dailyAddedProducts = dailyTransactions.Count(tx => tx.TransactionType == TransactionType.TransferIn);

                // Supplier verisini ekleme
                dashboardDto.SupplierData = new SupplierDashboardDto
                {
                    TotalProducts = totalProducts,
                    TotalStockQuantity = totalStockQuantity,
                    TotalStockValue = totalStockValue,
                    CategoryStockData = categoryStockData,
                    DailyAddedProducts = dailyAddedProducts,
                    CategoryDistribution = categoryDistribution,
                    StockDistribution = stockDistribution
                };
            }

            return dashboardDto;
        }


        public class DashboardDto
        {
            public List<WarehouseDashboardDto> WarehouseData { get; set; } = new List<WarehouseDashboardDto>();
            public SupplierDashboardDto SupplierData { get; set; }
        }

        public class WarehouseDashboardDto
        {
            public string WarehouseName { get; set; }
            public int TotalProducts { get; set; }
            public int TotalStockQuantity { get; set; }
            public decimal TotalStockValue { get; set; }
            public List<CategoryStockDto> CategoryStockData { get; set; }
            public int DailyAddedProducts { get; set; }
            public List<KeyValuePair<string, double>> CategoryDistribution { get; set; }
            public List<KeyValuePair<string, double>> StockDistribution { get; set; }
        }

        public class SupplierDashboardDto
        {
            public int TotalProducts { get; set; }
            public int TotalStockQuantity { get; set; }
            public decimal TotalStockValue { get; set; }
            public List<CategoryStockDto> CategoryStockData { get; set; }
            public int DailyAddedProducts { get; set; }
            public List<KeyValuePair<string, double>> CategoryDistribution { get; set; }
            public List<KeyValuePair<string, double>> StockDistribution { get; set; }
        }

        public class CategoryStockDto
        {
            public string CategoryName { get; set; }
            public int TotalQuantity { get; set; }
        }
    }

}