using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IShowRepository
    {
        Task<ShowModel?> GetShowDetailsAsync(int showId);
        Task<(int TotalItems, List<ShowModel>)> SearchShowAsync(int pageIndex, int pageSize, string keyword);
        Task<RegistrationModel?> GetKoiDetailAsync(int koiId);
        Task<(int TotalItems, List<RegistrationModel>)> GetKoiByShowIdAsync(int pageIndex, int pageSize, int showId);
        Task<List<ShowModel>> GetClosestShowAsync();
        Task<int> AddNewShow(ShowDTO dto);
        Task<List<VarietyModel>> GetAllVarietiesAsync();
    }
}
