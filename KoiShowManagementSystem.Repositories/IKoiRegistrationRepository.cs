﻿using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IKoiRegistrationRepository
    {
        Task<IEnumerable<KoiRegistModel>> GetKoiRegistrationByUserID(int id);
    }
}
