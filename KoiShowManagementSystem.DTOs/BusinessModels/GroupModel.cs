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
        public List<KoiModel>? KoiDetails { get; set; }
    }
}
