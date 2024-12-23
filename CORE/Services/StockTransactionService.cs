using AutoMapper;
using InventorySystem.CORE.Interfaces;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DATABASE.Repositories;
using InventorySystem.DTO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.CORE.Services
{
    public class StockTransactionService : IStockTransactionService
    {
        private readonly IMapper _mapper;
        private readonly StockTransactionRepository _stockTransactionRepository;

        public StockTransactionService(StockTransactionRepository stockTransactionRepository, IMapper mapper)
        {
            _stockTransactionRepository = stockTransactionRepository;
            _mapper = mapper;
        }

        public async Task AddAsync(StockTransactionDto stockTransactionDto)
        {
            //await _warehouseRepository.AddAsync(warehouseDto);
            var stockTransaction = _mapper.Map<StockTransaction>(stockTransactionDto);
            await _stockTransactionRepository.AddAsync(stockTransaction);
        }

        public async Task DeleteAsync(int id)
        {
            var stockTransaction = await _stockTransactionRepository.GetByIdAsync(id);
            if (stockTransaction != null)
            {
                await _stockTransactionRepository.DeleteAsync(stockTransaction);
            }
        }

        public async Task<IEnumerable<StockTransactionDto>> GetAllAsync()
        {
            //return await _warehouseRepository.GetAllAsync();
            var stockTransaction = await _stockTransactionRepository.GetAllAsync();
            var mappedStockTransaction = _mapper.Map<IEnumerable<StockTransactionDto>>(stockTransaction);
            return mappedStockTransaction;
        }

        public async Task<StockTransactionDto> GetByIdAsync(int id)
        {
            var stockTransaction = await _stockTransactionRepository.GetByIdAsync(id);
            if (stockTransaction == null)
            {
                return null;
            }
            var mappedStockTransaction = _mapper.Map<StockTransactionDto>(stockTransaction);
            return mappedStockTransaction;
        }

        public async Task UpdateAsync(StockTransactionDto stockTransactionDto)
        {
            //await _warehouseRepository.UpdateAsync(warehouse);
            var stockTransaction = await _stockTransactionRepository.GetByIdAsync(stockTransactionDto.ID);
            if (stockTransaction == null)
            {

            }

            _mapper.Map(stockTransactionDto, stockTransaction);
            await _stockTransactionRepository.UpdateAsync(stockTransaction);

        }
        public async Task<StockTransactionDto> FindFirstAsync(Expression<Func<StockTransaction, bool>> predicate)
        {
            var stockTransaction = await _stockTransactionRepository.FindFirstAsync(predicate);
            if (stockTransaction == null)
            {

            }
            var stockTransactionDto = _mapper.Map<StockTransactionDto>(stockTransaction);
            return stockTransactionDto;
        }
    }
}
