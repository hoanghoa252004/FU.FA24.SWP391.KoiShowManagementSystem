using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class ShowRepository : IShowRepository
    {
        private KoiShowManagementSystemContext _context;
        public ShowRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        public async Task<Show?> GetShowById(int showId)
        {
            return await _context.Set<Show>().FindAsync(showId);
        }

        public async Task<List<Group>> GetGroupsByShowId(int showId)
        {
            return await _context.Set<Group>()
                                   .Where(g => g.ShowId == showId)
                                   .ToListAsync();
        }

        public async Task<List<RefereeDetail>> GetRefereesByShowId(int showId)
        {
            return await _context.Set<RefereeDetail>()
                                   .Where(r => r.ShowId == showId)
                                   .ToListAsync();
        }

        public async Task<KoiRegistration?> GetKoiDetailById(int koiId)
        {
            return await _context.Set<KoiRegistration>()
                                   .FirstOrDefaultAsync(kr => kr.Id == koiId);
        }

        public async Task<(int TotalItems, List<Show>Shows)> SearchShow(int pageIndex, int pageSize, string keyword)
        {
            var query = _context.Set<Show>().Where(s => s.Title.Contains(keyword));
            var totalItems = await query.CountAsync();
            var shows = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            return (totalItems, shows);
        }
    }
}
