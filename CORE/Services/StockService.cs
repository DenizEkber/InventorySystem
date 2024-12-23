using AutoMapper;
using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DATABASE.Repositories;
using InventorySystem.DTO.Models;

namespace InventorySystem.CORE.Services
{
    public class StockService : IStockService
    {
        private readonly IMapper _mapper;
        private readonly StockRepository _stockRepository;
        private readonly CurrentStockRepository _currentStockRepository;
        private readonly IStockTransactionService _stockTransactionService;

        public StockService(StockRepository stockRepository, 
            CurrentStockRepository currentStockRepository,
            IStockTransactionService stockTransactionService,
            IMapper mapper)
        {
            _stockRepository = stockRepository;
            _currentStockRepository = currentStockRepository;
            _stockTransactionService = stockTransactionService;
            _mapper = mapper;
        }


        public async Task UpdateCurrentStockAsync()
        {
            // 1. En güncel stokları al
            var latestStocks = await _stockRepository.GetLatestStocksAsync();

            foreach (var stock in latestStocks)
            {
                // 2. CurrentStock tablosunda ilgili kaydı kontrol et
                //var currentStock = await _currentStockRepository.GetCurrentStockAsync(stock.ProductID, stock.SupplierOrdWarehouseId);
                var currentStock = await _currentStockRepository.FindFirstAsync(cs =>
                    cs.ProductID == stock.ProductID &&
                    cs.SupplierOrdWarehouseId == stock.SupplierOrdWarehouseId && cs.StockType == stock.StockType);

                if (currentStock != null)
                {
                    // Eğer varsa, güncelle
                    currentStock.Quantity = stock.Quantity;
                    currentStock.Price = stock.Price;
                    currentStock.SKU = stock.SKU;
                    currentStock.StockType = stock.StockType;
                    currentStock.LastUpdated = stock.LastUpdated;

                    await _currentStockRepository.UpdateAsync(currentStock);
                }
                else
                {
                    // Eğer yoksa, yeni kayıt ekle
                    var newCurrentStock = new CurrentStock
                    {
                        ProductID = stock.ProductID,
                        SupplierOrdWarehouseId = stock.SupplierOrdWarehouseId,
                        Quantity = stock.Quantity,
                        Price = stock.Price,
                        SKU = stock.SKU,
                        StockType = stock.StockType,
                        LastUpdated = stock.LastUpdated
                    };

                    await _currentStockRepository.AddAsync(newCurrentStock);
                }
            }

            // 3. Değişiklikleri kaydet
            //await _currentStockRepository.SaveChangesAsync();
        }

        public async Task<int> AddAsync(StockDto stockDto)
        {
            //await _warehouseRepository.AddAsync(warehouseDto);
            /*var warehouseExists = await _stockRepository.FindFirstAsync(w => w.WarehouseName == warehouseDto.WarehouseName);
            if (warehouseExists != null)
            {
                //return null;
            }*/
            var stock = _mapper.Map<Stock>(stockDto);
            await _stockRepository.AddAsync(stock);
            return stock.ID;
        }

        public async Task DeleteAsync(int id)
        {
            var stock = await _stockRepository.GetByIdAsync(id);
            if (stock != null)
            {
                await _stockRepository.DeleteAsync(stock);
            }
        }

        public async Task<IEnumerable<StockDto>> GetAllAsync()
        {
            //return await _warehouseRepository.GetAllAsync();
            var stock = await _stockRepository.GetAllAsync();
            var mappedStock = _mapper.Map<IEnumerable<StockDto>>(stock);
            return mappedStock;
        }

        public async Task<StockDto> GetByIdAsync(int id)
        {
            var stock = await _stockRepository.GetByIdAsync(id);
            if (stock == null)
            {
                return null;
            }
            var mappedStock = _mapper.Map<StockDto>(stock);
            return mappedStock;
        }

