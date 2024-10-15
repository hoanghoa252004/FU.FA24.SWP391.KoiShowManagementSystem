using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly KoiShowManagementSystemContext _context;
        public PaymentRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        public async Task<bool> ProcessPaymentWebhookAsync(PaymentWebhookDto paymentData)
        {
            string content = paymentData.Content; 
            bool isAllUpdated = false; //gán cờ để cho nó loop xong rồi mới lưu

            if (content.StartsWith("KoiShowReg:"))
            {
                //ví dụ content là "KoiShowReg:11,12" có id của đơn là 11 12
                string registrationIdsString = content.Replace("KoiShowReg:", "");
                var registrationIds = registrationIdsString.Split(',');

                foreach (var registrationId in registrationIds)
                {
                    if (int.TryParse(registrationId, out int id))
                    {
                        var registration = await _context.Registrations.FindAsync(id);
                        if (registration != null)
                        {
                            registration.IsPaid = true;
                            registration.PaymentReferenceCode = paymentData.ReferenceCode;
                            isAllUpdated = true; //xét flag để nào foreach hết rồi mới cập nhật một lượt
                        }
                    }
                }

                if (isAllUpdated)
                {
                    await _context.SaveChangesAsync();
                }
            }

            return isAllUpdated;
        }


    }
}
