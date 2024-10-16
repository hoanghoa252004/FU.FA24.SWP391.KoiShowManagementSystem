using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
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
                        .OrderByDescending(sh => sh.Status == "On Going")
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
                                Scored = g.Registrations.Count(r => r.Status == "Scored"),
                                AmountNotScored = g.Registrations.Count(r => r.Status == "Accepted")
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
                        CriterionId = c.Id,
                        CriterionName = c.Name
                    }).ToList()
                })).ToListAsync();

            return koiDetails;
        }

        //Gộp lại 
        public async Task<List<ShowModel>> GetShowsWithKoiByUserIdAsync(int userId)
        {
            var showsWithKoi = await _context.Shows
                .Where(sh => sh.Status == "On Going" &&
                             sh.RefereeDetails.Any(rd => rd.UserId == userId)) // Filter by userId instead of refereeId
                .Include(sh => sh.Groups)
                    .ThenInclude(g => g.Registrations)
                .Include(sh => sh.Groups)
                    .ThenInclude(g => g.Criteria)
                .Select(sh => new ShowModel
                {
                    ShowId = sh.Id,
                    ShowTitle = sh.Title,
                    ShowStatus = sh.Status,
                    ShowGroups = sh.Groups.Select(g => new GroupModel
                    {
                        GroupId = g.Id,
                        GroupName = g.Name,
                        Scored = g.Registrations.Count(r => r.Status == "Scored"),
                        AmountNotScored = g.Registrations.Count(r => r.Status == "Accepted"),
                        Kois = g.Registrations.Select(r => new KoiModel
                        {
                            KoiID = r.Koi.Id,
                            KoiName = r.Koi.Name,
                            criterions = g.Criteria.Select(c => new CriterionModel
                            {
                                CriterionId = c.Id,
                                CriterionName = c.Name
                            }).ToList()
                        }).ToList()
                    }).ToList()
                })
                .ToListAsync();

            return showsWithKoi;
        }









    }
}
