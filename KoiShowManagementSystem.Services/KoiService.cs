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
            var result = _repository.Koi.GetAllKoiByUserId(id);
            return result;
        }

        public Task<KoiModel?> GetKoiDetail(int koiId)
        {
            var result = _repository.Koi.GetKoi(koiId);
            return result;
        }
    }
}
