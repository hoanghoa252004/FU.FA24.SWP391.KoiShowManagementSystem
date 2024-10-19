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

        

        public async Task<bool> SaveScoresAsync(RefereeScoreDTO refereeScore, int UserId)
        {
            try
            {
                //duyệt từng Json gửi về có gì
                foreach (var scoreDetail in refereeScore.ScoreDetail)
                {
                    //Lấy ra cái đơn cần chấm
                    var registration = await _context.Registrations
                        .FirstOrDefaultAsync(r => r.Id    == scoreDetail.RegistraionId);
                    if (registration == null)
                    {
                        Console.WriteLine($"Registration not found for KoiId: {scoreDetail.RegistraionId}");
                        continue;
                    }
                    //Lấy điểm từ Json gửi về
                    foreach (var score in scoreDetail.Scores)
                    {
                        //Lấy ra tiêu chí chấm
                        var criterion = await _context.Criteria
                            .FirstOrDefaultAsync(c => c.Id == score.CriterionId);
                        if (criterion == null)
                        {
                            Console.WriteLine($"Criterion not found for CriterionId: {score.CriterionId}");
                            continue;
                        }
                        //lấy điểm giám khảo  input rồi nhân với Percentage 
                        var adjustedScore = score.Score * (criterion.Percentage)/100;

                        //Kiểm tra xem nó tồn tại không
                        var existingScore = await _context.Scores
                            .FirstOrDefaultAsync(s => s.RegistrationId == registration.Id
                                                   && s.CriteriaId == score.CriterionId
                                                   && s.RefereeDetailId == UserId);
                        //Có thì update
                        if (existingScore != null)
                        {
                            existingScore.Score1 = adjustedScore;
                            _context.Scores.Update(existingScore);
                        }
                        else //Không thì thêm mới
                        {
                            var newScore = new Score
                            {
                                RegistrationId = registration.Id,
                                RefereeDetailId = UserId,
                                Score1 = adjustedScore,
                                CriteriaId = score.CriterionId,
                            };
                            await _context.Scores.AddAsync(newScore);
                        }

                    } // end duyệt điểm
                    
                    registration.Status = "Scored";
                    _context.Registrations.Update(registration);
                }// end duyệt Json gửi về

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return false;
            }
        }

        public async Task CalculateTotalScoreAsync(int showId)
        {
            var show = _context.Shows
                .Include(s => s.RefereeDetails)
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Criteria)
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Registrations.Where(r => r.TotalScore == null))
                .ThenInclude(r => r.Scores)
                .FirstOrDefault(s => s.Id == showId);
            var groups = show!.Groups;
            int refereeCountForShow = show.RefereeDetails.Count;

            foreach (var group in groups)
            {
                var criterias = group.Criteria;
                var registrations = group.Registrations;
                int criteriaCountForGroup = criterias.Count;
                int totalScoreRecords = criteriaCountForGroup * refereeCountForShow;
                
                foreach (var registration in registrations)
                {
                    if (registration.Scores.Count == totalScoreRecords)
                    {
                        decimal? totalScore = registration.Scores
                                            .Sum(score => score.Score1 * criterias
                                            .First(c => c.Id == score.CriteriaId).Percentage);
                        totalScore /= refereeCountForShow;
                        registration.TotalScore = totalScore;
                    }                   
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
