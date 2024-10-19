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
        private readonly IEmailService _emailService;
        private const string WEBSITE_LOGO = "https://koi-shows-image.s3.ap-southeast-1.amazonaws.com/logo/logo.jpg";
        public RegistrationService(JwtServices jwtServices, Repository repository, IEmailService emailService)
        {
            _jwtServices = jwtServices;
            _repository = repository;
            _emailService = emailService;
        }

        // 1. GET MY REGISTRATION:
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
                case "draft":
                    {
                        result = myKoiRegistrations.Where(koi => koi.Status == "Draft").ToList();
                        break;
                    }
            }
            return result;
        }

        // 2. CREATE REGISTRATION:
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
                    || dto.Video.IsNullOrEmpty() == true
                    || dto.Description.IsNullOrEmpty() == true)
                    throw new Exception("Failed: Lack of information to register Koi for show !");
                else
                {
                    var createRegistration = new CreateRegistrationModel()
                    {
                        Image1 = dto.Image1,
                        Image2 = dto.Image2,
                        Image3 = dto.Image3,
                        Video = dto.Video,
                        KoiId = dto.KoiId,
                        Size = dto.Size,
                        Description = dto.Description,
                        GroupId = null!,
                        ShowId = dto.ShowId,
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
                    var koi = await _repository.Koi.GetKoiAsync((int)dto.KoiId!);
                    if (koi != null)
                    {
                        if (userId != koi.UserId)
                            throw new Exception("Failed: You're registering a Koi that does not belong to you !");
                        // V3: Con đó đã thi show đó chưa:
                        var registInShow = await _repository.Registrations.GetRegistrationByShowAsync((int)dto.ShowId);
                        var checkKoiInShow = registInShow.Where(r => r.KoiID == dto.KoiId).ToList();
                        if(checkKoiInShow.Any() == true)
                            throw new Exception("Failed: Your Koi already registered for this Show");
                        else
                            await _repository.Registrations.CreateRegistrationAsync(createRegistration);
                    }
                    else
                        throw new Exception("Failed: Koi does not exist !");
                }
            }
        }

        // SUPPORT METHOD 01:
        private async Task<int?> ClassifyRegistration(RegistrationModel dto)
        {
            int? groupId = null!;
            // Lấy con Koi:
            var koi = await _repository.Koi.GetKoiAsync((int)dto.KoiID!);
            if (koi != null)
            {
                var groups = await _repository.Groups.GetByShowIdAsync((int)dto.ShowId!);
                if (!groups.IsNullOrEmpty())
                {
                    foreach (var group in groups)
                    {
                        if (dto.Size >= group.SizeMin && dto.Size <= group.SizeMax)// Kiểm tra size:
                        {
                            var matchingVariety = group.Varieties!.FirstOrDefault(var => var.VarietyId == koi!.VarietyId);
                            if (matchingVariety != null) // Kiểm tra variety:
                            {
                                groupId = group.GroupId;
                                break;
                            }
                        }
                    }
                }
            }
            return groupId;
        }

        // 3. GET REGISTRATIONS BY SHOW:
        public async Task<(int TotalItems, IEnumerable<RegistrationModel> Registrations)> GetRegistrationByShow(int pageIndex, int pageSize, int showId)
        {
            var registrationList = await _repository.Registrations.GetRegistrationByShowAsync(showId);
            var list = registrationList.Where(regist => regist.Status!.Equals("Accepted", StringComparison.OrdinalIgnoreCase)).ToList();
            var count = list.Count();
            var result = list.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return (count, result);
        }

        // 4. GET REGISTRATION BY ID:
        public async Task<RegistrationModel?> GetRegistrationById(int registrationId)
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
                && dto.KoiId == null
                && dto.Note == null
                && dto.Description == null)
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
                        var kois =  await _repository.Koi.GetAllKoiByUserIdAsync(memberId);
                        var registrations = (await _repository.Registrations.GetRegistrationByUserIdAsync(memberId)).ToList();
                        var isBelong = from k in kois
                                       join r in registrations on k.KoiID equals r.KoiID
                                       //where r.Id == dto.Id
                                       select r;
                        int count = isBelong.Count();
                        if (count <= 0)
                            throw new Exception("Failed: This is not a registration of your Koi !");
                        // Kiểm tra quyền hạn được update:
                        if (dto.Status != null || dto.GroupId != null || dto.Note != null)
                           throw new Exception("Failed: Member does not have permission to update Status,Note, Group of Registration !");
                        else if (dto.KoiId != null) // Nếu có cập nhập con mới:
                        {
                            // Kiểm tra new Koi phải của Member ko:
                            var koi = await _repository.Koi.GetKoiAsync((int)dto.KoiId);
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
                            || dto.Video != null
                            || dto.Description != null)
                            throw new Exception("Staff does not have permission to update Koi, Size, Image & Video of Registration !");
                        else
                            check = true;
                    }
                }  
                // Sau khi vượt qua tất cả ràng buộc: 
                if(check == true)
                {
                    RegistrationModel result = await _repository.Registrations.UpdateRegistrationAsync(dto);
                    if(result != null && result.Status!.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
                    {
                        var member = await _repository.Users.GetUserById(memberId);
                        string subject = $"[{result.Show?.ToUpper()}] REGISTER KOI FOR SHOW SUCCESSFULLY !";
                        string content = $@"
                                        <!DOCTYPE html>
                                        <html lang='en'>
                                        <head>
                                            <meta charset='UTF-8'>
                                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                                            <title>Registration Information</title>
                                        </head>
                                        <body style='font-family: Arial, sans-serif;'>
                                            <p>Dear {member?.Name},</p>
                                            <p>We're glad to announce that you have successfully registered your Koi fish for the show with these information:</p>
                                            <table style='width: 700px; border-collapse: collapse; border: 1px solid black; text-align: left;'>
                                                <tr style='background-color: #FFD700; color: black; text-align: center;'>
                                                    <th colspan='4' style='padding: 10px; font-size: 20px;'>REGISTRATION INFORMATION</th>
                                                </tr>
                                                <tr>
                                                    <td style='border: 1px solid black; padding: 5px;'><b>Registration ID:</b></td>
                                                    <td style='border: 1px solid black; padding: 5px;'>{result.Id}</td>
                                                    <td style='border: 1px solid black; padding: 5px;'><b>Create Date:</b></td>
                                                    <td style='border: 1px solid black; padding: 5px;'>{result.CreateDate}</td>
                                                </tr>
                                                <tr>
                                                    <td style='border: 1px solid black; padding: 5px;'><b>Show:</b></td>
                                                    <td style='border: 1px solid black; padding: 5px;'>{result.Show}</td>
                                                    <td style='border: 1px solid black; padding: 5px;'><b>Group:</b></td>
                                                    <td style='border: 1px solid black; padding: 5px;'>{result.Group}</td>
                                                </tr>
                                                <tr>
                                                    <td style='border: 1px solid black; padding: 5px;'><b>Koi ID:</b></td>
                                                    <td style='border: 1px solid black; padding: 5px;'>{result.KoiID ?? 0}</td>
                                                    <td style='border: 1px solid black; padding: 5px;'><b>Koi Name:</b></td>
                                                    <td style='border: 1px solid black; padding: 5px;'>{result.Name ?? "lAY rA nULL"}</td>
                                                </tr>
                                                <tr>
                                                    <td style='border: 1px solid black; padding: 5px;'><b>Variety:</b></td>
                                                    <td style='border: 1px solid black; padding: 5px;'>{result.Variety ?? "lAY rA nuLL"}</td>
                                                    <td style='border: 1px solid black; padding: 5px;'><b>Size:</b></td>
                                                    <td style='border: 1px solid black; padding: 5px;'>{result.Size}</td>
                                                </tr>
                                                <tr>
                                                    <td style='border: 1px solid black; padding: 5px;' colspan='1'><b>Description:</b></td>
                                                    <td style='border: 1px solid black; padding: 5px;' colspan='3'>{result.Description}</td>
                                                </tr>
                                                <tr>
                                                    <td style='border: 1px solid black; padding: 5px;' colspan='1'; rowspan='3': center;'><b>Image:</b></td>
                                                    <td style='border: 1px solid black; padding: 5px;' colspan='3'>
                                                        <img src='{result.Image1}' alt='Image1' style='width: 100%; height: auto; margin-right: 10px;' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='border: 1px solid black; padding: 5px;' colspan='3'>
                                                        <img src='{result.Image2}' alt='Image2' style='width: 100%; height: auto; margin-right: 10px;' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='border: 1px solid black; padding: 5px;' colspan='3'>
                                                        <img src='{result.Image3}' alt='Image3' style='width: 100%; height: auto;' />
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td style='border: 1px solid black; padding: 5px;' colspan='1'><b>Video:</b></td>
                                                    <td style='border: 1px solid black; padding: 5px;' colspan='3'>{result.Video}</td>
                                                </tr>
                                            </table>
                                            <p>Hope your Koi fish will have high results in this show!</p>
                                            <hr style='width: 200px; margin-left: 0;'/>
                                            <div style='text-align: left; '>
                                                <img src='{WEBSITE_LOGO}' alt='Logo' style='max-width: 50px; height: auto;' />
                                                <span style='font-size: 14px; font-weight: bold;'>Koi Show Management System FPTU FA24 SWP391</span><br />
                                                <span>Website: 
                                                    <a href='{"https://github.com/hoanghoa252004/FU.FA24.SWP391.KoiShowManagementSystem"}'>
                                                        {"https://github.com/hoanghoa252004/FU.FA24.SWP391.KoiShowManagementSystem"}
                                                    </a>
                                                </span>
                                            </div>
                                        </body>
                                        </html>";
                        await _emailService.SendEmail(new EmailModel()
                        {
                            To = "hoathse184053@fpt.edu.vn",
                            Subject = subject,
                            Content = content,
                        });
                    }
                }
            }
        }

        // 7. PUBLISH SCORE TO MEMBER:
        public async Task PublishResult(int showId)
        {
            // Check Show whether it exists:
            var show = await _repository.Show.GetShowDetailsAsync(showId);
            if (show == null)
                throw new Exception("Failed: Show does not exit.");
            //Lấy hết đơn của show đó đã scored.
            var list = await _repository.Registrations.GetRegistrationByShowAsync(showId);
            if (list.Any())
            {
                var results = from regis in list
                              where regis.Status!.Equals("Scored",
                                    StringComparison.OrdinalIgnoreCase)
                              select regis;
                if (results.Any())
                {
                    foreach (var result in results)
                    {
                        var koi = await _repository.Koi.GetKoiAsync((int)result?.KoiID!);
                        var member = await _repository.Users.GetUserById((int)koi?.UserId!);
                        string content = @$"
                            <html>
                            <body>
                                <p>Dear {member?.Name},</p>
                                <p>We're glad to announce that your Koi just has result:</p>
                                <table border='1' style='border-collapse: collapse; width: 500px;'>
                                    <tr style='background-color: yellow;'>
                                        <th colspan='4' style='text-align: center; font-size: 18px;'>RESULT</th>
                                    </tr>
                                    <tr>
                                        <td style='width: 25%; padding: 5px;'><b>Registration ID:</b></td>
                                        <td style='width: 25%; padding: 5px; text-align: center;'>{result?.Id}</td>
                                        <td style='width: 25%; padding: 5px;'><b>Group:</b></td>
                                        <td style='width: 25%; padding: 5px; text-align: center;'>{result?.Group}</td>
                                    </tr>
                                    <tr>
                                        <td style='width: 25%; padding: 5px;'><b>Koi ID:</b></td>
                                        <td style='width: 25%; padding: 5px; text-align: center;'>{result?.KoiID}</td>
                                        <td style='width: 25%; padding: 5px;'><b>Koi Name:</b></td>
                                        <td style='width: 25%; padding: 5px; text-align: center;'>{result?.Name}</td>
                                    </tr>
                                    <tr>
                                        <td><b>Image:</b></td>
                                        <td colspan='3' style='text-align: center;'>
                                            <img src='{result?.Image1}' alt='Koi Image' style='max-width: 90%; height: auto;' />
                                        </td>
                                    </tr>
                                    <tr>
                                        <td><b>Total Score:</b></td>
                                        <td colspan='3' style='color: red; text-align: center;'>{result?.TotalScore}</td>
                                    </tr>
                                    <tr>
                                        <td><b>Rank:</b></td>
                                        <td colspan='3' style='color: green; text-align: center;'>{result?.Rank}</td>
                                    </tr>
                                </table>
                                <p>Hope your Koi fish will have high results in this show!</p>
                                <hr style='width: 200px; margin-left: 0;'/>
                                <table style='width: 400px%; border-collapse: collapse;'>
                                    <tr>
                                        <td  style=' width: 10%; text-align: center;'>
                                            <img src='{WEBSITE_LOGO}' alt='Logo' style='max-width: 50px; height: auto;' />
                                        </td>
                                        <td style='width: 75%; padding-left: 10px;'>
                                            <table style='border-collapse: collapse;'>
                                                <tr>
                                                    <td style='font-size: 14px; font-weight: bold;'>Koi Show Management System FPTU FA24 SWP391</td>
                                                </tr>
                                                <tr>
                                                    <td>Website: 
                                                        <a href='https://github.com/hoanghoa252004/FU.FA24.SWP391.KoiShowManagementSystem'>
                                                            https://github.com/hoanghoa252004/FU.FA24.SWP391.KoiShowManagementSystem
                                                        </a>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                    </tr>
                                </table>
                            </body>
                            </html>";
                        string subject = @$"[{show.ShowTitle?.ToUpper()} ANNOUCEMENT RESULT]";
                        await _emailService.SendEmail(new EmailModel()
                        {
                            To = member!.Email,
                            Subject = subject,
                            Content = content,
                        });
                    }
                }
            }
        }
        public async Task VoteRegistration(int registrationId, bool vote)
        {
            var userId = _jwtServices.GetIdAndRoleFromToken().userId;
            var member = await _repository.Users.GetUserById(userId);
            var registration = await _repository.Registrations.GetRegistrationAsync(registrationId);
            if (member != null && registration != null)
            {
                var list =  (await _repository.Registrations.GetRegistrationByUserIdAsync((int)member.Id!))
                    .Where(r => r.Id == registrationId);
                if (list.Any() == true)
                    throw new Exception("Failed: You can not vote for your own Koi !");
                var check = await _repository.Registrations.CheckVote(userId, registrationId);
                if (vote == true) // Muốn Vote:
                {
                    // Check xem đã vote chưa:
                    if (check == true) // Đã vote rồi:
                        throw new Exception("Failed: You've already voted for this Koi !");
                }
                else // Muốn bỏ Vote:
                {
                    if (check == false) // Chưa vote:
                        throw new Exception("Failed: You've not voted for this Koi yet!");
                }
                // Update:
                await _repository.Registrations.UpdateVotes(registrationId, (int)member.Id!, vote);
            }
            else
                throw new Exception("Failed: User/ Registration does not exist !");
        }
    }
}
