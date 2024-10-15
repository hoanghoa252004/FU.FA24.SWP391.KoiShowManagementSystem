using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.Request
{
    public class CriteriaDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Percentage { get; set; }
    }
}
