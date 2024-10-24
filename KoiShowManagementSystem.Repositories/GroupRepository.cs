using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories.Helper;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Group = KoiShowManagementSystem.Entities.Group;

namespace KoiShowManagementSystem.Repositories
{
    public class GroupRepository : IGroupRepository
    {
        private KoiShowManagementSystemContext _context;
        public GroupRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        public Task<List<GroupModel>> GetByShowIdAsync(int showId)
        {
            var result = (from grp in _context.Groups
                         where grp.ShowId == showId && grp.Status == true
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

        public async Task<bool> UpdateGroupAsync(GroupDTO dto)
        {
             var group = await _context.Groups
                                .Include(g => g.Criteria)
                                .Include(g=> g.Varieties)
                                .FirstOrDefaultAsync(g => g.Id == dto.Id);
            if (group == null)
            {
                return false;
            }
            if(dto.Name != null)
            {
                group.Name = dto.Name;
            }
            if (dto.MinSize != 0)
            {
                group.SizeMin = dto.MinSize;
            }
            if (dto.MaxSize != 0)
            {
                group.SizeMax = dto.MaxSize;
            }
            if (!dto.Varieties.IsNullOrEmpty())
            {
                var varieties = await _context.Varieties.Where(v => dto.Varieties!.Contains(v.Id)).ToListAsync();
                group.Varieties = varieties;
            }
            if (!dto.Criterias.IsNullOrEmpty())
            {
                group.Criteria = dto.Criterias!.Select(c => new Criterion
                {
                    Name = c.Name,
                    Percentage = c.Percentage,
                    Description = c.Description,
                    Status = true,
                }).ToList();
            }
            
            int result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }
        public async Task<bool> CreateAGroupAsync(GroupDTO dto)
        {
            // check varieties of dto and eliminate the duplicate data


            var show = await _context.Shows
                            .FirstOrDefaultAsync(s => s.Id == dto.ShowId);
            if (show == null)
            {
                return false;
            }
            var group = new Group()
            {
                Name = dto.Name,
                SizeMax = dto.MaxSize,
                SizeMin = dto.MinSize,
                ShowId = dto.ShowId,
                Status = true,
            };
            if (!dto.Criterias.IsNullOrEmpty())
            {
                group.Criteria = dto.Criterias!.Select(c => new Criterion
                {
                    Name = c.Name,
                    Percentage = c.Percentage,
                    Description = c.Description,
                    Status = true,
                }).ToList();
            }
            if (!dto.Varieties.IsNullOrEmpty())
            {
                group.Varieties = _context.Varieties.Where(v => dto.Varieties!.Contains(v.Id)).ToList();
            }
            show.Groups.Add(group);

            int result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<bool> DeleteGroupAsync(int groupId)
        {
            if (groupId < 1)
            {
                return false;
            }
            var group = await _context.Groups
                               .FirstOrDefaultAsync(g => g.Id == groupId);
            if (group == null)
            {
                return false;
            }
            group.Status = false;
            int result = await _context.SaveChangesAsync();
            if (result > 0) return true;
            return false;
        }

        public async Task<List<GroupModel>> GetAllGroupByShowAsync(int showId)
        {
            List<GroupModel> result = new List<GroupModel>();
            var show = await _context.Shows
                            .FirstOrDefaultAsync(s => s.Id == showId);

            result = await _context.Groups
                            .Where(g => g.ShowId == showId && g.Status == true)
                            .Select(g => new GroupModel()
                            {
                                GroupId = g.Id,
                                GroupName = g.Name,
                                SizeMax = g.SizeMax,
                                SizeMin = g.SizeMin,
                                Varieties = g.Varieties.Select(v => new VarietyModel()
                                {
                                    VarietyId = v.Id,
                                    VarietyName = v.Name,
                                }).ToList(),
                                Criterion = g.Criteria.Select(c => new CriterionModel()
                                {
                                    CriterionId = c.Id,
                                    CriterionName = c.Name,
                                    Percentage = c.Percentage,
                                    Description = c.Description,
                                }).ToList(),
                                Quantity_registration = g.Registrations.Count(r => r.Status.ToLower().Equals("accepted") || r.Status.ToLower().Equals("scored")),
                                Quantity_scored_registration = g.Registrations.Count(r => r.TotalScore != null),
                            }).ToListAsync();

            
            return result;
        }
    }
}
