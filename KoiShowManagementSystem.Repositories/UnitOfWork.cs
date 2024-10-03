using KoiShowManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private KoiShowManagementSystemContext _context;
        // (*) Fields:
        private IUserRepository? _users;
        private IRoleRepository? _role;
        private IKoiRegistrationRepository? _koiRegistration;
        private GenericRepository<Variety>? _variety;
        private GenericRepository<Illustration>? _illustration;
        private GenericRepository<Show>? _show;
        private GenericRepository<Group>? _group;
        private GenericRepository<RefereeDetail>? _RefereeDetail;
        private IShowRepository _koiShow;
        // ...
        // ...
        // End 
        public UnitOfWork(KoiShowManagementSystemContext context)
        {
            _context = context;
        }

        // (*) Properties:
        public IUserRepository Users
        {
            get
            {
                if(this._users == null)
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

        public IGenericRepository<Variety> Varieties
        {
            get
            {
                if (this._variety == null)
                {
                    this._variety = new GenericRepository<Variety>(_context);
                }
                return this._variety;
            }
        }

        public IGenericRepository<Show> Shows
        {
            get
            {
                if (this._show == null)
                {
                    this._show = new GenericRepository<Show>(_context);
                }
                return this._show;
            }
        }

        public IGenericRepository<Group> Groups
        {
            get
            {
                if (this._group == null)
                {
                    this._group = new GenericRepository<Group>(_context);
                }
                return this._group;
            }
        }

        public IGenericRepository<Illustration> Illustrations
        {
            get
            {
                if (this._illustration == null)
                {
                    this._illustration = new GenericRepository<Illustration>(_context);
                }
                return this._illustration;
            }
        }

        public IGenericRepository<RefereeDetail> RefereeDetails
        {
            get
            {
                if (this._RefereeDetail == null)
                {
                    this._RefereeDetail = new GenericRepository<RefereeDetail>(_context);
                }
                return this._RefereeDetail;
            }
        }

        public IShowRepository KoiShow
        {
            get
            {
                if (this._koiShow == null)
                {
                    this._koiShow = new KoiShowRepository(_context);
                }
                return this._koiShow;
            }
        }

        // ..
        // ..
        // End
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
