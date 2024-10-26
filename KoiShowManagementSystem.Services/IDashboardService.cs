using KoiShowManagementSystem.DTOs.BusinessModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IDashboardService
    {
        Task<List<DashboardRegistrationModel>> GetRegistrationDashboard(int quantityOfRecentShow);
        Task<List<DashboardRevenueModel>> GetRevenueDashboard(int quantityOfRecentShow);
        Task<List<DashboardVarietyModel>> GetVarietyDashboard(int quantityOfRecentShow);
    }
}
