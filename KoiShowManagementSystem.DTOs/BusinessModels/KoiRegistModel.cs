using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class KoiRegistModel
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public decimal? Size { get; set; }
        public string? Variety { get; set; }
        public string? Show { get; set; }
        public int? ShowId { get; set; }
        public string? Group { get; set; }
        public DateOnly? CreateDate { get; set; }
        public int? Rank { get; set; }
        public decimal? TotalScore { get; set; }
        public string? Status { get; set; }
        public bool? IsBestVote { get; set; }
        public string? ImageUrl { get; set; }
        public string? VideoUrl { get; set; }
    }
}
