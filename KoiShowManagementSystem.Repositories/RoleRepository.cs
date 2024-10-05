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
        public async Task<int> GetRoleId(string title)
        {
            var role = await _context.Set<Role>().FirstOrDefaultAsync(role => role.Title.Contains(title));
            return role!.Id;
        }

        public async Task<string> GetRoleTitle(int id)
        {
            var role = await _context.Set<Role>().FirstOrDefaultAsync(role => role.Id == id);
            return role!.Title;
        }
    }
}
