using KoiShowManagementSystem.DTOs.BusinessModels;
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
        Task<(int TotalItems, IEnumerable<KoiModel> Kois)> GetKoiByShowId(int pageIndex, int pageSize, int showId);
        Task<KoiModel?> GetKoiDetail(int koiId);
    }
}
