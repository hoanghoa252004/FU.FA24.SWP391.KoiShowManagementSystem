using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class CriterionModel
    {
        public int CriterionId { get; set; }
        public string? CriterionName { get; set; }
        public decimal Percentage { get; set; }
        public string? Description { get; set; }
    }
}
