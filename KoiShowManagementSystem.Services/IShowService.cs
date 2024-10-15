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
        Task<List<ShowModel>> GetClosestShow();
        Task<int> CreateAShow(ShowDTO dto);
        Task<List<VarietyModel>> GetAllVarieties();
        Task<bool> UpdateShow(ShowDTO dto);
    }
}
