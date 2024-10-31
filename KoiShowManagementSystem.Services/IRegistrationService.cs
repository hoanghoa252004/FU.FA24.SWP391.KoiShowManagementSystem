using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IRegistrationService
    {
        Task<IEnumerable<RegistrationModel>> GetMyRegistration(string status);
        Task CreateRegistration(CreateRegistrationRequest dto);
        Task<(int TotalItems, IEnumerable<RegistrationModel> Registrations)> GetRegistrationByShow(int pageIndex, int pageSize, int showId);
        Task<RegistrationModel?> GetRegistrationById(int registrationId);
        Task<(int TotalItems, IEnumerable<RegistrationModel> Registrations)> GetPendingRegistration(int pageIndex, int pageSize);
        Task UpdateRegistration(UpdateRegistrationModel dto);
        Task SendResult(int showId);
        Task VoteRegistration(int registrationId, bool vote);
        Task PublishScore(int showId);
        Task DeleteDraftRegistration(int registrationId);
        Task<(int TotalItems, IEnumerable<RegistrationModel> Registrations)> GetRegistrationByGroup(int pageIndex, int pageSize, int groupId);
        Task<UserModel?> GetUserInfoFromRegistration(int registrationId);
    }
}
