using InventorySystem.DATABASE.CodeFirst.Context;
using InventorySystem.DATABASE.CodeFirst.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventorySystem.DATABASE.Repositories
{
    public class CurrentStockRepository : BaseRepository<CurrentStock>
    {
        AppDbContext context;
        public CurrentStockRepository(AppDbContext context) : base(context)
        {
            this.context = context;
        }
        public async Task<List<CurrentStock>> GetAllWithThingsAsync()
        {
            return await _context.CurrentStocks.Include(cs => cs.Product).ToListAsync();
        }
        public IQueryable<CurrentStock> GetAll()
        {
            return _context.CurrentStocks.AsQueryable();
        }
        public IQueryable<CurrentStock> GetAllWithProduct()
        {
            return _context.CurrentStocks.Include(cs => cs.Product).AsQueryable();
        }
    }
}
