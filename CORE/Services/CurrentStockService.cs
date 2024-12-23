using AutoMapper;
using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DATABASE.Repositories;
using InventorySystem.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.CORE.Services
{
    public class CurrentStockService:ICurrentStockService
    {
        private readonly IMapper _mapper;
        private readonly CurrentStockRepository _currentStockRepository;

        public CurrentStockService(CurrentStockRepository currentStockRepository, IMapper mapper)
        {
            _currentStockRepository = currentStockRepository;
            _mapper = mapper;
        }

        public async Task AddAsync(StockDto stockDto)
        {
            //await _warehouseRepository.AddAsync(warehouseDto);
            /*var warehouseExists = await _stockRepository.FindFirstAsync(w => w.WarehouseName == warehouseDto.WarehouseName);
            if (warehouseExists != null)
            {
                //return null;
            }*/
            var stock = _mapper.Map<CurrentStock>(stockDto);
            await _currentStockRepository.AddAsync(stock);
        }

        public async Task DeleteAsync(int id)
        {
            var stock = await _currentStockRepository.GetByIdAsync(id);
            if (stock != null)
            {
                await _currentStockRepository.DeleteAsync(stock);
            }
        }

        public async Task<IEnumerable<StockDto>> GetAllAsync()
        {
            //return await _warehouseRepository.GetAllAsync();
            var stock = await _currentStockRepository.GetAllAsync();
            var mappedStock = _mapper.Map<IEnumerable<StockDto>>(stock);
            return mappedStock;
        }

        public async Task<StockDto> GetByIdAsync(int id)
        {
            var stock = await _currentStockRepository.GetByIdAsync(id);
            if (stock == null)
            {
                return null;
            }
            var mappedStock = _mapper.Map<StockDto>(stock);
            return mappedStock;
        }

        public async Task UpdateAsync(StockDto stockDto)
        {
            //await _warehouseRepository.UpdateAsync(warehouse);
            var stock = await _currentStockRepository.GetByIdAsync(stockDto.ID);
            if (stock == null)
            {

            }

            _mapper.Map(stockDto, stock);
            await _currentStockRepository.UpdateAsync(stock);

        }
    }
}
