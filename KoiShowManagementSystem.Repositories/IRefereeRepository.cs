using KoiShowManagementSystem.DTOs.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IRefereeRepository 
    {
        Task<List<ShowModel>> GetListShowAsync();
        Task<List<KoiModel>> GetKoiDetailsByGroupIdAsync(int groupId);
        Task<bool> SaveScoreAsync(int koiId, int criterionId, int refereeDetailId, decimal scoreValue);
    }
}
