using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Repositories.MyDbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public class VarietyRepository : IVarietyRepository
    {
        private KoiShowManagementSystemContext _context;
        public VarietyRepository(KoiShowManagementSystemContext context)
        {
            this._context = context;
        }
        public async Task<List<VarietyModel>> GetAllVarietiesByShowAsync(int showId)
        {
            var varieties = await _context.Groups
                .Where(g => g.ShowId == showId)
                .SelectMany(gr => gr.Varieties)
                .Select(v => new VarietyModel
                {
                    VarietyId = v.Id,
                    VarietyName = v.Name
                })
                .ToListAsync();
            return varieties;
        }

        public async Task<List<VarietyModel>> GetAllVarietiesAsync()
        {
            var reuslt = await _context.Varieties.Select(v => new VarietyModel
            {
                VarietyId = v.Id,
                VarietyName = v.Name
            }).ToListAsync();
            return reuslt;
        }

    }
}
