using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Repositories.Helper;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public class KoiService : IKoiService
    {
        private readonly Repository _repository;

        public KoiService(Repository repository, JwtServices jwtServices)
        {
            _repository = repository;
        }
        public Task<List<KoiModel>> GetKoiByUserId(int userId)
        {
            //implement this method
            var result = _repository.Koi.GetAllKoiByUserId(userId);
            return result;
        }

        public Task<KoiModel?> GetKoiDetail(int koiId)
        {
            var result = _repository.Koi.GetKoiDetail(koiId);
            return result;
        }
    }
}
