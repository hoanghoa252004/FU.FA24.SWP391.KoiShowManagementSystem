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
        public async Task<bool> SaveScoresAsync(List<ScoreDTO> scores)
        {
            try
            {
                //var status = await _context.Shows.Include(s => s.Groups)
                //    .Include(s => s.).Where(s => s.Status == "On Going").ToListAsync();
                ////if (status != null)
                ////{
                    foreach (var scoreDto in scores)
                    {
                        var registration = await _context.Registrations
                            .FirstOrDefaultAsync(r => r.KoiId == scoreDto.KoiId);
                        if (registration == null)
                        {
                            Console.WriteLine($"Registration not found for KoiId: {scoreDto.KoiId}");
                            continue;
                        }

                        var criterion = await _context.Criteria
                            .FirstOrDefaultAsync(c => c.Id == scoreDto.CriterionId);
                        if (criterion == null)
                        {
                            Console.WriteLine($"Criterion not found for CriterionId: {scoreDto.CriterionId}");
                            continue;
                        }

                        var existingScore = await _context.Scores
                            .FirstOrDefaultAsync(s => s.RegistrationId == registration.Id
                                                   && s.CriteriaId == scoreDto.CriterionId
                                                   && s.RefereeDetailId == scoreDto.RefereeDetailId);

                        if (existingScore != null)
                        {
                            existingScore.Score1 = scoreDto.ScoreValue;
                            _context.Scores.Update(existingScore);
                        }
                        else
                        {
                            var newScore = new Score
                            {
                                RegistrationId = registration.Id,
                                RefereeDetailId = scoreDto.RefereeDetailId,
                                Score1 = scoreDto.ScoreValue,
                                CriteriaId = scoreDto.CriterionId,
                            };
                            await _context.Scores.AddAsync(newScore);
                        }
                    }

                    await _context.SaveChangesAsync();
                    return true;
                //}
                //return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }








    }
}
