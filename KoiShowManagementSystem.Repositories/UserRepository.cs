using KoiShowManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context) { }
        public async Task<User?> GetByEmail(string email)
        {
            return await _dbContext.Set<User>().FirstOrDefaultAsync(user => user.Email == email);
        }
    }
}
