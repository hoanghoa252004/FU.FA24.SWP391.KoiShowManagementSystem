using KoiShowManagementSystem.DTOs.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IKoiRepository
    {
        Task<List<KoiModel>> GetAllKoiByUserId(int userId);
        Task<KoiModel?> GetKoiDetail(int koiId);
    }
}
