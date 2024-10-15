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
        public async Task<bool> AddGroupToShow(GroupDTO group)
        {
            return await _repository.Groups.CreateAGroup(group);
        }

        public async Task<bool> UpdateGroup(GroupDTO group)
        {
            return await _repository.Groups.UpdateGroup(group);
        }

        public async Task<bool> DeleteGroup(int groupId)
        {
            return await _repository.Groups.DeleteGroup(groupId);
        }

    }
}
