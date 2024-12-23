
namespace InventorySystem.DATABASE.CodeFirst.Entities
{
    public class Stock
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
    public enum StockType
    {
        Supplier,
        Warehouse
    }

}
