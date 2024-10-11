using KoiShowManagementSystem.DTOs.Request;
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
    public class ScoreRepository : IScoreRepository
    {
        private readonly KoiShowManagementSystemContext _context;
        public ScoreRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        public async Task<bool> SaveScoresAsync(RefereeScoreDTO refereeScore)
        {
            try
            {
                foreach (var scoreDetail in refereeScore.ScoreDetail)
                {
                    var registration = await _context.Registrations
                        .FirstOrDefaultAsync(r => r.KoiId == scoreDetail.KoiId);
                    if (registration == null)
                    {
                        Console.WriteLine($"Registration not found for KoiId: {scoreDetail.KoiId}");
                        continue;
                    }

                    foreach (var score in scoreDetail.Scores)
                    {
                        var criterion = await _context.Criteria
                            .FirstOrDefaultAsync(c => c.Id == score.CriterionId);
                        if (criterion == null)
                        {
                            Console.WriteLine($"Criterion not found for CriterionId: {score.CriterionId}");
                            continue;
                        }

                        var existingScore = await _context.Scores
                            .FirstOrDefaultAsync(s => s.RegistrationId == registration.Id
                                                   && s.CriteriaId == score.CriterionId
                                                   && s.RefereeDetailId == refereeScore.RefereeDetailId);

                        if (existingScore != null)
                        {
                            existingScore.Score1 = score.Score;
                            _context.Scores.Update(existingScore);
                        }
                        else
                        {
                            var newScore = new Score
                            {
                                RegistrationId = registration.Id,
                                RefereeDetailId = refereeScore.RefereeDetailId,
                                Score1 = score.Score,
                                CriteriaId = score.CriterionId,
                            };
                            await _context.Scores.AddAsync(newScore);
                        }
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }
    }
}
