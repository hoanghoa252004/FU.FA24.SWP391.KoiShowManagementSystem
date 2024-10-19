using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Entities;
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
                    VarietyName = v.Name,
                    VarietyStatus = v.Status,
                })
                .ToListAsync();
            return varieties;
        }

        public async Task<List<VarietyModel>> GetAllVarietiesAsync()
        {
            var reuslt = await _context.Varieties.Where(v => v.Status == true).Select(v => new VarietyModel
            {
                VarietyId = v.Id,
                VarietyName = v.Name,
                VarietyStatus = v.Status,
            }).ToListAsync();
            return reuslt;
        }

        public async Task AddAsync(Variety variety)
        {
            await _context.Varieties.AddAsync(variety);
            await _context.SaveChangesAsync();
        }

        // UpdateAsync method in repository
        public async Task UpdateAsync(Variety variety)
        {
            _context.Varieties.Update(variety);
            await _context.SaveChangesAsync();
        }

        // DeleteAsync method in repository
        public async Task DeleteAsync(int id)
        {
            var variety = await _context.Varieties.FindAsync(id);
            if (variety != null)
            {
                _context.Varieties.Remove(variety);
                await _context.SaveChangesAsync();
            }
        }
        public async Task<Variety?> GetByIdAsync(int id)
        {
            return await _context.Varieties.FindAsync(id);
        }

    }
}
