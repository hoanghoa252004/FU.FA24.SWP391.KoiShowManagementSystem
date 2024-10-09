using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.Request
{
    public class GroupDTO
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public decimal MinSize { get; set; }
        [Required]
        public decimal MaxSize { get; set; }
        [Required]
        public HashSet<int>? Varieties { get; set; }
        [Required]
        public List<CriteriaDTO>? Criterias { get; set; }
    }
}
