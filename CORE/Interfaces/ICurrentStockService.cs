using InventorySystem.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.CORE.Interfaces
{
    public interface ICurrentStockService
    {
        Task<IEnumerable<StockDto>> GetAllAsync();
        Task<StockDto> GetByIdAsync(int id);
        Task AddAsync(StockDto stockDto);
        Task UpdateAsync(StockDto stockDto);
        Task DeleteAsync(int id);
    }
}
