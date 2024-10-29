using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Repositories.Helper;
using KoiShowManagementSystem.Services.Helper;
using Microsoft.AspNetCore.Http;
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
            var result = await _repository.Show.GetShowDetailsAsync(showId);

            return result;
        }

        public async Task<(int TotalItems, List<ShowModel> Shows)> SearchShow(int pageIndex, int pageSize, string keyword)
        {
            var (totalItems, shows) = await _repository.Show.SearchShowAsync(pageIndex, pageSize, keyword);
            return (totalItems, shows);
        }



        public async Task<List<ShowModel>> GetClosestShow()
        {
            // catch the exception from the repository
            try
            {
                var result = await _repository.Show.GetClosestShowAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<int> CreateAShow(ShowDTO dto)
        {

            return await _repository.Show.AddNewShow(dto);
        }


        public async Task<bool> UpdateShow(ShowDTO dto)
        {
            return await _repository.Show.UpdateAShow(dto);
        }

        public async Task<List<ShowModel>> GetAllShow()
        {
            return await _repository.Show.GetAllShow();
        }

        public async Task ChangeStatusShow(string status, int showId)
        {
            await _repository.Show.ChangeShowStatus(status, showId);
        }

        public async Task<bool> DeleteShow(int showId)
        {
            return await _repository.Show.DeleteShowAsync(showId);

        }
    }
}
