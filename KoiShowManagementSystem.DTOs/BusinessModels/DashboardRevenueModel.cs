using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class DashboardRevenueModel
    {
        public int ShowId { get; set; }
        public string? ShowTitle { get; set; }
        public DateOnly? StartRegisterDate { get; set; }
        public string? Status { get; set; }
        public decimal? TotalRevenue { get; set; }
    }
}
