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
    public class StockTransactionService : IStockTransactionService
    {
        private readonly StockTransactionRepository _repository;

        public StockTransactionService(StockTransactionRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<StockTransaction>> GetAllStockTransactionsAsync()
        {
            return (await _repository.GetAllAsync()).ToList();
        }

        public async Task AddStockTransactionAsync(StockTransaction transaction)
        {
            await _repository.AddAsync(transaction);
        }

        public async Task DeleteStockTransactionAsync(int id)
        {
            var transaction = await _repository.GetByIdAsync(id);
            if (transaction != null)
            {
                await _repository.DeleteAsync(transaction);
            }
        }
    }
}
