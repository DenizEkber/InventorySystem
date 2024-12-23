using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.CORE.Interfaces
{
    public interface IStockService
    {
        Task UpdateCurrentStockAsync();
        Task<IEnumerable<StockDto>> GetAllAsync();
        Task<StockDto> GetByIdAsync(int id);
        Task<int> AddAsync(StockDto stockDto);
        Task UpdateAsync(StockDto stockDto);
        Task DeleteAsync(int id);
        Task RequestPurchaseAsync(StockDto stockDto, int warehouseId, int quantity);
    }
}
