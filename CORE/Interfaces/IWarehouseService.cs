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
    public interface IWarehouseService
    {
        Task<IEnumerable<WarehouseDto>> GetAllAsync();
        Task<WarehouseDto> GetByIdAsync(int id);
        Task AddAsync(WarehouseDto warehouseDto);
        Task UpdateAsync(WarehouseDto warehouseDto);
        Task DeleteAsync(int id);
        Task<WarehouseDto> FindFirstAsync(Expression<Func<Warehouse, bool>> predicate);
        Task<List<KeyValuePair<string, double>>> GetWarehouseStatisticsAsync(int userId);
    }
}
