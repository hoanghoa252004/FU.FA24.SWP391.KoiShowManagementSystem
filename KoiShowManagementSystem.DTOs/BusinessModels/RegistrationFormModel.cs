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
        // Lấy lên cho đăng kí:
        public int? ShowId { get; set; }
        public string? ShowName { get; set; }
        public List<VarietyModel>? VarietyList { get; set; }
        public List<GroupModel>? SizeList { get; set; }
        // Lấy về để tạo đơn:
        public int? KoiId { get; set; }
        public decimal Size {  get; set; }
        public IFormFile Image1 { get; set; }
        public IFormFile? Image2 { get; set;}
        public IFormFile? Image3 { get; set; }
        public string? Video { get; set; }
    }
}
