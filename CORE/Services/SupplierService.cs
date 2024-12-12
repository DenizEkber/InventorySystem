using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DATABASE.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.CORE.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly SupplierRepository _repository;

        public SupplierService(SupplierRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            return (await _repository.GetAllAsync()).ToList();
        }

        public async Task AddSupplierAsync(Supplier supplier)
        {
            await _repository.AddAsync(supplier);
        }

        public async Task DeleteSupplierAsync(int id)
        {
            var supplier = await _repository.GetByIdAsync(id);
            if (supplier != null)
            {
                await _repository.DeleteAsync(supplier);
            }
        }
    }


}
