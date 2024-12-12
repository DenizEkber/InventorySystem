using InventorySystem.DATABASE.CodeFirst.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.CORE.Interfaces
{
    public interface IStockTransactionService
    {
        Task<List<StockTransaction>> GetAllStockTransactionsAsync();
        Task AddStockTransactionAsync(StockTransaction transaction);
        Task DeleteStockTransactionAsync(int id);
    }
}
