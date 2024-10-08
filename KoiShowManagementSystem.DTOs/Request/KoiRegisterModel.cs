using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.Request
{
    public class KoiRegisterModel
    {
        public string? KoiName { get; set; }
        public string? Description { get; set; }
        public decimal? Size { get; set; }
        public DateOnly? CreateDate { get; set; }
        public decimal? TotalScore { get; set; }
        public bool? IsPaid { get; set; }
        public int? VarietyId { get; set; }
        public int? GroupId { get; set; }
    }
}
