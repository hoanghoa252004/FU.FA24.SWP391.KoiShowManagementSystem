using KoiShowManagementSystem.DTOs.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IKoiService
    {
        // get all koi of a user by user id
        Task<List<KoiModel>> GetKoiByUserId(int userId);
        // get Koi details by koi id
        Task<KoiModel?> GetKoiDetail(int koiId);
    }
}
