using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public class RoleService : IRoleService
    {
        private readonly Repository _repository;
        public RoleService(Repository repository)
        {
            _repository = repository;
        }

        public async Task<List<RoleDTO>> GetAllRoles()
        {
            var roles = (from r in await _repository.Roles.GetAllRoles()
                        where r.Status == true
                        select r).ToList();
            if (roles.Any() == true)
                return roles;
            else
                throw new Exception("Failed: No Role exists !");
        }
    }
}
