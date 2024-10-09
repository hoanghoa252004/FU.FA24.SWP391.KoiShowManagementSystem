using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Repositories.MyDbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class RefereeRepository : IRefereeRepository
    {
        private readonly KoiShowManagementSystemContext _context;

        public RefereeRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }

        public Task<ShowModel> GetListShow()
        {
            throw new NotImplementedException();
        }

        public Task ScoreKoi(int koiId)
        {
            throw new NotImplementedException();
        }
    }
}
