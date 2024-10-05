using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public class ShowService : IShowService
    {

        private readonly Repository _repository;

        public ShowService(Repository repository, JwtServices jwtServices)
        {
            _repository = repository;
        }

        public async Task<ShowModel?> GetShowDetails(int showId)
        {
            var result = await _repository.KoiShow.GetShowDetailsAsync(showId);

            return result;
        }


        public async Task<(int TotalItems, List<ShowModel> Shows)> SearchShow(int pageIndex, int pageSize, string keyword)
        {
            var (totalItems, shows) = await _repository.KoiShow.SearchShowAsync(pageIndex, pageSize, keyword);

            //var result = shows.Select(s => new ShowModel
            //{
            //    ShowId = s.ShowId,
            //    ShowTitle = s.ShowTitle,
            //    ShowBanner = s.ShowBanner,
            //    StartDate = s.StartDate,
            //    RegistrationStartDate = s.RegistrationStartDate,
            //    RegistrationCloseDate = s.RegistrationCloseDate,
            //    ShowStatus = s.ShowStatus
            //}).ToList();

            return (totalItems, shows);
        }


        public async Task<(int TotalItems, IEnumerable<KoiModel> Kois)> GetKoiByShowId(int pageIndex, int pageSize, int showId)
        {
            var result = await _repository.KoiShow.GetKoiByShowIdAsync(pageIndex,pageSize,showId);

            return result;
        }

        public async Task<KoiModel?> GetKoiDetail(int koiId)
        {
            var result = await _repository.KoiShow.GetKoiDetailAsync(koiId);
            return result;
        }

        public async Task<List<ShowModel>> GetClosestShow()
        {
            // catch the exception from the repository
            try
            {
                var result = await _repository.KoiShow.GetClosestShowAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }




    }
}
