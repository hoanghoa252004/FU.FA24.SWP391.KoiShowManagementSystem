using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IVarietyRepository
    {
        Task<List<VarietyModel>> GetAllVarietiesByShowAsync(int showId);
        Task<List<VarietyModel>> GetAllVarietiesAsync();
        Task AddAsync(Variety variety);
        Task UpdateAsync(Variety variety);
        Task DeleteAsync(int id);
        Task<Variety?> GetByIdAsync(int id);
    }
}
