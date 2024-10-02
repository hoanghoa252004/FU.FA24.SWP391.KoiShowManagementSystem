using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public class KoiShowService : IKoiShowService
    {
        private readonly UnitOfWork _unitOfWork;
        public KoiShowService(UnitOfWork unitOfWork, JwtServices jwtServices)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<object?> GetShowDetails(int showId)
        {
            // Lấy dữ liệu liên quan từ UnitOfWork
            var shows = await _unitOfWork.Shows.GetAll();
            var groups = await _unitOfWork.Groups.GetAll();
            var referees = await _unitOfWork.RefereeDetails.GetAll();
            var users = await _unitOfWork.Users.GetAll();
            var koiRegistrations = await _unitOfWork.KoiRegistrations.GetAll();

            // Query dữ liệu show dựa trên showId
            var result = (from sho in shows
                          where sho.Id == showId
                          select new
                          {
                              ShowId = sho.Id,
                              ShowName = sho.Title,
                              ShowBanner = sho.Banner,
                              ShowDesc = sho.Description!,
                              StartDate = sho.StartDate,
                              RegistrationStartDate = sho.RegisterStartDate,
                              RegistrationCloseDate = sho.RegisterEndDate,
                              ShowStatus = sho.Status == "Up Comming" ? 0 : sho.Status == "On Going" ? 1 : sho.Status == "Finished" ? 2 : -1,

                              // Nhóm Show
                              ShowGroups = (from gro in groups
                                            where gro.ShowId == sho.Id
                                            select new
                                            {
                                                GroupId = gro.Id,
                                                GroupName = gro.Name
                                            }).ToList(),

                              // Trọng tài của Show
                              ShowReferee = (from refDetail in referees
                                             join usr in users on refDetail.UserId equals usr.Id
                                             where refDetail.ShowId == sho.Id
                                             select new
                                             {
                                                 RefereeId = refDetail.Id,
                                                 RefereeName = usr.Name
                                             }).ToList(),

                              // Koi tham gia Show
                              Koi = (from koi in koiRegistrations
                                     join gro in groups on koi.GroupId equals gro.Id
                                     where gro.ShowId == sho.Id
                                     select new
                                     {
                                         BestVote = koi.IsBestVote,
                                         Rank = koi.Rank,
                                     }).ToList(),
                          }).FirstOrDefault();

            return result;
        }

        public async Task<(int TotalItems, List<object> Shows)> SearchShow(int pageIndex, int pageSize, string keyword)
        {
            var (totalItems, shows) = await _unitOfWork.KoiShow.SearchShow(pageIndex, pageSize, keyword);
            var result = shows.Select(s => new
            {
                Id = s.Id,
                Title = s.Title,
                Banner = s.Banner,
                Status = s.Status
            }).ToList<object>();

            return (totalItems, result);
        }

        //public async Task<(int TotalItems, List<object> Kois)> GetKoiByShowId(int pageIndex, int pageSize, int showId)
        //{
        //    var totalItems = await _unitOfWork.KoiRegistrations.GetAll()
        //        .Where(k => k.Group.ShowId == showId)
        //        .CountAsync();

        //    var koiList = await _unitOfWork.KoiRegistrations.GetAll()
        //        .Include(k => k.Group)
        //        .Include(k => k.Variety)
        //        .Include(k => k.Illustration)
        //        .Where(k => k.Group.ShowId == showId)
        //        .Select(k => new
        //        {
        //            KoiID = k.Id,
        //            KoiName = k.Name,
        //            KoiImg = k.Illustration != null ? k.Illustration.ImageUrl : null,
        //            KoiVariety = k.Variety != null ? k.Variety.Name : null,
        //            KoiSize = k.Size,
        //            TotalScore = k.TotalScore,
        //            BestVoted = k.IsBestVote,
        //            Status = k.Status == "Scored" ? new
        //            {
        //                Rank = k.Rank,
        //                BestVote = k.IsBestVote
        //            } : null
        //        })
        //        .Skip((pageIndex - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    return (totalItems, koiList);
        //}

        public async Task<object?> GetKoiDetail(int koiId)
        {
            // Lấy dữ liệu liên quan từ UnitOfWork
            var koiRegistrations = await _unitOfWork.KoiRegistrations.GetAll();
            var users = await _unitOfWork.Users.GetAll();
            var groups = await _unitOfWork.Groups.GetAll();
            var shows = await _unitOfWork.Shows.GetAll();
            var varieties = await _unitOfWork.Varieties.GetAll();
            var illustrations = await _unitOfWork.Illustrations.GetAll();

            // Query dữ liệu koi dựa trên koiId
            var result = (from kr in koiRegistrations
                          join u in users on kr.UserId equals u.Id
                          join g in groups on kr.GroupId equals g.Id
                          join s in shows on g.ShowId equals s.Id
                          join v in varieties on kr.VarietyId equals v.Id 
                          where kr.Id == koiId && kr.Size >= g.SizeMin && kr.Size <= g.SizeMax
                          select new
                          {
                              KoiID = kr.Id,
                              KoiName = kr.Name,
                              KoiImg = illustrations.Where(i => i.KoiId == kr.Id).Select(i => i.ImageUrl).ToList(),
                              KoiVideo = illustrations.Where(i => i.KoiId == kr.Id).Select(i => i.VideoUrl).FirstOrDefault(), 
                              KoiVariety = v.Name, 
                              KoiDesc = kr.Description,
                              KoiSize = kr.Size,
                              TotalScore = kr.TotalScore,
                              BestVote = kr.IsBestVote,
                              RegistrationStatus = (s.RegisterStartDate <= DateTime.Now && s.RegisterEndDate >= DateTime.Now),
                              Rank = kr.Rank
                          }).FirstOrDefault();

            return result;
        }






    }
}
