using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.CORE.Interfaces
{
    public interface IStockTransactionService
    {
        Task<IEnumerable<StockTransactionDto>> GetAllAsync();
        Task<StockTransactionDto> GetByIdAsync(int id);
        Task AddAsync(StockTransactionDto stockTransactionDto);
        Task DeleteAsync(int id);
        Task UpdateAsync(StockTransactionDto stockTransactionDto);
        Task<StockTransactionDto> FindFirstAsync(Expression<Func<StockTransaction, bool>> predicate);
    }
}
