using KoiShowManagementSystem.DTOs.BusinessModels;
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

        public async Task<ShowModel?> GetShowDetailsAsync(int showId)
        {
            var result = await (from sho in _context.Shows
                                join gro in _context.Groups on sho.Id equals gro.ShowId
                                join koi in _context.KoiRegistrations on gro.Id equals koi.GroupId
                                join refDetail in _context.RefereeDetails on sho.Id equals refDetail.ShowId
                                join usr in _context.Users on refDetail.UserId equals usr.Id
                                where sho.Id == showId
                                select new ShowModel
                                {
                                    ShowId = sho.Id,
                                    ShowTitle = sho.Title,
                                    ShowBanner = sho.Banner!,
                                    ShowDesc = sho.Description!,
                                    StartDate = sho.ScoreStartDate,
                                    RegistrationStartDate = sho.RegisterStartDate,
                                    RegistrationCloseDate = sho.RegisterEndDate,
                                    ShowStatus = sho.Status!,
                                    EndDate = sho.ScoreEndDate,
                                    ShowGroups = (from gro in _context.Groups
                                                  where gro.ShowId == sho.Id
                                                  select new GroupModel
                                                  {
                                                      GroupId = gro.Id,
                                                      GroupName = gro.Name,
                                                      KoiDetails = (from koi in _context.KoiRegistrations
                                                                    where koi.GroupId == gro.Id
                                                                    select new KoiModel
                                                                    {
                                                                        KoiID = koi.Id,
                                                                        KoiName = koi.Name,
                                                                        Rank = koi.Rank,
                                                                        BestVote = koi.IsBestVote
                                                                    }).ToList()
                                                  }).ToList(),

                                    ShowReferee = (from refDetail in _context.RefereeDetails
                                                   join usr in _context.Users on refDetail.UserId equals usr.Id
                                                   where refDetail.ShowId == sho.Id
                                                   select new RefereeModel
                                                   {
                                                       RefereeId = refDetail.Id,
                                                       RefereeName = usr.Name
                                                   }).ToList(),
                                }).FirstOrDefaultAsync();

            return result!;
        }

        public async Task<(int TotalItems, List<ShowModel>)> SearchShowAsync(int pageIndex, int pageSize, string keyword)
        {
            var query = _context.Shows.Where(s => s.Title.Contains(keyword));

            var totalItems = await query.CountAsync();

            var shows = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new ShowModel
                {
                    ShowId = s.Id,
                    ShowTitle = s.Title,
                    ShowBanner = s.Banner,
                    ShowStatus = s.Status
                }).ToListAsync();

            return (totalItems, shows);
        }

        public async Task<KoiModel?> GetKoiDetailAsync(int koiId)
        {
            var result = await (from kr in _context.KoiRegistrations
                                join u in _context.Users on kr.UserId equals u.Id
                                join g in _context.Groups on kr.GroupId equals g.Id
                                join s in _context.Shows on g.ShowId equals s.Id
                                join v in _context.Varieties on kr.VarietyId equals v.Id
                                where kr.Id == koiId && kr.Size >= g.SizeMin && kr.Size <= g.SizeMax
                                select new KoiModel
                                {
                                    KoiID = kr.Id,
                                    KoiName = kr.Name,
                                    KoiImg = _context.Illustrations.Where(i => i.KoiId == kr.Id).Select(i => i.ImageUrl).FirstOrDefault(),
                                    KoiVideo = _context.Illustrations.Where(i => i.KoiId == kr.Id).Select(i => i.VideoUrl).FirstOrDefault(),
                                    KoiVariety = v.Name,
                                    KoiDesc = kr.Description,
                                    KoiSize = kr.Size,
                                    TotalScore = kr.TotalScore,
                                    BestVote = kr.IsBestVote,
                                    RegistrationStatus = kr.Status,
                                    Rank = kr.Rank
                                }).FirstOrDefaultAsync();

            return result;
        }

        public async Task<(int TotalItems, List<KoiModel>)> GetKoiByShowIdAsync(int pageIndex, int pageSize, int showId)
        {
            var query = _context.KoiRegistrations
                .Include(k => k.Group)
                .Include(k => k.Variety)
                .Include(k => k.Illustration)
                .Where(k => k.Group.ShowId == showId);

            var totalItems = await query.CountAsync();

            var koiList = await query
                .Select(k => new KoiModel
                {
                    KoiID = k.Id,
                    KoiName = k.Name,
                    KoiImg = k.Illustration != null ? k.Illustration.ImageUrl : null,
                    KoiVariety = k.Variety != null ? k.Variety.Name : "Unknown",
                    KoiSize = k.Size,
                    TotalScore = k.TotalScore,
                    IsBestVote = k.IsBestVote,
                    KoiStatus = k.Status,
                    Rank = k.Rank
                })
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalItems, koiList);
        }
    }
}
