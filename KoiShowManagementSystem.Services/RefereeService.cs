using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public class RefereeService : IRefereeService
    {
        private readonly Repository _repository;
        private readonly JwtServices _jwtServices;

        public RefereeService(Repository repository, JwtServices jwtServices)
        {
            _repository = repository;
            _jwtServices = jwtServices;
        }
        public async Task<List<ShowModel>> GetListShow()
        {
            var result = await _repository.Referees.GetListShowAsync();

            return result;
        }
        public async Task<List<KoiModel>> GetKoiDetailsByGroupId(int groupId)
        {
            var result = await _repository.Referees.GetKoiDetailsByGroupIdAsync(groupId);

            return result;
        }

        public async Task<bool> SaveScoreFromReferee(RefereeScoreDTO dto)
        {
            var userId = _jwtServices.GetIdAndRoleFromToken().userId;
            var result = await _repository.Scores.SaveScoresAsync(dto, userId);
            return result;
        }
        public async Task<List<ShowModel>> GetShowsWithKoiByUserIdAsync()
        {
            var userId = _jwtServices.GetIdAndRoleFromToken().userId;
            var result = await _repository.Referees.GetShowsWithKoiByUserIdAsync(userId);
            return result;
        }
        public async Task<List<RefereeModel>> GetAllRefereeByShow(int showId)
        {
            var result = await _repository.Referees.GetAllRefereeByShowAsync(showId);
            return result;
        }

        public async Task<bool> AddRefereeToShow(List<int> referees, int showId)
        {
            var result = await _repository.Referees.AddRefereeToShowAsync(referees, showId);
            return result;
        }
    }
}
