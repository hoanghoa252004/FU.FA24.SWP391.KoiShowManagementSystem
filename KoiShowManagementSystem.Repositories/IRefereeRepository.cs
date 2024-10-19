using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
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
        Task<List<ShowModel>> GetShowsWithKoiByUserIdAsync(int userId);

        Task<List<RefereeModel>> GetAllRefereeByShowAsync(int showId);
        Task<bool> AddRefereeToShowAsync(List<int> referees, int showId);
    }
}
