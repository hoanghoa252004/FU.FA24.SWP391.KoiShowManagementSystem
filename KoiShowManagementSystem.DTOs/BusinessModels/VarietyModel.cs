using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class VarietyModel
    {
        public int VarietyId { get; set; }
        public string? VarietyName { get; set; } = null;

        public string? VarietyOrigin { get; set; }

        public bool VarietyStatus { get; set; }

        public string? VarietyDescription { get; set; }


    }
}
