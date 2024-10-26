using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly Repository _repository;
        public DashboardService(Repository repository)
        {
            _repository = repository;
        }

        private bool CheckQuantityOfRecentShow(int quantityOfRecentShow) => quantityOfRecentShow > 0 ? true : false;
        public async Task<List<DashboardRegistrationModel>> GetRegistrationDashboard(int quantityOfRecentShow)
        {
            if (CheckQuantityOfRecentShow(quantityOfRecentShow) == false)
                throw new Exception("Failed: How can a quantity could be zero OR negative ?");
            var result = await _repository.Show.CountRegistrationsOfRecentShowAsync(quantityOfRecentShow);
            return result;
        }

        public async Task<List<DashboardRevenueModel>> GetRevenueDashboard(int quantityOfRecentShow)
        {
            if (CheckQuantityOfRecentShow(quantityOfRecentShow) == false)
                throw new Exception("Failed: How can a quantity could be zero OR negative ?");
            var result = await _repository.Show.CalculateRevenueOfRecentShowAsync(quantityOfRecentShow);
            return result;
        }

        public async Task<List<DashboardVarietyModel>> GetVarietyDashboard(int quantityOfRecentShow)
        {
            if (CheckQuantityOfRecentShow(quantityOfRecentShow) == false)
                throw new Exception("Failed: How can a quantity could be zero OR negative ?");
            var result = await _repository.Show.CountKoiVarietyOfRecentShowAsync(quantityOfRecentShow);
            return result;
        }
    }
}
