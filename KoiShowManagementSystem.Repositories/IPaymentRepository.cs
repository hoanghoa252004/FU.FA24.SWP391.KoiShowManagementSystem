using KoiShowManagementSystem.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IPaymentRepository
    {
        Task<bool> ProcessPaymentWebhookAsync(PaymentWebhookDto paymentData);
        Task<bool> CheckIfPaymentIsCompleteAsync(int registrationId);
    }
}
