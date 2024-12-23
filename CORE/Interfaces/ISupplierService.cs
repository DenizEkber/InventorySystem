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
    public interface ISupplierService
    {
        Task<IEnumerable<SupplierDto>> GetAllAsync();
        Task<SupplierDto> GetByIdAsync(int id);
        Task AddAsync(SupplierDto supplierDto);
        Task UpdateAsync(SupplierDto supplierDto);
        Task DeleteAsync(int id);
        Task<SupplierDto> FindFirstAsync(Expression<Func<Supplier, bool>> predicate);
        Task<List<KeyValuePair<string, double>>> GetSupplierStatisticsAsync(int userId);
    }
}
