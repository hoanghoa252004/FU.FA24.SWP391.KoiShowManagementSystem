using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IKoiRepository
    {
        Task<List<KoiModel>> GetAllKoiByUserIdAsync(int userId);
        Task<KoiModel?> GetKoiAsync(int koiId);
        Task<bool> CreateKoiAsync(KoiDTO koi, int userId);
        Task<bool> UpdateKoiAsync(KoiDTO koi);
        Task<bool> DeleteKoiAsync(int koiId);
        Task<UserModel> GetKoiUser(int koiId);
    }
}
