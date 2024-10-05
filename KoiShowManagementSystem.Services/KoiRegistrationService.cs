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
    public class KoiRegistrationService : IKoiRegistrationService
    {
        private readonly Repository _repository;
        private readonly JwtServices _jwtServices;
        public KoiRegistrationService(JwtServices jwtServices, Repository repository)
        {
            _jwtServices = jwtServices;
            _repository = repository;
        }

        // 1. GET KOI REGISTRATION BY MEMBER:
        public async Task<IEnumerable<KoiRegistModel>> GetMyKoiRegistration(string status)
        {
            IEnumerable<KoiRegistModel> result= null!;
            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            IEnumerable<KoiRegistModel> myKoiRegistrations = await _repository.KoiRegistrations.GetKoiRegistrationByUserID(id);
            status = status.ToLower();
            switch (status)
            {
                case "inprocess":
                    {
                        result = myKoiRegistrations.Where(koi => koi.Status == "Accepted" || koi.Status == "Rejected" || koi.Status == "Pending").ToList();
                        break;
                    }
                case "draft":
                    {
                        result = myKoiRegistrations.Where(koi => koi.Status == "Draft").ToList();
                        break;
                    }
                case "scored":
                    {
                        result = myKoiRegistrations.Where(koi => koi.Status == "Scored").ToList();
                        break;
                    }
            }
            return result;
        }
    }
}
