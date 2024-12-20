﻿using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class RefereeRepository : IRefereeRepository
    {
        private readonly KoiShowManagementSystemContext _context;

        public RefereeRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        public async Task<List<ShowModel>> GetListShowAsync()
        {
            var query = await _context.Shows
                        .Include(sh => sh.Groups)
                        .ThenInclude(g => g.Registrations)
                        .OrderByDescending(sh => sh.Status == "scoring")
                        .Include(sh => sh.RefereeDetails)
                        .Select(sh => new ShowModel
                        {
                            ShowId = sh.Id,
                            ShowTitle = sh.Title,
                            ShowStatus = sh.Status,
                            ShowGroups = sh.Groups.Select(g => new GroupModel
                            {
                                GroupId = g.Id,
                                GroupName = g.Name,
                                //Scored = g.Registrations.Count(r => r.Status == "Scored"),
                                //AmountNotScored = g.Registrations.Count(r => r.Status == "Accepted")
                            }).ToList()
                        }).ToListAsync();

            return query;
        }

        public async Task<List<KoiModel>> GetKoiDetailsByGroupIdAsync(int groupId)
        {
            var koiDetails = await _context.Groups
                .Where(g => g.Id == groupId)
                .SelectMany(g => g.Registrations.Select(r => new KoiModel
                {
                    KoiID = r.Koi.Id,
                    KoiName = r.Koi.Name,
                    criterions = g.Criteria.Select(c => new CriterionModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    }).ToList()
                })).ToListAsync();

            return koiDetails;
        }

        //Gộp lại 
        //public async Task<List<ShowModel>> GetShowsWithKoiByUserIdAsync(int userId)
        //{
        //    var showsWithKoi = await _context.Shows
        //        .Where(sh => sh.Status == "On Going" &&
        //                     sh.RefereeDetails.Any(rd => rd.UserId == userId))
        //        .Include(sh => sh.Groups)
        //        .Include(sh => sh.Groups)
        //            .ThenInclude(g => g.Registrations)
        //            .ThenInclude(r => r.Koi)
        //        .Include(sh => sh.Groups)
        //            .ThenInclude(g => g.Criteria)
        //        .Include(sh => sh.Groups)
        //            .ThenInclude(g => g.Registrations)
        //            .ThenInclude(r => r.Media)
        //        .Include(sh => sh.Groups)
        //            .ThenInclude(g => g.Criteria)
        //            .ThenInclude(c => c.Scores)
        //        .Select(sh => new ShowModel
        //        {
        //            ShowId = sh.Id,
        //            ShowTitle = sh.Title,
        //            ShowStatus = sh.Status,
        //            ShowGroups = sh.Groups.Select(g => new GroupModel
        //            {
        //                GroupId = g.Id,
        //                GroupName = g.Name,
        //                Scored = g.Registrations.Count(r => r.Status == "Scored"), // Count the koi that have been scored
        //                AmountNotScored = g.Registrations.Count(r => r.Status == "Accepted"), // Count the koi that have been accepted
        //                Kois = g.Registrations.Select(r => new KoiModel
        //                {
        //                    KoiID = r.Koi.Id,
        //                    RegistrationId = r.Id,
        //                    KoiName = r.Koi.Name,
        //                    Image1 = r.Media.Image1,
        //                    Image2 = r.Media.Image2,
        //                    Image3 = r.Media.Image3,
        //                    Video = r.Media.Video,
        //                    isScored = r.Status == "Scored",
        //                    criterions = g.Criteria.Select(c => new CriterionModel
        //                    {
        //                        CriterionId = c.Id,
        //                        CriterionName = c.Name,
        //                        Percentage = c.Percentage,
        //                        Description = c.Description,
        //                        Score1 = c.Scores
        //                    .Where(score =>  score.CriteriaId == c.Id && score.RegistrationId == r.Id)
        //                    .Select(score => score.Score1)
        //                    .FirstOrDefault()
        //                    }).ToList()
        //                }).ToList()
        //            }).ToList()
        //        })
        //        .ToListAsync();

        //    return showsWithKoi;
        //}

        public async Task<List<ShowModel>> GetShowsWithKoiByUserIdAsync(int userId)
        {

            var shows = _context.Shows
                .Include(s => s.RefereeDetails)
                .Where(s => s.Status == "scoring" &&
                            s.RefereeDetails.Any(rd => rd.UserId == userId && s.Id == rd.ShowId))
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Registrations)
                    .ThenInclude(r => r.Koi)
                    .ThenInclude(v => v.Variety)
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Criteria)
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Registrations)
                    .ThenInclude(r => r.Media)
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Registrations)
                    .ThenInclude(r => r.Scores);
            var listShow = await shows.Select(sh => new ShowModel
                 {
                     ShowId = sh.Id,
                     ShowTitle = sh.Title,
                     ShowStatus = sh.Status,
                     
                     ShowGroups = sh.Groups.Select(g => new GroupModel
                     {
                         GroupId = g.Id,
                         GroupName = g.Name,
                         //Scored = g.Registrations.Where(r => r.GroupId == g.Id ).Count(r => r.Status.ToLower().Equals("scored")),
                         //AmountNotScored = g.Registrations.Where(r => r.GroupId == g.Id).Count(r => r.Status.ToLower().Equals("accepted")),
                         Kois = g.Registrations.Where(r => r.Status.ToLower().Equals("accepted")).Select(r => new KoiModel
                         {
                             KoiID = r.Koi!.Id,
                             RegistrationId = r.Id,
                             KoiName = r.Koi.Name,
                             Image1 = r.Media!.Image1,
                             Image2 = r.Media.Image2,
                             KoiSize = r.Size,
                             Image3 = r.Media.Image3,
                             Video = r.Media.Video,
                             VarietyName = r.Koi.Variety.Name,
                             isScored = r.Scores.Any(s => s.RegistrationId == r.Id && s.RefereeDetailId == sh.RefereeDetails.FirstOrDefault(rd => rd.UserId == userId)!.Id),
                             //isScored = r.Status == "Scored",
                             criterions = g.Criteria.Select(c => new CriterionModel
                             {
                                 Id = c.Id,
                                 Name = c.Name,
                                 Percentage = c.Percentage,
                                 Description = c.Description,
                                 Score1 = c.Scores
                                     .Where(score => score.CriteriaId == c.Id && score.RegistrationId == r.Id && score.RefereeDetailId == sh.RefereeDetails.First(rd => rd.UserId == userId).Id)
                                     .Select(score => score.Score1)
                                     .FirstOrDefault()
                             }).ToList()
                         }).ToList()
                     }).ToList()
                 })
                .ToListAsync();
            return listShow;   
        }


        public async Task<List<RefereeModel>> GetAllRefereeByShowAsync(int showId)
        {
            var show = await _context.Shows
                .Include(s => s.RefereeDetails)
                .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(s => s.Id == showId);

            var referees = show!.RefereeDetails.Where(r => r.User!.Status == true)
                                               .Select(rd => new RefereeModel
                                                {
                                                   Id = rd.User!.Id,
                                                    RefereeId = rd.Id,
                                                    RefereeName = rd.User!.Name,
                                                }).ToList();

            return referees;
        }


        public async Task<bool> AddRefereeToShowAsync(List<int> referees, int showId)
        {
            var show = await _context.Shows
                .Include(s => s.RefereeDetails)
                .FirstOrDefaultAsync(s => s.Id == showId);

            // add referee to show
            foreach (var refereeId in referees)
            {
                var referee = await _context.Users
                                        .Where(u => u.RoleId == 3 && u.Status == true)
                                        .FirstOrDefaultAsync(rd => rd.Id == refereeId);

                if (referee != null && !show!.RefereeDetails.Any(rd => rd.UserId == refereeId))
                {
                    var refereeDetail = new RefereeDetail
                    {
                        ShowId = showId,
                        UserId = refereeId
                    };

                    show!.RefereeDetails.Add(refereeDetail);
                }
            }

            int result = await _context.SaveChangesAsync();
            if (result == 0) return false;
            return true;
        }

        public async Task<RefereeModel> GetRefereeDetailById(int refereeDetailId)
        {
            var refereeDetail = await _context.RefereeDetails
                .Include(rd => rd.Show)
                .Include(rd => rd.User)
                .SingleOrDefaultAsync(r => r.Id == refereeDetailId);
            RefereeModel result = null!;
            if (refereeDetail != null)
                result = new RefereeModel()
                {
                    RefereeId = refereeDetail.Id,
                    RefereeName = refereeDetail.User!.Name,
                    ShowTookOnStatus = refereeDetail.Show!.Status,
                };
            return result;
        }

        public async Task<bool> RemoveRefereeDetailFromShow(int refereeDetailId)
        {
            bool result = false;
            var refereeDetail = await _context.RefereeDetails
                .SingleOrDefaultAsync(r => r.Id == refereeDetailId);
            if (refereeDetail != null)
            {
                _context.RefereeDetails.Remove(refereeDetail);
                await _context.SaveChangesAsync();
                result = true;
            }
            return result;
        }

        public async Task<List<UserModel>> GetAllRefereeAsync()
        {
            var referees = await _context.Users
                .Where(u => u.RoleId == 3 && u.Status == true)
                .Select(u => new UserModel
                {
                    Id = u.Id,
                    Name = u.Name,
                    
         }).ToListAsync();

            return referees;
        }
    }
}
