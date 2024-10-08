using KoiShowManagementSystem.Repositories.MyDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class Repository
    {
        private KoiShowManagementSystemContext _context;
        private IUserRepository? _users;
        private IRoleRepository? _role;
        private IRegistrationRepository? _registration;
        private IShowRepository? _show;
        public Repository(KoiShowManagementSystemContext context)
        {
            _context = context;
        }
        // (*) Properties:
        public IUserRepository Users
        {
            get
            {
                if (this._users == null)
                {
                    this._users = new UserRepository(_context);
                }
                return this._users;
            }
        }
        public IRoleRepository Roles
        {
            get
            {
                if (this._role == null)
                {
                    this._role = new RoleRepository(_context);
                }
                return this._role;
            }
        }
        public IRegistrationRepository Registrations
        {
            get
            {
                if (this._registration == null)
                {
                    this._registration = new RegistrationRepository(_context);
                }
                return this._registration;
            }
        }
        public IShowRepository Show
        {
            get
            {
                if (this._show == null)
                {
                    this._show = new ShowRepository(_context);
                }
                return this._show;
            }
        }
    }
}
