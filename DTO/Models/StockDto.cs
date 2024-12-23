using InventorySystem.DATABASE.CodeFirst.Entities;

namespace InventorySystem.DTO.Models
{
    public class StockDto
    {
        public int ID { get; set; }
        public int ProductID { get; set; }
        public int SupplierOrdWarehouseId { get; set; }
        public string SKU { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public StockType StockType { get; set; }

        public DateTime LastUpdated { get; set; }
    }
}
