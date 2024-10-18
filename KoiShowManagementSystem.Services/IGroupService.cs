using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IGroupService
    {
        public Task<bool> AddGroupToShow(GroupDTO group);
        public Task<bool> UpdateGroup(GroupDTO group);
        public Task<bool> DeleteGroup(int groupId);
        public Task<List<GroupModel>> GetAllGroupByShow(int showId);

        public Task<List<GroupModel>> ReviewGroupScore(int showId);
    }
}
