using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
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
        private readonly JwtServices _jwtServices;
        public PaymentService(Repository repository, JwtServices jwtServices)
        {
            _repository = repository;
            _jwtServices = jwtServices;
        }

        public async Task<bool> ProcessPaymentWebhookAsync(PaymentWebhookDto paymentData)
        {
            var result = await _repository.Payment.ProcessPaymentWebhookAsync(paymentData);
            return result;
        }

        public async Task<bool> AreAllRegistrationsPaidAsync(string content)
        {
            var result = await _repository.Payment.CheckIfPaymentIsCompleteAsync(content);
            return result;
        }
    }
}
