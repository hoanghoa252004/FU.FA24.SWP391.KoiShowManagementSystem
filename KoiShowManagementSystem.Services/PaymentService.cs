using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly Repository _repository;
        public PaymentService(Repository repository)
        {
            _repository = repository;
        }

        public async Task<bool> ProcessPaymentWebhookAsync(PaymentWebhookDto paymentData)
        {
            var result = await _repository.Payment.ProcessPaymentWebhookAsync(paymentData);
            return result;
        }
    }
}
