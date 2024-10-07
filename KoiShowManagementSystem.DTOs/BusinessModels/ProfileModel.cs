using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class ProfileModel
    {

        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Role { get; set; }
        public string Name { get; set; } = null!;
        public bool? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
    }
}
