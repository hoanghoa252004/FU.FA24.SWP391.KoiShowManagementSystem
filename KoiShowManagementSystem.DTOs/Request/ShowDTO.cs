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
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly RegisterStartDate { get; set; }
        public DateOnly RegisterEndDate { get; set; }
        public DateOnly ScoreStartDate { get; set; }
        public DateOnly ScoreEndDate { get; set; }
        public IFormFile? Banner { get; set; }
        public List<GroupDTO>? Groups { get; set; }
    }
}
