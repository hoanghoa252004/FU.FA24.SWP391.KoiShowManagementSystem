using KoiShowManagementSystem.DTOs.BusinessModels;
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
    public class RoleRepository : IRoleRepository
    {
        private KoiShowManagementSystemContext _context;
        public RoleRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        public async Task<List<RoleDTO>> GetAllRoles()
        {
            return await _context.Roles.Select(r => new RoleDTO()
            {
                Id = r.Id,
                Title = r.Title,
                Status = r.Status,
            }).ToListAsync();
        }
    }
}
