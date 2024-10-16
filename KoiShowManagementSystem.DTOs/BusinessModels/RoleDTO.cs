using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class RoleDTO
    {
        public int? Id { get; set; }

        public string? Title { get; set; }
        public bool? Status { get; set; }
    }
}
