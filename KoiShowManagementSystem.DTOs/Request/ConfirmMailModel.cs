using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.Request
{
    public class ConfirmMailModel
    {
        public string? Token { get; set; }
        public string? Email { get; set; }
    }
}
