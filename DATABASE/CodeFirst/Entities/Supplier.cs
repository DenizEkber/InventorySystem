
namespace InventorySystem.DATABASE.CodeFirst.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactInfo { get; set; }
        public string SupplierAddress { get; set; }

        // User Relationship
        public int UserID { get; set; }
        public User User { get; set; } // Navigation property

    }
}
