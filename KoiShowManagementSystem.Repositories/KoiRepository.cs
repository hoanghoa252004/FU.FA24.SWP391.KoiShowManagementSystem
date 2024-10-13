using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories.Helper;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class KoiRepository : IKoiRepository
    {
        private readonly S3UploadService _s3UploadService;
        private KoiShowManagementSystemContext _context;
        public KoiRepository(KoiShowManagementSystemContext context, S3UploadService _s3UploadService)
        {
            this._context = context;
            this._s3UploadService = _s3UploadService;
        }

        public async Task<List<KoiModel>> GetAllKoiByUserId(int userId)
        {
            return await  _context.Kois.Where(k => k.UserId == userId && k.Status == true).Select(k => new KoiModel
            {
                KoiID = k.Id,
                KoiName = k.Name,
                KoiVariety = k.Variety.Name,
                KoiSize = k.Size,
                KoiImg = k.Image,
                VarietyId = k.Variety.Id,
            }).ToListAsync();
        }

        public async Task<KoiModel?> GetKoi(int koiId)
        {
            return await _context.Kois.Where(k => k.Id == koiId).Select(k => new KoiModel
            {
                KoiID = k.Id,
                KoiName = k.Name,
                KoiVariety = k.Variety.Name,
                VarietyId = k.Variety.Id,
                KoiImg = k.Image,
                KoiSize = k.Size,
                KoiDesc = k.Description,
                KoiStatus = k.Status,
                registrations = k.Registrations.Select(r => new RegistrationModel
                {
                    Id = r.Id,
                    Show = r.Group!.Show!.Title,
                    Group = r.Group.Name,
                    CreateDate = r.CreateDate,
                    Rank = r.Rank,
                    TotalScore = r.TotalScore,
                    Status = r.Status,
                    IsBestVote = r.IsBestVote,
                    Image1 = r.Media.Select(m => m.Image1).FirstOrDefault(),
                    Image2 = r.Media.Select(m => m.Image2).FirstOrDefault(),
                    Image3 = r.Media.Select(m => m.Image3).FirstOrDefault(),
                    Video = r.Media.Select(m => m.Video).FirstOrDefault()
                }).ToList(),
                UserId = k.UserId,
            }).FirstOrDefaultAsync();
        }

        public async Task<bool> CreateKoi(KoiDTO koi, int userId)
        {
            if (koi == null) throw new ArgumentNullException(nameof(koi));
            
            var newKoi = new Koi
            {
                Name = koi.Name,
                Description = koi.Description,
                Image = koi.Image != null ? await _s3UploadService.UploadKoiImage(koi.Image) : null,
                Size = koi.Size,
                VarietyId = koi.VarietyId,
                UserId = userId,
                Status = true
            };
            _context.Kois.Add(newKoi);
            await _context.SaveChangesAsync();
            return true;
        }



    }
}
