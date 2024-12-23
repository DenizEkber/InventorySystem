using InventorySystem.DATABASE.CodeFirst.Context;
using InventorySystem.DATABASE.CodeFirst.Entities;
using InventorySystem.DTO.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DATABASE.Repositories
{
    public class ProductRepository : BaseRepository<Product>
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<int> GetProductTransactionCountForTodayAsync(TransactionType transactionType)
        {
            var today = DateTime.Today;
            return await _context.StockTransactions.CountAsync(st =>
                st.TransactionDate >= today &&
                st.TransactionDate < today.AddDays(1) &&
                st.TransactionType == transactionType);
        }
        public async Task<int> GetCountAsync()
        {
            return await _context.Products.CountAsync();
        }

        public IQueryable<Product> GetAll()
        {
            return _context.Products.AsQueryable();
        }

    }
}
