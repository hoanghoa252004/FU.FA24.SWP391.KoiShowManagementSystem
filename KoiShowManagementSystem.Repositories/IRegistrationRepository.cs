using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IRegistrationRepository
    {
        Task<IEnumerable<RegistrationModel>> GetRegistrationByUserIdAsync(int id);
        Task CreateRegistrationAsync(CreateRegistrationModel dto);
        Task<List<RegistrationModel>> GetRegistrationByShowAsync(int showId);
        Task<RegistrationModel?> GetRegistrationAsync(int registrationId);
        Task<RegistrationModel> UpdateRegistrationAsync(UpdateRegistrationModel dto);
        Task<IEnumerable<RegistrationModel>> GetAllRegistrationAsync();
        Task<bool> CheckVote(int userId, int registrationId);
        Task UpdateVotes(int registrationId, int memberId, bool vote);
    }
}
