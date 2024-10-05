using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class KoiRegistrationRepository : IKoiRegistrationRepository
    {
        private KoiShowManagementSystemContext _context;
        public KoiRegistrationRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<KoiRegistModel>> GetKoiRegistrationByUserID(int id)
        {
            // Lấy đơn của User:
            IEnumerable<KoiRegistration> koiRegistrations = (await _context.Set<KoiRegistration>().ToListAsync())
                        .Where(koiRegist => koiRegist.UserId == id);
            // Join lấy thông tin:
            IEnumerable<Show> shows = await _context.Set<Show>().ToListAsync();
            IEnumerable<Group> groups = await _context.Set<Group>().ToListAsync();
            IEnumerable<Illustration> illustrations = await _context.Set<Illustration>().ToListAsync();
            IEnumerable<Variety> varieties = await _context.Set<Variety>().ToListAsync();
            IEnumerable<KoiRegistModel> result = from koi in koiRegistrations
                         join var in varieties on koi.VarietyId equals var.Id
                         join illus in illustrations on koi.Id equals illus.KoiId
                         join grp in groups on koi.GroupId equals grp.Id
                         join show in shows on grp.ShowId equals show.Id
                         select new KoiRegistModel()
                         {
                            Id = koi.Id,
                            Name = koi.Name,
                            Description = koi.Description,
                            Size = koi.Size,
                            Variety = var.Name,
                            ShowId = show.Id,
                            Show = show.Title,
                            Group = grp.Name,
                            CreateDate = koi.CreateDate,
                            Rank = koi.Rank,
                            TotalScore = koi.TotalScore,
                            Status = koi.Status,
                            IsBestVote = koi.IsBestVote,
                            ImageUrl = illus.ImageUrl,
                            VideoUrl = illus.VideoUrl,
                         };
            return result;
        }

        /* Hàm này có dùng không Tín, nếu dùng thì sửa lại rồi qua interface đăng kí thêm hàm này.
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
        */

    }
}
