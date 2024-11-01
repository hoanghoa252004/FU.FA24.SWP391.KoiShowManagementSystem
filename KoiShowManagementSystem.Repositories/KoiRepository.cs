using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories.Helper;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

        public async Task<List<KoiModel>> GetAllKoiByUserIdAsync(int userId)
        {
            return await _context.Kois.Where(k => k.UserId == userId && k.Status == true).Select(k => new KoiModel
            {
                KoiID = k.Id,
                KoiName = k.Name,
                KoiVariety = k.Variety.Name,
                KoiSize = k.Size,
                KoiImg = k.Image,
                VarietyId = k.Variety.Id,
            }).ToListAsync();
        }

        public async Task<KoiModel?> GetKoiAsync(int koiId)
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
                registrations = k.Registrations.Where(r => r.Status.ToLower().Equals("scored")).Select(r => new RegistrationModel
                {
                    Id = r.Id,
                    Show = r.Group!.Show!.Title,
                    Group = r.Group.Name,
                    CreateDate = r.CreateDate,
                    Rank = r.Rank,
                    TotalScore = r.TotalScore,
                    Size = r.Size,
                    Status = r.Status,
                    IsBestVote = r.IsBestVote,
                    Image1 = r.Media!.Image1,
                    Image2 = r.Media.Image2,
                    Image3 = r.Media.Image3,
                    Video = r.Media.Video,
                }).ToList(),
                UserId = k.UserId,
            }).FirstOrDefaultAsync();
        }

        public async Task<bool> CreateKoiAsync(KoiDTO koi, int userId)
        {
            if (koi == null) throw new ArgumentNullException(nameof(koi));

            var newKoi = new Koi
            {
                Name = koi.Name!,
                Description = koi.Description,
                Image = koi.Image != null ? await _s3UploadService.UploadKoiImage(koi.Image) : null,
                Size =  koi.Size,
                VarietyId =koi.VarietyId,
                UserId = userId,
                Status = true
            };
            _context.Kois.Add(newKoi);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateKoiAsync(KoiDTO koi)
        {
            if (koi == null) throw new ArgumentNullException(nameof(koi));
            var koiToUpdate = await _context.Kois.Include(k => k.Registrations).FirstOrDefaultAsync(k => k.Id == koi.Id);
            
            if (koiToUpdate == null) return false;
            if (koi.Name != null)
            {
                koiToUpdate.Name = koi.Name!;
            }
            if (koi.Description != null)
            {
                koiToUpdate.Description = koi.Description;
            }
            if (koi.Size != 0)
            {
                koiToUpdate.Size = koi.Size;
            }

            if (koi.VarietyId != 0)
            {
                if (koiToUpdate.Registrations.IsNullOrEmpty())
                    koiToUpdate.VarietyId = koi.VarietyId;
                else throw new ArgumentException("This Koi has joined a show");
            }

            if (koi.Image != null)
            {
                koiToUpdate.Image = await _s3UploadService.UpdateImageAsync(koiToUpdate.Image!, koi.Image!);
            }
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteKoiAsync(int koiId)
        {
            var koiToDelete = await _context.Kois.Include(k => k.Registrations).FirstOrDefaultAsync(k => k.Id == koiId);

            if (koiToDelete == null) return false;
            if (koiToDelete.Registrations.IsNullOrEmpty())
            {
                koiToDelete.Status = false;
                
            }
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

    }
}
