using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class RegistrationFormModel
    {
        public int? ShowId { get; set; }
        public string? ShowName { get; set; }
        public List<VarietyModel>? VarietyList { get; set; }
        public List<GroupModel>? SizeList { get; set; } 
    }
}
