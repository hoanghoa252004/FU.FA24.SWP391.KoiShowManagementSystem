using KoiShowManagementSystem.DTOs.BusinessModels;
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
        Task<bool> SaveScoreFromReferee(int criterionId, int koiId, int refereeDetailId, decimal scoreValue);
    }
}
