using KoiShowManagementSystem.DTOs.BusinessModels;
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
                                StatusScored = g.Registrations.Count(r => r.Status == "Scored"),
                                StatusAccepted = g.Registrations.Count(r => r.Status == "Accepted")
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

        //bởi vì bảng Score có 3 khóa ngoại nên nhận cả 3 với điểm nữa
        public async Task<bool> SaveScoreAsync(int criterionId, int koiId, int refereeDetailId, decimal scoreValue)
        {
            try
            {
                // Fetch the registration for the given Koi
                var registration = await _context.Registrations
                    .Where(r => r.KoiId == koiId).FirstOrDefaultAsync();

                if (registration == null)
                {
                    Console.WriteLine("Registration not found for KoiId: " + koiId);
                    return false;
                }

                var criterion = await _context.Criteria
                    .FirstOrDefaultAsync(c => c.Id == criterionId);

                if (criterion == null)
                {
                    Console.WriteLine("Criterion not found for CriterionId: " + criterionId);
                    return false; 
                }

                
                Score score = new Score()
                {
                    RegistrationId = registration.Id,
                    RefereeDetailId = refereeDetailId,
                    Score1 = scoreValue , 
                    CriteriaId = criterionId,
                };

               
                await _context.Set<Score>().AddAsync(score);              
                await _context.SaveChangesAsync();
                return true; // Successfully saved
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }








    }
}
