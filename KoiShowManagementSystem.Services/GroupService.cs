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
    public class GroupService : IGroupService
    {
        private readonly Repository _repository;

        public GroupService(Repository repository, JwtServices jwtServices)
        {
            _repository = repository;
        }

        // implement all method
        public  Task<bool> AddGroupToShow(GroupDTO group)
        {
            return  _repository.Groups.CreateAGroupAsync(group);
        }

        public  Task<bool> UpdateGroup(GroupDTO group)
        {
            return  _repository.Groups.UpdateGroupAsync(group);
        }

        public  Task<bool> DeleteGroup(int groupId)
        {
            return  _repository.Groups.DeleteGroupAsync(groupId);
        }

        public  Task<List<GroupModel>> GetAllGroupByShow(int showId)
        {
            return  _repository.Groups.GetAllGroupByShowAsync(showId);
        }

        public Task<List<GroupModel>> ReviewGroupScore(int showId)
        {
            _repository.Scores.CalculateTotalScoreAsync(showId);
            _repository.Scores.CalculateRankAsync(showId);
            return _repository.Groups.GetAllGroupByShowAsync(showId);
        }
    }
}
