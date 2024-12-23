using System.Collections.Generic;

namespace InventorySystem.DATABASE.CodeFirst.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string BarCode { get; set; }
        public string ProductDescription { get; set; }
        public decimal PackedWeight { get; set; }
        public decimal PackedWidth { get; set; }
        public decimal PackedHeight { get; set; }
        public decimal PackedDepth { get; set; }
        public bool Refrigerated { get; set; }

        // Navigation properties
        public int CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
