using KoiShowManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class KoiRegistrationRepository : GenericRepository<KoiRegistration>, IKoiRegistrationRepository
    {
        public KoiRegistrationRepository(DbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<KoiRegistration>> GetByUserID(int id)
        {
            IEnumerable<KoiRegistration> result = null!;
            result = (await _dbContext.Set<KoiRegistration>().ToListAsync())
                        .Where(koiRegist => koiRegist.UserId == id);
            return result;
        }

        public async Task<(int TotalItems, IEnumerable<object> Kois)> GetKoiByShowId(int pageIndex, int pageSize, int showId)
        {
            // Query to get total koi count for pagination purposes
            var totalItems = await _dbContext.Set<KoiRegistration>()
                .Where(k => k.Group.ShowId == showId)
                .CountAsync();

            // Query to fetch koi details with pagination and additional fields
            var koiList = await _dbContext.Set<KoiRegistration>()
                .Include(k => k.Group)
                .Include(k => k.Variety)
                .Include(k => k.Illustration)
                .Where(k => k.Group.ShowId == showId)
                .Select(k => new
                {
                    Id = k.Id,
                    Name = k.Name,
                    Size = k.Size,
                    VarietyName = k.Variety!.Name,
                    Image = k.Illustration!.ImageUrl,
                    TotalScore = k.TotalScore,
                    BestVoted = k.IsBestVote,
                    Status = k.Status,
                    Rank = k.Rank
                })
                .Skip((pageIndex - 1) * pageSize)  
                .Take(pageSize)                    
                .ToListAsync();        
            return (totalItems, koiList);
        }


    }
}
