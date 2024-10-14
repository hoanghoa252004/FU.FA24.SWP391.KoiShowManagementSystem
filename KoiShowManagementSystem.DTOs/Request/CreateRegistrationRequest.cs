using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.Request
{
    public class CreateRegistrationRequest
    {
        [Required]
        public int? ShowId { get; set; }
        [Required]
        public int? KoiId { get; set; }
        [Required]
        public decimal? Size { get; set; }
        [Required]
        public IFormFile? Image1 { get; set; }
        [Required]
        public IFormFile? Image2 { get; set; }
        [Required]
        public IFormFile? Image3 { get; set; }
        [Required]
        public string? Video { get; set; }
        [Required]
        public string? Description { get; set; }
    }
}
