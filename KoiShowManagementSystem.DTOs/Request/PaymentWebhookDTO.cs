using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.DTOs.Request
{
    public class PaymentWebhookDto
    {
        public string Content { get; set; }             // Nội dung chuyển khoản
        public string ReferenceCode { get; set; }       // Mã tham chiếu
    }
}
