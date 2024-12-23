using InventorySystem.DATABASE.CodeFirst.Context;
using InventorySystem.DATABASE.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.DATABASE.Repositories
{
    public class StockRepository : BaseRepository<Stock>
    {
        public StockRepository(AppDbContext context) : base(context)
        {
        }
        // En güncel stokları al
        public async Task<List<Stock>> GetLatestStocksAsync()
        {
            return await Task.Run(() =>
                _context.Stocks
                .GroupBy(s => new { s.ProductID, s.SupplierOrdWarehouseId })
                .Select(g => g.OrderByDescending(s => s.LastUpdated).FirstOrDefault())
                .ToList()
            );
        }
        public async Task<Stock> AddReturnInfoAsync(Stock stock)
        {
            await base.AddAsync(stock);
            return stock;
        }
        public async Task<List<StockTransaction>> GetStockTransactionsByDateAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.StockTransactions
                .Where(tx => tx.TransactionDate >= startDate && tx.TransactionDate < endDate).ToListAsync();

        }
        public IQueryable<StockTransaction> GetStockTransactionsByDate(DateTime startDate, DateTime endDate)
        {
            return _context.StockTransactions
                .Where(tx => tx.TransactionDate >= startDate && tx.TransactionDate < endDate).AsQueryable();
        }

    }
}
