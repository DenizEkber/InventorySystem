
namespace InventorySystem.DATABASE.CodeFirst.Entities
{
    public class Warehouse
    {
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public bool IsRefrigerated { get; set; }
        public string ContactInfo { get; set; }
        public string Address { get; set; }

        // User Relationship
        public int UserID { get; set; }
        public User User { get; set; } // Navigation property

        // Navigation Property
        //public ICollection<Location> Locations { get; set; }
    }

}
