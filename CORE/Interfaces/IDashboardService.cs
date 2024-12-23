using InventorySystem.CORE.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static InventorySystem.CORE.Services.DashboardService;

namespace InventorySystem.CORE.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardDataAsync(string userRole, int? userId = null);
    }
}
