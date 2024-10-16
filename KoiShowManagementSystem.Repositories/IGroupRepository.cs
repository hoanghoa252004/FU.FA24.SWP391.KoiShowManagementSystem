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
        Task<List<GroupModel>> GetByShowIdAsync(int showId);
        Task<bool> CreateAGroupAsync(GroupDTO group);
        Task<bool> UpdateGroupAsync(GroupDTO group);
        Task<bool> DeleteGroupAsync(int groupId);

        Task<List<GroupModel>> GetAllGroupByShowAsync(int showId);
    }
}
