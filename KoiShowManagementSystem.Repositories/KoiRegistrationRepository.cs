using KoiShowManagementSystem.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class KoiRegistrationRepository : GenericRepository<KoiRegistration>, IKoiRegistrationRepository
    {
        public KoiRegistrationRepository(DbContext dbContext) : base(dbContext) { }

        public async Task<IEnumerable<KoiRegistration>> GetByUserID(int id)
        {
            IEnumerable<KoiRegistration> result = null!;
            result = (await _dbContext.Set<KoiRegistration>().ToListAsync())
                        .Where(koiRegist => koiRegist.UserId == id);
            return result;
            
        }
    }
}
