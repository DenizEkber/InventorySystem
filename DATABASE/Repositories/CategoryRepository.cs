using InventorySystem.DATABASE.CodeFirst.Context;
using InventorySystem.DATABASE.CodeFirst.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DATABASE.Repositories
{
    public class CategoryRepository : BaseRepository<Category>
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

    }
}
