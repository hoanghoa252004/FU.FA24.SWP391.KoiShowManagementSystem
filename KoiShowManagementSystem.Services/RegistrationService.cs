using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Repositories.Helper;
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
        public RegistrationService(JwtServices jwtServices, Repository repository)
        {
            _jwtServices = jwtServices;
            _repository = repository;
        }

        

        // 1. GET REGISTRATION BY MEMBER:
        public async Task<IEnumerable<RegistrationModel>> GetMyKoiRegistration(string status)
        {
            IEnumerable<RegistrationModel> result= null!;
            int id = _jwtServices.GetIdAndRoleFromToken().userId;
            IEnumerable<RegistrationModel> myKoiRegistrations = await _repository.Registrations.GetRegistrationByUserIdAsync(id);
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

        // 2. GET REGISTRATION FORM TO REGISTER A SHOW:
        public Task<RegistrationFormModel?> GetRegistrationForm(int showId)
        {
            var registrationForm = _repository.Registrations.GetRegistrationFormAsync(showId);
            return registrationForm!;
        }

        // 3. CREATE REGISTRATION:
        public async Task CreateRegistration(RegistrationFormModel dto)
        {
            await _repository.Registrations.CreateRegistrationAsync(dto);
        }
    }
}
