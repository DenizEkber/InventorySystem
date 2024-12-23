using InventorySystem.DATABASE.CodeFirst.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DTO.Models
{
    public class WarehouseDto
    {
        public int WarehouseID { get; set; }
        public string WarehouseName { get; set; }
        public bool IsRefrigerated { get; set; }
        public string ContactInfo { get; set; }
        public string Address { get; set; }
        public int UserID { get; set; }
    }
}
