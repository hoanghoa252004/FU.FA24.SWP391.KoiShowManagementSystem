﻿using KoiShowManagementSystem.DTOs.Request;
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
                var showId =  _context.Shows.Where(s => s.Status.ToLower()!.Equals("scoring")).Single().Id;
                var refereeId = await _context.RefereeDetails.Where(r => r.UserId == UserId && r.ShowId == showId).FirstOrDefaultAsync();
                foreach (var scoreDetail in refereeScore.ScoreDetail)
                {
                    var registration = await _context.Registrations
                        .FirstOrDefaultAsync(r => r.Id == scoreDetail.RegistraionId);
                    if (registration == null)
                    {
                        Console.WriteLine($"Registration not found for KoiId: {scoreDetail.RegistraionId}");
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
                                                   && s.RefereeDetailId == refereeId.Id);

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
                                RefereeDetailId = refereeId.Id,
                                Score1 = score.Score,
                                CriteriaId = score.CriterionId,
                            };
                            await _context.Scores.AddAsync(newScore);
                        }
                    }
                    //registration.Status = "Scored";
                    _context.Registrations.Update(registration);
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

        public async Task CalculateTotalScoreAsync(int showId)
        {
            var show = _context.Shows
                .Include(s => s.RefereeDetails)
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Criteria)
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Registrations)
                           .ThenInclude(r => r.Scores)
                .FirstOrDefault(s => s.Id == showId);
            var groups = show!.Groups;
            int refereeCountForShow = show.RefereeDetails.Count;

            foreach (var group in groups)
            {
                var criterias = group.Criteria;
                var registrations = group.Registrations.Where(r => r.TotalScore == null && r.Status!.ToLower().Equals("accepted"));
                int criteriaCountForGroup = criterias.Count;
                int totalScoreRecords = criteriaCountForGroup * refereeCountForShow;
                
                foreach (var registration in registrations)
                {
                    if (registration.Scores.Count == totalScoreRecords)
                    {
                        decimal? totalScore = registration.Scores
                                            .Sum(score => score.Score1 * criterias
                                            .First(c => c.Id == score.CriteriaId).Percentage/100);
                        totalScore /= refereeCountForShow;
                        registration.TotalScore = totalScore;
                    }                   
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task CalculateRankAsync(int showId)
        {
            var show = _context.Shows
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Registrations)
                        .ThenInclude(r => r.Users)
                    .ThenInclude(g => g.Registrations)
                        .ThenInclude(r => r.Scores)
                            .ThenInclude(s => s.Criteria)
                .Include(s => s.Groups)
                    .ThenInclude(g => g.Criteria)
                .FirstOrDefault(s => s.Id == showId);

            var groups = show!.Groups;

            foreach (var group in groups)
            {
                int scoredRegistration = group.Registrations.Count(r => r.TotalScore != null);
                int expectedScoredRegistration = group.Registrations.Count(r => r.Status!.Equals("Accepted", StringComparison.OrdinalIgnoreCase));

                if (scoredRegistration == expectedScoredRegistration)
                {
                    var registrations = group.Registrations
                        .Where(r => r.TotalScore != null && r.Status!.Equals("Accepted", StringComparison.OrdinalIgnoreCase))
                        .Select(r => new
                        {
                            Registration = r,
                            ScoreList = _context.Scores
                                .Where(s => s.RegistrationId == r.Id)
                                .GroupBy(s => s.CriteriaId)
                                .Select(g => new
                                {
                                    CriteriaId = g.Key,
                                    TotalScoreByCriteria = g.Average(s => s.Score1) * g.First().Criteria!.Percentage / 100,
                                    Percentage = g.First().Criteria!.Percentage,
                                })
                                .OrderByDescending(x => x.Percentage)
                                .ToList(),
                        })
                        .OrderByDescending(x => x.ScoreList.Sum(sl => sl.TotalScoreByCriteria));

                    int quantityCriteriaInGroup = group.Criteria.Count;

                    for (int index = 0; index < quantityCriteriaInGroup; index++)
                    {
                        registrations = registrations.ThenByDescending(x => x.ScoreList.ElementAtOrDefault(index)?.TotalScoreByCriteria ?? 0);
                    }

                    int rank = 1;
                    foreach (var regist in registrations)
                    {
                        regist.Registration.Rank = rank;
                        rank++;
                        regist.Registration.Status = "Scored";
                    }
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task CalculateBestVote(int showId)
        {
            var show = _context.Shows.Include(s => s.Groups).ThenInclude(g => g.Registrations).ThenInclude(r => r.Users).FirstOrDefault(s => s.Id == showId);
            var groups = show!.Groups;
            foreach (var group in groups) {
                var bestVoteRegistration = group.Registrations.Where(r => r.Status!
                                                        .Equals("Scored", StringComparison.OrdinalIgnoreCase))
                                                        .OrderByDescending(r => r.Users.Count())
                                                        .FirstOrDefault();
                if (bestVoteRegistration != null)
                {
                    bestVoteRegistration.IsBestVote = true;
                }
            }
            await _context.SaveChangesAsync();
        }



    }
}
