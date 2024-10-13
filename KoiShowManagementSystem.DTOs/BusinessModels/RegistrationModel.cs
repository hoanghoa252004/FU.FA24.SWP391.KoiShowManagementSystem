using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class RegistrationModel
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
        public string? Image1 { get; set; }
        public string? Image2 { get; set; }
        public string? Image3 { get; set; }
        public string? Video { get; set; }
        public int? KoiID { get; set; }
        public string? GroupName { get; set; }
        public bool? IsPaid { get; set; }
    }
}
