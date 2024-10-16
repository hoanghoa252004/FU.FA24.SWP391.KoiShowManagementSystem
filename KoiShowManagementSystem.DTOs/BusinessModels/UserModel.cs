using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class UserModel
    {
        public int? Id { get; set; }
        public string? Name { get; set; }
        public string? Email{ get; set; }
        public string? Role { get; set; }
        public string? Phone { get; set; }
        public string? Token { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Password { get; set; }
        public bool? Status { get; set; }
        public bool? Gender { get; set; }
    }
}
