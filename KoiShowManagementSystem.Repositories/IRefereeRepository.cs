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
        Task<ShowModel> GetListShow();
        Task ScoreKoi(int koiId);
    }
}
