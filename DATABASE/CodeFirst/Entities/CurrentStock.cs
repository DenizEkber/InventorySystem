using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DATABASE.CodeFirst.Entities
{
    public class CurrentStock
    {
        public int ID { get; set; }

        // Foreign Key
        public int ProductID { get; set; }
        public Product Product { get; set; }

        public int SupplierOrdWarehouseId { get; set; }
        public string SKU { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public StockType StockType { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
