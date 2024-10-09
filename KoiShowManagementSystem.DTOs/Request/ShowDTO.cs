using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.Request
{
    public class ShowDTO
    {
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public DateOnly RegisterStartDate { get; set; }
        [Required]
        public DateOnly RegisterEndDate { get; set; }
        [Required]
        public DateOnly ScoreStartDate { get; set; }
        [Required]
        public DateOnly ScoreEndDate { get; set; }
        [Required]
        public IFormFile Banner { get; set; }
        public List<GroupDTO>? Groups { get; set; }
    }
}
