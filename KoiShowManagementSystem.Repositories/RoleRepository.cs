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

        public async Task<List<Role>> GetAllRole()
        {
            return await _context.Roles.ToListAsync();
        }
    }
}
