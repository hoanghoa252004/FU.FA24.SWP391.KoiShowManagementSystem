using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories.Helper;
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
        private readonly S3UploadService _s3UploadService;
        private KoiShowManagementSystemContext _context;
        public RegistrationRepository(KoiShowManagementSystemContext context, S3UploadService _s3UploadService)
        {
            this._context = context;
            this._s3UploadService = _s3UploadService;
        }

        public async Task CreateRegistrationAsync(RegistrationFormModel dto)
        {
            string image1 = await _s3UploadService.UploadKoiImage(dto.Image1!);
            string image2 = await _s3UploadService.UploadKoiImage(dto.Image2!);
            string image3 = await _s3UploadService.UploadKoiImage(dto.Image3!);
            Registration newRegistration = new Registration()
            {
                CreateDate = DateOnly.FromDateTime(DateTime.Now),
                Size = dto.Size,
                KoiId = dto.KoiId,
                GroupId = dto.GroupId,
            };
            await _context.Set<Registration>().AddAsync(newRegistration);
            await _context.SaveChangesAsync();
            // Add Media:
            await _context.Set<Media>().AddAsync(new Media()
            {
                RegistrationId = newRegistration.Id,
                Image1 = image1,
                Image2 = image2,
                Image3 = image3,
                Video = dto.Video!,
            });
            await _context.SaveChangesAsync();
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
    }
}
