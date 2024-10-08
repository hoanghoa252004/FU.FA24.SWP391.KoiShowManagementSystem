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
    public class RegistrationRepository : IRegistrationRepository
    {
        private KoiShowManagementSystemContext _context;
        public RegistrationRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        public async Task<IEnumerable<RegistrationModel>> GetRegistrationByUserIdAsync(int id)
        {
            // Lấy đơn của User:
            IEnumerable<Koi> kois = (await _context.Set<Koi>().ToListAsync())
                        .Where(koiRegist => koiRegist.UserId == id);
            // Join lấy thông tin:
            IEnumerable<Registration> registrations = await _context.Set<Registration>().ToListAsync();
            IEnumerable<Show> shows = await _context.Set<Show>().ToListAsync();
            IEnumerable<Group> groups = await _context.Set<Group>().ToListAsync();
            IEnumerable<Media> media = await _context.Set<Media>().ToListAsync();
            IEnumerable<Variety> varieties = await _context.Set<Variety>().ToListAsync();
            IEnumerable<RegistrationModel> result = from koi in kois
                         join var in varieties on koi.VarietyId equals var.Id
                         join regist in registrations on  koi.Id equals regist.KoiId
                         join med in media on regist.Id equals med.RegistrationId
                         join grp in groups on regist.GroupId equals grp.Id
                         join show in shows on grp.ShowId equals show.Id
                         select new RegistrationModel()
                         {
                            Id = koi.Id,
                            Name = koi.Name,
                            Description = koi.Description,
                            Size = koi.Size,
                            Variety = var.Name,
                            ShowId = show.Id,
                            Show = show.Title,
                            Group = grp.Name,
                            CreateDate = regist.CreateDate,
                            Rank = regist.Rank,
                            TotalScore = regist.TotalScore,
                            Status = regist.Status,
                            IsBestVote = regist.IsBestVote,
                            Image1 = med.Image1,
                            Image2 = med.Image1,
                            Image3 = med.Image1,
                            Video = med.Video,
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

        public async Task<RegistrationFormModel?> GetRegistrationFormAsync(int showId)
        {
            // Lấy show:
            var show = await _context.Shows.SingleOrDefaultAsync(show => show.Id == showId);
            // Tìm những groups của show để lấy bảng size:
            var groupList = _context.Groups.Where(group => group.ShowId == showId);
            List<GroupModel> sizeList = new List<GroupModel>();
            List<VarietyModel> varietyModels = new List<VarietyModel>();
            var varietiesList = _context.Varieties.Include(var => var.Groups).ToList();

            foreach (var grp in groupList)
            {
                foreach (var var in varietiesList)
                {
                    if (grp.Varieties.Contains(var))
                    {
                        varietyModels.Add(new VarietyModel()
                        {
                            VarietyId = var.Id,
                            VarietyName = var.Name,
                        });
                        sizeList.Add(new GroupModel()
                        {
                            GroupId = grp.Id,
                            GroupName = grp.Name,
                            SizeMax = grp.SizeMax,
                            SizeMin = grp.SizeMin,
                            Unit = "cm"
                        });
                    }
                }
            }

            return new RegistrationFormModel()
            {
                ShowId = showId,
                ShowName = show!.Title,
                SizeList = sizeList,
                VarietyList = varietyModels,
            };
        }
    }
}
