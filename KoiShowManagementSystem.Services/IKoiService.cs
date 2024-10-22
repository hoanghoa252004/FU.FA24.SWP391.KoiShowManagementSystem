using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
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
        Task<List<KoiModel>> GetKoiByUserId();
        // get Koi details by koi id
        Task<KoiModel?> GetKoiDetail(int koiId);

        // create a new koi
        Task<bool> CreateKoi(KoiDTO koi);

        Task<bool> UpdateKoi(KoiDTO koi);
        Task<bool> DeleteKoi(int koiId);
        Task<List<KoiModel>> DashboardKoi();
    }
}
