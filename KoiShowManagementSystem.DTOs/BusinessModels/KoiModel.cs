using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class KoiModel
    {
        public int KoiID { get; set; }
        public string? KoiName { get; set; }
        public int? Rank { get; set; }
        public string? KoiImg { get; set; }
        //public string? KoiVideo { get; set; }
        public string? KoiVariety { get; set; }
        public string? KoiDesc { get; set; }
        public decimal? KoiSize { get; set; }
        //public decimal? TotalScore { get; set; }
        //public string? RegistrationStatus { get; set; }
        public bool?  KoiStatus { get; set; }
        //public bool? IsBestVote { get; set; }
        //public bool? IsPaid { get; set; }
        //public string? GroupName { get; set; }
        public int RegistrationId { get; set; }
        public bool? IsBestVote { get; set; }
        public string? Image1 { get; set; }
        public string? Image2 { get; set; }
        public string? Image3 { get; set; }
        public string? Video { get; set; }
        public List<RegistrationModel>? registrations { get; set; }
        public List<CriterionModel>? criterions { get; set; }
        public int? VarietyId { get; set; }
        public int? UserId { get; set; }
    }
}
