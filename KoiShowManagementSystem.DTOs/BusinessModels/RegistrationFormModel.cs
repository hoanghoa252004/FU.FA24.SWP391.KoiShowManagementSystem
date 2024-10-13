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
        public int Id { get; set; }
        public int? ShowId { get; set; }
        public int? GroupId { get; set; } // Client gửi về null.
        public int? KoiId { get; set; }
        public decimal? Size {  get; set; }
        public IFormFile? Image1 { get; set; }
        public IFormFile? Image2 { get; set;}
        public IFormFile? Image3 { get; set; }
        public string? Video { get; set; }
    }
}
