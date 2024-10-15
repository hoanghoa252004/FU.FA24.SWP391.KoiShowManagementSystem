using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IGroupRepository
    {
        Task<List<GroupModel>> GetByShowId(int showId);
        Task<bool> CreateAGroup(GroupDTO group);
        Task<bool> UpdateGroup(GroupDTO group);
        Task<bool> DeleteGroup(int groupId);
    }
}
