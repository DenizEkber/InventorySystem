using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DATABASE.CodeFirst.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SKU { get; set; }
        public decimal Price { get; set; }
        public int StockLevel { get; set; }
        public int MinimumStock { get; set; }
        public string Location { get; set; }

        // Navigation properties
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public ICollection<StockTransaction> StockTransactions { get; set; }
    }
}
