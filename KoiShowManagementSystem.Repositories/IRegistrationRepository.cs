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
        Task CreateRegistrationAsync(RegistrationFormModel dto);
        Task<List<RegistrationModel>> GetRegistrationByShowAsync(int showId);
        Task<RegistrationModel?> GetRegistrationAsync(int registrationId);
    }
}
