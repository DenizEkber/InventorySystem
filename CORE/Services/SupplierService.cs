using AutoMapper;
using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DATABASE.Repositories;
using InventorySystem.DTO.Models;
using System.Linq.Expressions;

namespace InventorySystem.CORE.Services
{
    public class SupplierService : ISupplierService
    {
        private readonly IMapper _mapper;
        private readonly SupplierRepository _supplierRepository;

        public SupplierService(SupplierRepository repository, IMapper mapper)
        {
            _supplierRepository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<SupplierDto>> GetAllAsync()
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<SupplierDto>>(suppliers);
        }

        public async Task<SupplierDto> GetByIdAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            return supplier == null ? null : _mapper.Map<SupplierDto>(supplier);
        }

        public async Task AddAsync(SupplierDto supplierDto)
        {
            var supplierExists = await _supplierRepository.FindFirstAsync(w => w.Name == supplierDto.Name);
            if (supplierExists != null)
            {
                MessageBox.Show("Supplier already exists.");
                return;
            }

            var supplier = _mapper.Map<Supplier>(supplierDto);
            await _supplierRepository.AddAsync(supplier);
        }

        public async Task DeleteAsync(int id)
        {
            var supplier = await _supplierRepository.GetByIdAsync(id);
            if (supplier != null)
            {
                await _supplierRepository.DeleteAsync(supplier);
            }
        }

        public async Task UpdateAsync(SupplierDto supplierDto)
        {
            var supplier = await _supplierRepository.GetByIdAsync(supplierDto.Id);
            if (supplier == null) return;

            _mapper.Map(supplierDto, supplier);
            await _supplierRepository.UpdateAsync(supplier);
        }

        public async Task<SupplierDto> FindFirstAsync(Expression<Func<Supplier, bool>> predicate)
        {
            var supplier = await _supplierRepository.FindFirstAsync(predicate);
            return supplier == null ? null : _mapper.Map<SupplierDto>(supplier);
        }

        public async Task<List<KeyValuePair<string, double>>> GetSupplierStatisticsAsync(int userId)
        {
            var suppliers = await _supplierRepository.GetAllAsync();
            var statistics = suppliers
                .Where(s => s.UserID == userId) // Assuming Supplier has a UserId property
                .GroupBy(s => s.ContactInfo) // Assuming Supplier has a Category property
                .Select(g => new KeyValuePair<string, double>(g.Key, g.Count()))
                .ToList();

            return statistics;
        }
    }


}
