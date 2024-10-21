using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

        private static string GetContent(string data)
        {
            return Regex.Replace(data, "Ma giao dich .*", "");
        }

        //public async Task<bool> ProcessPaymentWebhookAsync(PaymentWebhookDto paymentData)
        //{
        //    string content = GetContent(paymentData.Content);
        //    bool isAllUpdated = false; //gán cờ để cho nó loop xong rồi mới lưu

        //    if (content.StartsWith("KoiShowReg"))
        //    {
        //        //ví dụ content là "KoiShowReg:11,12" có id của đơn là 11 12
        //        string registrationIdsString = content.Replace("KoiShowReg", "");
        //        var registrationIds = registrationIdsString.Split(' ');

        //        foreach (var registrationId in registrationIds)
        //        {
        //            if (int.TryParse(registrationId, out int id))
        //            {
        //                var registration = await _context.Registrations.FindAsync(id);
        //                if (registration != null)
        //                {
        //                    registration.IsPaid = true;
        //                    registration.PaymentReferenceCode = paymentData.ReferenceCode;
        //                    isAllUpdated = true; //xét flag để nào foreach hết rồi mới cập nhật một lượt
        //                }
        //            }
        //        }

        //        if (isAllUpdated)
        //        {
        //            await _context.SaveChangesAsync();
        //        }
        //    }

        //    return isAllUpdated;
        //}
        public async Task<bool> ProcessPaymentWebhookAsync(PaymentWebhookDto paymentData)
        {
            string content = GetContent(paymentData.Content);
            bool isAllUpdated = false; // Flag to indicate if updates were made

            if (content.StartsWith("KoiShowReg"))
            {
                // Example content: "KoiShowReg 11 12 " where 11 and 12 are registration IDs
                string registrationIdsString = content.Replace("KoiShowReg", "");
                var registrationIds = registrationIdsString.Split(' ');

                foreach (var registrationId in registrationIds)
                {
                    if (int.TryParse(registrationId.Trim(), out int id))
                    {
                        var registration = await _context.Registrations.FindAsync(id);
                        if (registration != null)
                        {
                            registration.IsPaid = true;
                            registration.PaymentReferenceCode = paymentData.ReferenceCode;

                            // Use repository method to classify and get the groupId
                            var groupId = await ClassifyRegistrationAsync((int)registration.KoiId!, (decimal)registration.Size!, (int)registration.ShowId!);
                            if (groupId != null)
                            {
                                registration.GroupId = groupId; // Update the RegistrationModel with the GroupId
                            }

                            isAllUpdated = true; // Set flag to true since an update was made
                        }
                    }
                }

                if (isAllUpdated)
                {
                    await _context.SaveChangesAsync(); // Save all changes at once
                }
            }

            return isAllUpdated;
        }


        private async Task<int?> ClassifyRegistrationAsync(int koiId, decimal size, int showId)
        {
            int? groupId = null;

            // Retrieve the koi data
            var koi = await _context.Kois.FindAsync(koiId);
            if (koi != null)
            {
                // Get groups for the given show
                var groups = await _context.Groups
                    .Where(g => g.ShowId == showId)
                    .ToListAsync();

                // Classify the koi into a group based on size and variety
                foreach (var group in groups)
                {
                    if (size >= group.SizeMin && size <= group.SizeMax) // Check size range
                    {
                        var matchingVariety = group.Varieties!.FirstOrDefault(v => v.Id == koi.VarietyId);
                        if (matchingVariety != null) // Check variety match
                        {
                            groupId = group.Id;
                            break;
                        }
                    }
                }
            }

            return groupId;
        }



    }
}
