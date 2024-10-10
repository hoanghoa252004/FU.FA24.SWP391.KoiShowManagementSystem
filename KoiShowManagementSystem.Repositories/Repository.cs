using KoiShowManagementSystem.Repositories.Helper;
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
        private IKoiRepository? _koi;
        private S3UploadService _uploadService;
        private IRefereeRepository _refereeRepository;
        private IGroupRepository _groupRepository;
        public Repository(KoiShowManagementSystemContext context, S3UploadService _uploadService)
        {
            _context = context;
            this._uploadService = _uploadService;
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
        public IKoiRepository Koi
        {
            get
            {
                if (this._koi == null)
                {
                    this._koi = new KoiRepository(_context);
                }
                return this._koi;
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
                    this._registration = new RegistrationRepository(_context, _uploadService);
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
                    this._show = new ShowRepository(_context, _uploadService);
                }
                return this._show;
            }
        }
        public IRefereeRepository Referees
        {
            get
            {
                if (this._refereeRepository == null)
                {
                    this._refereeRepository = new RefereeRepository(_context);
                }
                return this._refereeRepository;
            }
        }
        public IGroupRepository Groups
        {
            get
            {
                if (this._groupRepository == null)
                {
                    this._groupRepository = new GroupRepository(_context);
                }
                return this._groupRepository;
            }
        }
    }
}
