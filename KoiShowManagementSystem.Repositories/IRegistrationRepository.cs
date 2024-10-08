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
        Task<RegistrationFormModel?> GetRegistrationFormAsync(int showId);
    }
}
