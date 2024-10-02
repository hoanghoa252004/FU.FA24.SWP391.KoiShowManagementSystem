using KoiShowManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class KoiShowRepository : GenericRepository<Show>, IKoiShowRepository
    {
        public KoiShowRepository(DbContext context) : base(context) { }

        public async Task<Show?> GetShowById(int showId)
        {
            return await _dbContext.Set<Show>().FindAsync(showId);
        }

        public async Task<List<Group>> GetGroupsByShowId(int showId)
        {
            return await _dbContext.Set<Group>()
                                   .Where(g => g.ShowId == showId)
                                   .ToListAsync();
        }

        public async Task<List<RefereeDetail>> GetRefereesByShowId(int showId)
        {
            return await _dbContext.Set<RefereeDetail>()
                                   .Where(r => r.ShowId == showId)
                                   .ToListAsync();
        }

        public async Task<KoiRegistration?> GetKoiDetailById(int koiId)
        {
            return await _dbContext.Set<KoiRegistration>()
                                   .FirstOrDefaultAsync(kr => kr.Id == koiId);
        }

        public async Task<(int TotalItems, List<Show>Shows)> SearchShow(int pageIndex, int pageSize, string keyword)
        {
            var query = _dbContext.Set<Show>().Where(s => s.Title.Contains(keyword));
            var totalItems = await query.CountAsync();
            var shows = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            return (totalItems, shows);
        }
    }
}
