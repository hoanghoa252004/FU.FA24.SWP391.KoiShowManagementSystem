﻿using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IRegistrationService
    {
        Task<IEnumerable<RegistrationModel>> GetMyRegistration(string status);
        Task CreateRegistration(RegistrationFormModel dto);
        Task<(int TotalItems, IEnumerable<RegistrationModel> Kois)> GetRegistrationByShow(int pageIndex, int pageSize, int showId);
        Task<RegistrationModel?> GetRegistration(int registrationId);
    }
}
