using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IRefereeService
    {
        Task<List<ShowModel>> GetListShow();
        Task<List<KoiModel>> GetKoiDetailsByGroupId(int groupId);
        Task<bool> SaveScoreFromReferee(RefereeScoreDTO dto);
        Task<List<ShowModel>> GetShowsWithKoiByUserIdAsync();
        Task<List<RefereeModel>> GetAllRefereeByShow(int showId);
        Task<bool> AddRefereeToShow(List<int> referees, int showId);
        Task<bool> RemoveRefereeFromShow(int refereeDetailId);
    }
}
