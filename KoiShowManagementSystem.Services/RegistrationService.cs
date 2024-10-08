using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly Repository _repository;
        private readonly JwtServices _jwtServices;
        private readonly S3UploadService _s3Service;
        public RegistrationService(JwtServices jwtServices, Repository repository, S3UploadService s3Service)
        {
            _jwtServices = jwtServices;
            _repository = repository;
            _s3Service = s3Service;
        }

        // 1. GET KOI REGISTRATION BY MEMBER:
        public async Task<IEnumerable<RegistrationModel>> GetMyKoiRegistration(string status)
        {
            IEnumerable<RegistrationModel> result= null!;
            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            IEnumerable<RegistrationModel> myKoiRegistrations = await _repository.KoiRegistrations.GetKoiByUserID(id);
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
