using KoiShowManagementSystem.DTOs.BusinessModels;
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
    }
}
