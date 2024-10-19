using KoiShowManagementSystem.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IScoreRepository
    {
        Task<bool> SaveScoresAsync(RefereeScoreDTO refereeScore, int UserId);
        Task CalculateTotalScoreAsync(int showId);
    }
}
