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
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal MinSize { get; set; }
        public decimal MaxSize { get; set; }
        public List<int>? Varieties { get; set; }
        public List<CriteriaDTO>? Criterias { get; set; }
        public int ShowId { get; set; }
    }
}
