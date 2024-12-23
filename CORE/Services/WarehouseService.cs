using AutoMapper;
using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DATABASE.Repositories;
using InventorySystem.DTO.Models;
using System.Linq.Expressions;

namespace InventorySystem.CORE.Services
{
    public class WarehouseService : IWarehouseService
    {
        private readonly IMapper _mapper;
        private readonly WarehouseRepository _warehouseRepository;

        public WarehouseService(WarehouseRepository warehouseRepository, IMapper mapper)
        {
            _warehouseRepository = warehouseRepository;
            _mapper = mapper;
        }

        public async Task AddAsync(WarehouseDto warehouseDto)
        {
            var warehouseExists = await _warehouseRepository.FindFirstAsync(w => w.WarehouseName == warehouseDto.WarehouseName);
            if (warehouseExists != null) return;

            var warehouse = _mapper.Map<Warehouse>(warehouseDto);
            await _warehouseRepository.AddAsync(warehouse);

        }

        public async Task DeleteAsync(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            if (warehouse != null)
            {
                await _warehouseRepository.DeleteAsync(warehouse);
            }
        }

        public async Task<IEnumerable<WarehouseDto>> GetAllAsync()
        {
            var warehouses = await _warehouseRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
        }

        public async Task<WarehouseDto> GetByIdAsync(int id)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(id);
            return warehouse == null ? null : _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task UpdateAsync(WarehouseDto warehouseDto)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(warehouseDto.WarehouseID);
            if (warehouse == null) return;

            _mapper.Map(warehouseDto, warehouse);
            await _warehouseRepository.UpdateAsync(warehouse);
        }

        public async Task<WarehouseDto> FindFirstAsync(Expression<Func<Warehouse, bool>> predicate)
        {
            var warehouse = await _warehouseRepository.FindFirstAsync(predicate);
            return warehouse == null ? null : _mapper.Map<WarehouseDto>(warehouse);
        }

        public async Task<List<KeyValuePair<string, double>>> GetWarehouseStatisticsAsync(int userId)
        {
            var warehouses = await _warehouseRepository.GetAllAsync();
            var statistics = warehouses
                .Where(w => w.UserID == userId) // Assuming Warehouse has a UserId property
                .GroupBy(w => w.ContactInfo) // Assuming Warehouse has a Type property
                .Select(g => new KeyValuePair<string, double>(g.Key, g.Count()))
                .ToList();

            return statistics;
        }
    }
}
