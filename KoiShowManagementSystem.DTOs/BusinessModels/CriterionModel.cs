using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class CriterionModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? Percentage { get; set; }
        public string? Description { get; set; }
        public decimal? Score1 { get; set; }
    }
}
