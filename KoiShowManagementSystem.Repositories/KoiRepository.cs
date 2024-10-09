using KoiShowManagementSystem.DTOs.BusinessModels;
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
        private readonly KoiShowManagementSystemContext _context;
        public KoiRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        public async Task<List<KoiModel>> GetAllKoiByUserId(int userId)
        {
            return await  _context.Kois.Where(k => k.UserId == userId).Select(k => new KoiModel
            {
                KoiID = k.Id,
                KoiName = k.Name,
                KoiVariety = k.Variety.Name,
                KoiSize = k.Size,
                KoiImg = k.Image,

            }).ToListAsync();
        }

        public async Task<KoiModel?> GetKoiDetail(int koiId)
        {
            return await _context.Kois.Where(k => k.Id == koiId).Select(k => new KoiModel
            {
                KoiID = k.Id,
                KoiName = k.Name,
                KoiVariety = k.Variety.Name,
                KoiImg = k.Image,
                KoiSize = k.Size,
                KoiDesc = k.Description,
                KoiStatus = k.Status,
                registrations = k.Registrations.Select(r => new RegistrationModel
                {
                    Id = r.Id,
                    Show = r.Group.Show.Title,
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
                }).ToList()

            }).FirstOrDefaultAsync();
        }


    }
}
