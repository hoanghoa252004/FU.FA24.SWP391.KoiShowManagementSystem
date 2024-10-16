using KoiShowManagementSystem.DTOs.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IVarietyService
    {
        Task<List<VarietyModel>> GetAllVarietiesByShow(int showId);
        Task<List<VarietyModel>> GetAllVarieties();
    }
}
