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
        Task<Show?> GetShowById(int showId);

        Task<List<Group>> GetGroupsByShowId(int showId);

        Task<List<RefereeDetail>> GetRefereesByShowId(int showId);

        Task<KoiRegistration?> GetKoiDetailById(int koiId);

        Task<(int TotalItems, List<Show>Shows)> SearchShow(int pageIndex, int pageSize, string keyword);
    }
}
