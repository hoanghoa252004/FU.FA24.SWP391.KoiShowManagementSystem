using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Repositories.Helper;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private KoiShowManagementSystemContext _context;
        public GroupRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        public Task<List<GroupModel>> GetByShowId(int showId)
        {
            var result = (from grp in _context.Groups
                         where grp.ShowId == showId
                         select new GroupModel()
                         {
                             GroupId = grp.Id,
                             SizeMax = grp.SizeMax,
                             SizeMin = grp.SizeMin,
                             Varieties = grp.Varieties.Select(var => new VarietyModel()
                             {
                                 VarietyId = var.Id,
                                 VarietyName = var.Name,
                             }).ToList(),
                            Registrations = grp.Registrations.Select(reg => new RegistrationModel()
                            {
                                Id = reg.Id,
                                KoiID = reg.KoiId,
                                CreateDate = reg.CreateDate,
                            }).ToList(),
                         }).ToListAsync();
            return result;
        }
    }
}
