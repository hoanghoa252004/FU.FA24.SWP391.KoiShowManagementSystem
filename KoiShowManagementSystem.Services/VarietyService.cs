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
    public class VarietyService : IVarietyService
    {
        private readonly Repository _repository;

        public VarietyService(Repository repository, JwtServices jwtServices)
        {
            _repository = repository;
        }
        public Task<List<VarietyModel>> GetAllVarietiesByShow(int showId)
        {
            var result = _repository.Varieties.GetAllVarietiesByShowAsync(showId);
            return result;
        }
        public  Task<List<VarietyModel>> GetAllVarieties()
        {
            var result =  _repository.Varieties.GetAllVarietiesAsync();
            return result;
        }

    }
}
