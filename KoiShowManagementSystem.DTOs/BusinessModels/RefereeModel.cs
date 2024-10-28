using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.BusinessModels
{
    public class RefereeModel
    {
        public int Id { get; set; } // UserId
        public int RefereeId { get; set; }
        public string? RefereeName { get; set; }
        public string? ShowTookOnStatus { get; set; } // Dùng để lưu luôn status của show mà nó đảm nhiệm 
                                                        // để check trước khi thực hiện hành động xóa 1 RefereeDetail
                                                        // Vì model này ko dùng để nhận về, nên t thêm field này cho tiện đỡ tạo mới.
    }
}
