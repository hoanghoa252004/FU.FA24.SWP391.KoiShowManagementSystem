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

        public async Task<bool> AreAllMemberRegistrationsPaidAsync()
        {
            var userId = _jwtServices.GetIdAndRoleFromToken().userId;
            // Lấy tất cả các đăng ký (registrations) liên quan đến thành viên từ database
            var registrations = await _repository.Registrations.GetRegistrationsByMemberIdAsync(userId);

            // Kiểm tra nếu tất cả các đăng ký đều đã được thanh toán
            return registrations.All(r => r.IsPaid == true);
        }
    }
}
