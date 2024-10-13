using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Repositories.Helper;
using KoiShowManagementSystem.Services.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly Repository _repository;
        private readonly JwtServices _jwtServices;
        public RegistrationService(JwtServices jwtServices, Repository repository)
        {
            _jwtServices = jwtServices;
            _repository = repository;
        }

        // 1. GET MY REGISTRATION:
        // ACTOR: MEMBER.
        public async Task<IEnumerable<RegistrationModel>> GetMyRegistration(string status)
        {
            IEnumerable<RegistrationModel> result= null!;
            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            IEnumerable<RegistrationModel> myKoiRegistrations = await _repository.Registrations.GetRegistrationByUserIdAsync(id);
            status = status.ToLower();
            switch (status)
            {
                case "inprocess":
                    {
                        result = myKoiRegistrations.Where(koi => koi.Status == "Accepted" || koi.Status == "Rejected" || koi.Status == "Pending").ToList();
                        break;
                    }
                case "draft":
                    {
                        result = myKoiRegistrations.Where(koi => koi.Status == "Draft").ToList();
                        break;
                    }
                case "scored":
                    {
                        result = myKoiRegistrations.Where(koi => koi.Status == "Scored").ToList();
                        break;
                    }
            }
            return result;
        }

        // 2. CREATE REGISTRATION:
        // ACTOR: MEMBER.
        public async Task CreateRegistration(RegistrationFormModel dto)
        {
            // VALIDATE INPUT:
            // V1: Kiểm tra có đủ input hay ko:
            if (dto == null
                || dto.Image1 == null
                || dto.Image2 == null
                || dto.Image3 == null
                || dto.Video.IsNullOrEmpty() == true)
                throw new Exception("Lack of information to register Koi for show !");
            // V2: Kiểm tra con cá đó có đúng phải của Member đó ko:
            var userId = _jwtServices.GetIdAndRoleFromToken().userId;
            var koi = await _repository.Koi.GetKoi((int)dto.KoiId!);
            if (koi != null && userId != koi.UserId)
                throw new Exception("You're registering a Koi that does not belong to you !");
            // START: 
            var groups = await _repository.Groups.GetByShowId((int)dto.ShowId!);
            if (!groups.IsNullOrEmpty())
            {
                foreach (var group in groups)
                {
                    var registrations = group.Registrations;
                    // 1. Con đó đăng kí cuộc thi này rồi hay chưa.
                    // Nếu group có đơn đăng kí rồi thì kiểm tra
                    // xem con cá đó đã đăng kí chưa. Nếu rồi -> biến.
                    // Nếu group chưa có đơn nào, bỏ qua.
                    // CHƯA GIẢI QUYẾT:
                    // Nếu lần trước đăng kí, vì hệ thống phân loại ko được và staff chưa duyệt
                    // Nên groupId bằng null => sẽ ko bắt được đã tham gia show.
                    if (!registrations.IsNullOrEmpty())
                        foreach (var regist in registrations!)
                        {
                            if (regist.KoiID == dto.KoiId)
                                throw new Exception("Your Koi already registered for this Show");
                        }
                    // 2. Thực hiện phân loại: 
                    if (dto.Size >= group.SizeMin && dto.Size <= group.SizeMax)// Kiểm tra size:
                    {
                        var matchingVariety = group.Varieties!.FirstOrDefault(var => var.VarietyId == koi!.VarietyId);
                        if (matchingVariety != null) // Kiểm tra variety:
                        {
                            dto.GroupId = group.GroupId;
                            break;
                        }
                    }
                }
                await _repository.Registrations.CreateRegistrationAsync(dto);
            }
        }

        // 3. GET REGISTRATIONS BY SHOW:
        // ACTOR: ALL
        public async Task<(int TotalItems, IEnumerable<RegistrationModel> Registrations)> GetRegistrationByShow(int pageIndex, int pageSize, int showId)
        {
            var registrationList = await _repository.Registrations.GetRegistrationByShowAsync(showId);
            var list = registrationList.Where(regist => regist.Status!.Equals("Accepted", StringComparison.OrdinalIgnoreCase)).ToList();
            var count = list.Count();
            var result = list.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return (count, result);
        }

        // 4. GET REGISTRATION BY ID:
        public async Task<RegistrationModel?> GetRegistration(int registrationId)
        {
            var result = await _repository.Registrations.GetRegistrationAsync(registrationId);
            return result!;
        }

        // 5. GET PENDING REGISTRATION:
        public async Task<(int TotalItems, IEnumerable<RegistrationModel> Registrations)> GetPendingRegistration(int pageIndex, int pageSize, int showId)
        {
            var registrationList = await _repository.Registrations.GetAllRegistrationAsync();
            if (registrationList.Count() > 0)
            {
                var list = registrationList.Where(regist => regist.Status!.Equals("Pending", StringComparison.OrdinalIgnoreCase) && regist.IsPaid == true);
                var count = list.Count();
                var result = list.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                return (count, result);
            }
            else
                throw new Exception("Show's contained any registration yet.");
        }

        // 6. UPDATE REGISTRATION:
        public async Task UpdateRegistration(RegistrationFormModel dto)
        {
            if (dto == null)
                throw new Exception("Update registration has nothing");
            else
                await _repository.Registrations.UpdateRegistrationAsync(dto);
        }
    }
}
