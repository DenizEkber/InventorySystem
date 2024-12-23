using System.Collections.Generic;

namespace InventorySystem.DATABASE.CodeFirst.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; }
    }
}
