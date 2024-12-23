using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventorySystem.DTO.Models
{
    public class DashboardStatisticsDto
    {
        public int TotalProducts { get; set; }
        public int TotalStockQuantity { get; set; }
        public decimal TotalStockValue { get; set; }
        public List<KeyValuePair<string, double>> CategoryDistribution { get; set; }
        public List<KeyValuePair<string, double>> StockDistribution { get; set; }
    }

}
