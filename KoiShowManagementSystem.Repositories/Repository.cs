using KoiShowManagementSystem.Entities;
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
        private IKoiRegistrationRepository? _koiRegistration;
        private IShowRepository? _koiShow;
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
        public IKoiRegistrationRepository KoiRegistrations
        {
            get
            {
                if (this._koiRegistration == null)
                {
                    this._koiRegistration = new KoiRegistrationRepository(_context);
                }
                return this._koiRegistration;
            }
        }
        public IShowRepository KoiShow
        {
            get
            {
                if (this._koiShow == null)
                {
                    this._koiShow = new ShowRepository(_context);
                }
                return this._koiShow;
            }
        }
    }
}
