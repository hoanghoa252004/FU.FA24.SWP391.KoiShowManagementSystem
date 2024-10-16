using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class GroupModel
    {
        public int GroupId { get; set; }
        public string? GroupName { get; set; }
        public decimal? SizeMin { get; set; }
        public decimal? SizeMax { get; set; }
        public string? Unit { get; set; }
        public int? Scored { get; set; }
        public int? AmountNotScored { get; set; }
        public List<RegistrationModel>? Registrations { get; set; }
        //public List<KoiModel>? KoiDetailsInShow { get; set; }
        public List<KoiModel>? Kois { get; set; }
        public List<CriterionModel>? Criterion { get; set; }
        public List<VarietyModel>? Varieties { get; set; }
    }
}
