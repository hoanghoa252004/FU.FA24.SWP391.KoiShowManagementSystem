using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Repositories
{
    public interface IShowRepository
    {
        Task<ShowModel?> GetShowDetailsAsync(int showId);
        Task<(int TotalItems, List<ShowModel>)> SearchShowAsync(int pageIndex, int pageSize, string keyword);
        Task<List<ShowModel>> GetClosestShowAsync();
        Task<List<VarietyModel>> GetAllVarietiesAsync();
        Task<List<ShowModel>> GetAllShow();
        Task<int> AddNewShow(ShowDTO dto);
        //Task<bool> EditAShow(ShowDTO dto);
        Task<bool> ChangeShowStatus(string status, int showId);


    }
}
