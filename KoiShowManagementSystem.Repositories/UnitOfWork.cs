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
        private IGenericRepository<Role>? _role;
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

        public IGenericRepository<Role> Roles
        {
            get
            {
                if (this._role == null)
                {
                    this._role = new GenericRepository<Role>(_context);
                }
                return this._role;
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
