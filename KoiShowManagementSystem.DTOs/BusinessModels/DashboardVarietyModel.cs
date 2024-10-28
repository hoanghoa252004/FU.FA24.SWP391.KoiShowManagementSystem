using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class DashboardVarietyModel
    {
        public int VarietyId { get; set; }
        public string? VarietyName { get; set; } = null;
        public int Quantity { get; set; } = 0;
    }
}
