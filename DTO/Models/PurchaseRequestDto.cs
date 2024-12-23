using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DTO.Models
{
    public class PurchaseRequestDto
    {
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public int SupplierID { get; set; }
        public DateTime RequestDate { get; set; }
    }
}
