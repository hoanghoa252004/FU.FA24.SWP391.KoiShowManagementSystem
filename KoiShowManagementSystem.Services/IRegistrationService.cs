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
        Task<RegistrationModel?> GetRegistration(int registrationId);
        Task<(int TotalItems, IEnumerable<RegistrationModel> Registrations)> GetPendingRegistration(int pageIndex, int pageSize);
        Task UpdateRegistration(UpdateRegistrationModel dto);
        Task PublishResult(int showId);
    }
}
