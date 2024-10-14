﻿using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IKoiRepository
    {
        Task<List<KoiModel>> GetAllKoiByUserId(int userId);
        Task<KoiModel?> GetKoi(int koiId);
        Task<bool> CreateKoi(KoiDTO koi, int userId);
        Task<bool> UpdateKoi(KoiDTO koi);
        Task<bool> DeleteKoi(int koiId);
    }
}
