using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IShowService
    {
        Task<ShowModel?> GetShowDetails(int showId);
        Task<(int TotalItems, List<ShowModel> Shows)> SearchShow(int pageIndex, int pageSize, string keyword);
        Task<(int TotalItems, IEnumerable<RegistrationModel> Kois)> GetKoiByShowId(int pageIndex, int pageSize, int showId);
        Task<RegistrationModel?> GetKoiDetail(int koiId);
        Task<List<ShowModel>> GetClosestShow();
        Task<int> CreateAShow(ShowDTO dto);
        Task<List<VarietyModel>> GetAllVarieties();

    }
}
