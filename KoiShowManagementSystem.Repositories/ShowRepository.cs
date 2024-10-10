﻿using KoiShowManagementSystem.DTOs.BusinessModels;
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
    public class ShowRepository : IShowRepository
    {
        private KoiShowManagementSystemContext _context;
        private readonly S3UploadService _s3Service;
        public ShowRepository(KoiShowManagementSystemContext context, S3UploadService s3Service)
        {
            this._context = context;
            this._s3Service = s3Service;
        }

        public async Task<ShowModel?> GetShowDetailsAsync(int showId)
        {
            var result = await (from sho in _context.Shows
                                where sho.Id == showId
                                select new ShowModel
                                {
                                    ShowId = sho.Id,
                                    ShowTitle = sho.Title,
                                    ShowBanner = sho.Banner ?? string.Empty,
                                    ShowDesc = sho.Description ?? string.Empty,
                                    StartDate = sho.ScoreStartDate,
                                    RegistrationStartDate = sho.RegisterStartDate,
                                    RegistrationCloseDate = sho.RegisterEndDate,
                                    ShowStatus = sho.Status ?? string.Empty,
                                    EndDate = sho.ScoreEndDate,

                                    ShowGroups = (from gro in _context.Groups
                                                  where gro.ShowId == sho.Id
                                                  select new GroupModel
                                                  {
                                                      GroupId = gro.Id,
                                                      GroupName = gro.Name ?? string.Empty,
                                                      Registrations = (from reg in _context.Registrations
                                                                    where reg.GroupId == gro.Id
                                                                    select new RegistrationModel
                                                                    {
                                                                        KoiID = reg.KoiId ?? 0,
                                                                        Name = reg.Koi != null ? reg.Koi.Name : "Unknown",
                                                                        Rank = reg.Rank,
                                                                        IsBestVote = reg.IsBestVote
                                                                    }).ToList(),
                                                      Criterion = (from cri in _context.Criteria
                                                                   where cri.GroupId == gro.Id
                                                                   select new CriterionModel()
                                                                   {
                                                                       CriterionId = cri.Id,
                                                                       CriterionName = cri.Name,    
                                                                       Description = cri.Description,
                                                                       Percentage = cri.Percentage,
                                                                   }).ToList(),
                                                  }).ToList(),
                                    ShowReferee = (from refDetail in _context.RefereeDetails
                                                   join usr in _context.Users on refDetail.UserId equals usr.Id into referees
                                                   from usr in referees.DefaultIfEmpty()
                                                   where refDetail.ShowId == sho.Id
                                                   select new RefereeModel
                                                   {
                                                       RefereeId = refDetail.Id,
                                                       RefereeName = usr != null ? usr.Name : "No Referee"
                                                   }).ToList(),
                                }).FirstOrDefaultAsync();

            return result;
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

        public async Task<List<ShowModel>> GetClosestShowAsync()
        {
            var shows = await _context.Shows
                .Where(s => s.Status != "Draft")
                .Include(s => s.Groups)
                .OrderByDescending(s => s.RegisterStartDate).Take(5)
                .Select(s => new ShowModel
                {
                    ShowId = s.Id,
                    ShowTitle = s.Title,
                    ShowBanner = s.Banner,
                    ShowDesc = s.Description,
                    ShowStatus = s.Status
                }).ToListAsync();

            var firstShow = shows.FirstOrDefault();

            if (firstShow == null)
            {
                throw new Exception("No show found");
            }

            if (firstShow.ShowStatus == "Finished")
            {
                var groups  = _context.Groups
                   .Where(g => g.ShowId == firstShow.ShowId)
                   .Select(g => new GroupModel
                   {
                       GroupId = g.Id,
                       GroupName = g.Name,
                       Registrations = g.Registrations
                           .Where(r => r.Rank == 1 || r.Rank == 2 || r.Rank == 3 || r.IsBestVote == true)
                           .OrderBy(r=> r.Rank)
                           .Select(r => new RegistrationModel
                           {
                               Id = r.Id,
                               Name = r.Koi.Name,
                               Rank = r.Rank,
                               IsBestVote = r.IsBestVote
                           }).ToList()
                   }).ToList();
                if (!groups.IsNullOrEmpty())
                {
                    firstShow.ShowGroups = groups;
                }
            }
            else
            {
                firstShow.ShowReferee = _context.RefereeDetails
                    .Where(r => r.ShowId == firstShow.ShowId)
                    .Include(r => r.User)
                    .Select(r => new RefereeModel
                    {
                        RefereeId = r.Id,
                        RefereeName = r.User.Name
                    }).ToList();
            }
            return shows;
        }

        public async Task<int> AddNewShow(ShowDTO dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException("ShowDTO is null");
            }
            if (dto.RegisterStartDate > dto.RegisterEndDate)
            {
                throw new ArgumentException("Start date cannot be greater than end date");
            }

            if (dto.ScoreStartDate > dto.ScoreEndDate)
            {
                throw new ArgumentException("Start date cannot be greater than end date");
            }

            int result = 0;
            Show show = new()
            {
                Title = dto.Title,
                Description = dto.Description,
                ScoreStartDate = dto.ScoreStartDate,
                RegisterStartDate = dto.RegisterStartDate,
                RegisterEndDate = dto.RegisterEndDate,
                ScoreEndDate = dto.ScoreEndDate,
                Banner = await _s3Service.UploadShowBannerImage(dto.Banner),
                Status = "draft",
                Groups = dto.Groups.Select(g => new Group
                {
                    Name = g.Name,
                    SizeMin = g.MinSize,
                    SizeMax = g.MaxSize,
                    Varieties = _context.Varieties.Where(v => g.Varieties.Contains(v.Id)).ToList(),
                    Criteria = g.Criterias.Select(c => new Criterion
                    {
                        Name = c.Name,
                        Percentage = c.Percentage,
                        Description = c.Description,
                        Status = true,
                    }).ToList()
                }).ToList()
            };

            _context.Shows.Add(show);         
            _context.Groups.AddRange(show.Groups);            
            _context.Criteria.AddRange(show.Groups.SelectMany(g => g.Criteria));
            result = await _context.SaveChangesAsync();
            return result;
        }

        public async Task<List<VarietyModel>> GetAllVarietiesAsync()
        {
            var reuslt =await  _context.Varieties.Select(v => new VarietyModel
            {
                VarietyId = v.Id,
                VarietyName = v.Name
            }).ToListAsync();
            return reuslt;
        }

        public async Task<bool> ChangeShowStatus(string status, int showId)
        {
            string[] validStatus = { "draft", "up comming", "on going", "finished" };

            if (!validStatus.Contains(status))
            {
                throw new ArgumentException("Invalid status");
            }
            var show = _context.Shows.Find(showId);
            if (show == null)
            {
                throw new Exception("Show not found");
            }
            show.Status = status;
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        //để tạm ở đây
        public async Task<bool> UpdateRegistrationAverageScore(int registrationId)
        {

            var registration =_context.Registrations
                                         .Include(r => r.Scores)
                                         .FirstOrDefault(r => r.Id == registrationId);

            if (registration == null)
            {
                throw new ArgumentException($"Registration with Id {registrationId} not found.");
                
            }
            //lấy đơn đó có bao nhiêu điểm
            var scoreCount = registration.Scores.Count;
            //Nếu scoreCount đó lớn hơn không thì tính trung bình cộng
            decimal averageScore = scoreCount > 0 ? registration.Scores.Average(score => score.Score1 ?? 0) : 0;

            registration.TotalScore = averageScore;
            registration.Status = "Scored";

            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public Task<List<ShowModel>> GetAllShow()
        {
            return _context.Shows.Select(s => new ShowModel
            {
                ShowId = s.Id,
                ShowTitle = s.Title,
                ShowBanner = s.Banner,
                ShowDesc = s.Description,
                ShowStatus = s.Status
            }).ToListAsync();
        }


        //public async Task<bool> EditAShow(ShowDTO dto)
        //{
        //    var show = await _context.Shows.FindAsync(dto.Id);

        //    if (show == null)
        //    {
        //        throw new Exception("Show not found");
        //    }
        //    if (show.Status == "on going")
        //    {
        //        throw new Exception("Show is on going");
        //    }

        //    if (dto.RegisterStartDate > dto.RegisterEndDate)
        //    {
        //        throw new ArgumentException("Start date cannot be greater than end date");
        //    }

        //    if (dto.ScoreStartDate > dto.ScoreEndDate)
        //    {
        //        throw new ArgumentException("Start date cannot be greater than end date");
        //    }

        //    if (show.Title != dto.Title)
        //    {
        //        show.Title = dto.Title;
        //    }
        //    show.Description = dto.Description;
        //    show.ScoreStartDate = dto.ScoreStartDate;
        //    show.RegisterStartDate = dto.RegisterStartDate;
        //    show.RegisterEndDate = dto.RegisterEndDate;
        //    show.ScoreEndDate = dto.ScoreEndDate;


        //    var newGroups = dto.Groups.Select(g => new Group
        //    {
        //        Name = g.Name,
        //        SizeMin = g.MinSize,
        //        SizeMax = g.MaxSize,
        //        Varieties = _context.Varieties.Where(v => g.Varieties.Contains(v.Id)).ToList(),
        //        Criteria = g.Criterias.Select(c => new Criterion
        //        {
        //            Name = c.Name,
        //            Percentage = c.Percentage,
        //            Description = c.Description,
        //            Status = true
        //        }).ToList()
        //    }).ToList();

        //    _context.Groups.AddRange(newGroups);

        //    // Remove old groups
        //    var oldGroups = _context.Groups.Where(g => g.ShowId == show.Id && !newGroups.Any(ng => ng.Id == g.Id)).ToList();
        //    _context.Groups.RemoveRange(oldGroups);

        //    // Update existing groups
        //    foreach (var group in show.Groups)
        //    {
        //        var updatedGroup = dto.Groups.FirstOrDefault(g => g.Id == group.Id);
        //        if (updatedGroup != null)
        //        {
        //            group.Name = updatedGroup.Name;
        //            group.SizeMin = updatedGroup.MinSize;
        //            group.SizeMax = updatedGroup.MaxSize;
        //            group.Varieties = _context.Varieties.Where(v => updatedGroup.Varieties.Contains(v.Id)).ToList();

        //            // Add new criteria
        //            var newCriterias = updatedGroup.Criterias.Where(c => !group.Criteria.Any(gc => gc.Id == c.Id)).Select(c => new Criterion
        //            {
        //                Name = c.Name,
        //                Percentage = c.Percentage,
        //                Description = c.Description,
        //                Status = true
        //            }).ToList();
        //            group.Criteria.AddRange(newCriterias);

        //            // Remove old criteria
        //            var oldCriterias = group.Criteria.Where(c => !updatedGroup.Criterias.Any(uc => uc.Id == c.Id)).ToList();
        //            group.Criteria.RemoveAll(c => oldCriterias.Contains(c));
        //        }
        //    }

        //    await _context.SaveChangesAsync();

        //    return true;
        //}



        /*
        public async Task<List<ShowModel>> GetClosestShowAsync()
        {
           var shows = await _context.Shows
               .Where(s => s.Status != "Draft")
               .Include(s => s.Groups)
               .OrderByDescending(s => s.RegisterStartDate).Take(5)
               .Select(s => new ShowModel
               {
                   ShowId = s.Id,
                   ShowTitle = s.Title,
                   ShowBanner = s.Banner,
                   ShowDesc = s.Description,
                   ShowStatus = s.Status
               }).ToListAsync();
           // get the first show
           var firstShow = shows.FirstOrDefault();
           if (firstShow == null)
           {
               throw new Exception("No show found");
           }
           if (firstShow.ShowStatus == "Finished")
           {
               //get group of firstShow  and top rank 1,2,3, best vote of each groupModel
               firstShow.ShowGroups = _context.Groups
                   .Where(g => g.ShowId == firstShow.ShowId)
                   .Include(g => g.KoiRegistrations)
                   .Select(g => new GroupModel
                   {
                       GroupId = g.Id,
                       GroupName = g.Name,
                       KoiDetails = g.KoiRegistrations
                           .Where(k => k.Rank == 1 || k.Rank == 2 || k.Rank == 3 || k.IsBestVote == true)
                           .OrderBy(k => k.Rank)
                           .Select(k => new KoiModel
                           {
                               KoiID = k.Id,
                               KoiName = k.Name,
                               Rank = k.Rank,
                               IsBestVote = k.IsBestVote
                           }).ToList()
                   }).ToList();
           }
           else
           {
               firstShow.ShowReferee = _context.RefereeDetails
                   .Where(r => r.ShowId == firstShow.ShowId)
                   .Include(r => r.User)
                   .Select(r => new RefereeModel
                   {
                       RefereeId = r.Id,
                       RefereeName = r.User.Name
                   }).ToList();
           }


           return shows;
        }

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
        */
    }
}