﻿using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public interface IRefereeService
    {
        Task<List<ShowModel>> GetListShow();
        Task<List<KoiModel>> GetKoiDetailsByGroupId(int groupId);
        Task<bool> SaveScoreFromReferee(List<ScoreDTO> dto);
    }
}