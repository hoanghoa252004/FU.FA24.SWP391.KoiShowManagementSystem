using KoiShowManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IRoleRepository : IGenericRepository<Role>
    {
        //Task<string> GetRoleTitle(int id);
        Task<int> GetRoleId(string title);
    }
}
