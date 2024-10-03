using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IShowService
    {
        Task<object?> GetShowDetails(int showId);
        Task<(int TotalItems, List<object> Shows)> SearchShow(int pageIndex, int pageSize, string keyword);
        Task<(int TotalItems, IEnumerable<object> Kois)> GetKoiByShowId(int pageIndex, int pageSize, int showId);
        Task<object?> GetKoiDetail(int koiId);
    }
}
