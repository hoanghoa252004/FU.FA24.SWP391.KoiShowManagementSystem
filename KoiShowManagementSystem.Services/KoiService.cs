using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Repositories.Helper;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KoiShowManagementSystem.DTOs.Request;

namespace KoiShowManagementSystem.Services
{
    public class KoiService : IKoiService
    {
        private readonly Repository _repository;
        private readonly JwtServices _jwtServices;
        public KoiService(JwtServices jwtServices, Repository repository)
        {
            _jwtServices = jwtServices;
            _repository = repository;
        }
        public Task<List<KoiModel>> GetKoiByUserId()
        {
            //implement this method
            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            var result = _repository.Koi.GetAllKoiByUserIdAsync(id);
            return result;
        }

        public Task<KoiModel?> GetKoiDetail(int koiId)
        {
            var result = _repository.Koi.GetKoiAsync(koiId);
            return result;
        }

        public Task<bool> CreateKoi(KoiDTO koi)
        {
            int userId = _jwtServices.GetIdAndRoleFromToken().userId;
            var result = _repository.Koi.CreateKoiAsync(koi, userId);
            return result;
        }

        public Task<bool> UpdateKoi(KoiDTO koi)
        {
            var result = _repository.Koi.UpdateKoiAsync(koi);
            return result;
        }

        public Task<bool> DeleteKoi(int koiId)
        {
            var result = _repository.Koi.DeleteKoiAsync(koiId);
            return result;
        }

        public Task<List<KoiModel>> DashboardKoi()
        {
            throw new NotImplementedException();
        }
    }
}
