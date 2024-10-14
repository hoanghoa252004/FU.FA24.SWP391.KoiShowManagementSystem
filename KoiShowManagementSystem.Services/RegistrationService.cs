using Amazon.Runtime.Telemetry;
using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
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
        public async Task CreateRegistration(CreateRegistrationRequest dto)
        {
            if(dto != null)
            {
                // V0: Kiểm tra có đủ input hay ko:
                if (dto.KoiId == null
                    || dto.ShowId == null
                    || dto.Image1 == null
                    || dto.Image2 == null
                    || dto.Image3 == null
                    || dto.Video.IsNullOrEmpty() == true)
                    throw new Exception("Failed: Lack of information to register Koi for show !");
                else
                {
                    bool check = false;
                    var createRegistration = new CreateRegistrationModel()
                    {
                        Image1 = dto.Image1,
                        Image2 = dto.Image2,
                        Image3 = dto.Image3,
                        Video = dto.Video,
                        KoiId = dto.KoiId,
                        Size = dto.Size,
                        GroupId = null!,
                    };
                    // V1: Kiểm tra Show còn Up Comming ko:
                    var show = await _repository.Show.GetShowDetailsAsync((int)dto.ShowId);
                    if (show != null)
                    {
                        if (!show.ShowStatus!.Equals("On Going", StringComparison.OrdinalIgnoreCase))
                            throw new Exception("Failed: Show is not on going !");
                    }
                    else
                        throw new Exception("Failed: Show does not exist !");
                    if (dto.Size <= 0 || dto.Size >= 100)
                        throw new Exception("Failed: Invalid size for Koi Fish !");
                    // V2: Kiểm tra con cá đó có đúng phải của Member đó ko:
                    var userId = _jwtServices.GetIdAndRoleFromToken().userId;
                    var koi = await _repository.Koi.GetKoi((int)dto.KoiId!);
                    if (koi != null)
                    {
                        if (userId != koi.UserId)
                            throw new Exception("Failed: You're registering a Koi that does not belong to you !");
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
                                            throw new Exception("Failed: Your Koi already registered for this Show");
                                    }
                                // 2. Thực hiện phân loại: 
                                if (dto.Size >= group.SizeMin && dto.Size <= group.SizeMax)// Kiểm tra size:
                                {
                                    var matchingVariety = group.Varieties!.FirstOrDefault(var => var.VarietyId == koi!.VarietyId);
                                    if (matchingVariety != null) // Kiểm tra variety:
                                    {
                                        createRegistration.GroupId = group.GroupId;
                                        break;
                                    }
                                }
                            }
                            check = true;
                        }
                        if (check == true)
                            await _repository.Registrations.CreateRegistrationAsync(createRegistration);
                    }
                    else
                        throw new Exception("Failed: Koi does not exist !");
                }
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
        public async Task<(int TotalItems, IEnumerable<RegistrationModel> Registrations)> GetPendingRegistration(int pageIndex, int pageSize)
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
        public async Task UpdateRegistration(UpdateRegistrationModel dto)
        {
            if (dto == null || 
                (dto.Status.IsNullOrEmpty() == true
                && dto.Size == null
                && dto.GroupId == null
                && dto.Image3 == null
                && dto.Image2 == null
                && dto.Image1 == null
                && dto.Video == null
                && dto.KoiId == null)
                )
                throw new Exception("Failed: Nothing To Update");
            else
            {
                string role = _jwtServices.GetIdAndRoleFromToken().role;
                int memberId = _jwtServices.GetIdAndRoleFromToken().userId;
                bool check = false;
                if (role == null)
                    throw new Exception("Failed: Invalid actor !");
                else
                {
                    if(role.Equals("Member", StringComparison.OrdinalIgnoreCase))
                    {
                        // Kiểm tra xem đơn này phải của nó ko:
                        var kois =  await _repository.Koi.GetAllKoiByUserId(memberId);
                        var registrations = (await _repository.Registrations.GetRegistrationByUserIdAsync(memberId)).ToList();
                        var isBelong = from k in kois
                                       join r in registrations on k.KoiID equals r.KoiID
                                       //where r.Id == dto.Id
                                       select r;
                        int count = isBelong.Count();
                        if (count <= 0)
                            throw new Exception("Failed: This is not a registration of your Koi !");
                        // Kiểm tra quyền hạn được update:
                        if (dto.Status != null || dto.GroupId != null)
                           throw new Exception("Failed: Member does not have permission to update Status & Group of Registration !");
                        else if (dto.KoiId != null) // Nếu có cập nhập con mới:
                        {
                            // Kiểm tra new Koi phải của Member ko:
                            var koi = await _repository.Koi.GetKoi((int)dto.KoiId);
                            if (koi != null)
                            {
                                if (koi.UserId != memberId)
                                    throw new Exception("Failed: New koi does not belong to you !");
                                else
                                    check = true;
                            }
                        }
                        else // Cập nhập con cũ
                        {
                            check = true;
                        }
                    }
                    else if (role.Equals("Staff", StringComparison.OrdinalIgnoreCase))
                    {
                        if (dto.KoiId != null
                            || dto.Size != null
                            || dto.Image1 != null
                            || dto.Image2 != null
                            || dto.Image3 != null
                            || dto.Video != null)
                            throw new Exception("Staff does not have permission to update Koi, Size, Image & Video of Registration !");
                        else
                            check = true;
                    }
                }  
                // Sau khi vượt qua tất cả ràng buộc: 
                if(check == true)
                    await _repository.Registrations.UpdateRegistrationAsync(dto);
            }
        }
    }
}
