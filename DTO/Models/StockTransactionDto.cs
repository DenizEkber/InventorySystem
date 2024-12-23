using InventorySystem.DATABASE.CodeFirst.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DTO.Models
{
    public class StockTransactionDto
    {
        public int ID { get; set; } 
        public int StockID { get; set; }
        public int From { get; set; } 
        public int To { get; set; }

        public int Quantity { get; set; } 

        public DateTime TransactionDate { get; set; } 
        public TransactionType TransactionType { get; set; }
    }
}
