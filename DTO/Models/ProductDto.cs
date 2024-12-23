using InventorySystem.DATABASE.CodeFirst.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DTO.Models
{
    public class ProductDto
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
        public int CategoryId { get; set; }
    }
}