        public async Task UpdateAsync(StockDto stockDto)
        {
            //await _warehouseRepository.UpdateAsync(warehouse);
            var stock = await _stockRepository.GetByIdAsync(stockDto.ID);
            if (stock == null)
            {

            }

            _mapper.Map(stockDto, stock);
            await _stockRepository.UpdateAsync(stock);

        }
        public async Task RequestPurchaseAsync(StockDto stockDto, int warehouseId, int quantity)
        {
            // Supplier stok kontrolü
            var currentStock = await _currentStockRepository.FindFirstAsync(s =>
                s.ProductID == stockDto.ProductID &&
                s.SupplierOrdWarehouseId == stockDto.SupplierOrdWarehouseId &&
                s.StockType == StockType.Supplier);

            if (currentStock == null || currentStock.Quantity < quantity)
            {
                throw new Exception("Insufficient stock for this product.");
            }

            // Supplier stoktan ürün düş
            currentStock.Quantity -= quantity;

            var stock = _mapper.Map<Stock>(currentStock);
            stock.ID = default;
            await _stockRepository.AddAsync(stock);

            // İlk işlem: Supplier'den satış
            var saleTransactionDto = new StockTransactionDto
            {
                StockID = stock.ID,
                From = stockDto.SupplierOrdWarehouseId,
                To = warehouseId,
                Quantity = quantity,
                TransactionDate = DateTime.Now,
                TransactionType = TransactionType.Sale
            };
            await _stockTransactionService.AddAsync(saleTransactionDto);

            // Warehouse işlemleri
            var warehouseCurrentStock = await _currentStockRepository.FindFirstAsync(cs =>
                cs.ProductID == stockDto.ProductID && cs.SupplierOrdWarehouseId == warehouseId &&
                cs.StockType == StockType.Warehouse);

            Stock addedStock;

            if (warehouseCurrentStock != null)
            {
                // Mevcut warehouse stokuna ürün ekle
                warehouseCurrentStock.Quantity += quantity;
                var warehouseStock = _mapper.Map<Stock>(warehouseCurrentStock);
                warehouseStock.ID = default;
                addedStock = await _stockRepository.AddReturnInfoAsync(warehouseStock);
                await _currentStockRepository.UpdateAsync(warehouseCurrentStock);
            }
            else
            {
                // Yeni warehouse stoğu oluştur
                warehouseCurrentStock = new CurrentStock
                {
                    ProductID = stockDto.ProductID,
                    SupplierOrdWarehouseId = warehouseId,
                    SKU = stockDto.SKU,
                    Quantity = quantity,
                    Price = stockDto.Price,
                    StockType = StockType.Warehouse,
                    LastUpdated = DateTime.Now
                };

                var newWarehouseStock = _mapper.Map<Stock>(warehouseCurrentStock);
                addedStock = await _stockRepository.AddReturnInfoAsync(newWarehouseStock);
            }
            var buyTransaction = new StockTransactionDto
            {
                StockID = addedStock.ID,
                From = warehouseCurrentStock.SupplierOrdWarehouseId,
                To = warehouseId,
                Quantity = quantity,
                TransactionDate = DateTime.Now,
                TransactionType = TransactionType.Purchase
            };
            await _stockTransactionService.AddAsync(buyTransaction);

            // 2 dakika sonra Supplier'den çıkış işlemi kaydı
            //_ = Task.Run(async () =>
            //{
            //    await Task.Delay(TimeSpan.FromMinutes(2));
            //    var transferOutTransaction = new StockTransactionDto
            //    {
            //        StockID = currentStock.ID,
            //        From = stockDto.SupplierOrdWarehouseId,
            //        To = warehouseId,
            //        Quantity = quantity,
            //        TransactionDate = DateTime.Now,
            //        TransactionType = TransactionType.TransferOut
            //    };
            //    await _stockTransactionService.AddAsync(transferOutTransaction);
            //});

            //// 2 saat sonra Warehouse'a giriş işlemi kaydı
            //_ = Task.Run(async () =>
            //{
            //    await Task.Delay(TimeSpan.FromHours(2));
            //    var transferInTransaction = new StockTransactionDto
            //    {
            //        StockID = addedStock?.ID ?? 0, // Warehouse stoğu varsa ID'sini al
            //        From = stockDto.SupplierOrdWarehouseId,
            //        To = warehouseId,
            //        Quantity = quantity,
            //        TransactionDate = DateTime.Now,
            //        TransactionType = TransactionType.TransferIn
            //    };
            //    await _stockTransactionService.AddAsync(transferInTransaction);
            //});
        }


    }
}
