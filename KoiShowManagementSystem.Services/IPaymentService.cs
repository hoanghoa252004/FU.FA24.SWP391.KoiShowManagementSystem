using KoiShowManagementSystem.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IPaymentService
    {
        Task<bool> ProcessPaymentWebhookAsync(PaymentWebhookDto paymentData);
    }
}
