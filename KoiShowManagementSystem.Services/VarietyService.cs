using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.Entities;
using KoiShowManagementSystem.Repositories;
using KoiShowManagementSystem.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoiShowManagementSystem.Services
{
    public class VarietyService : IVarietyService
    {
        private readonly Repository _repository;

        public VarietyService(Repository repository, JwtServices jwtServices)
        {
            _repository = repository;
        }
        public Task<List<VarietyModel>> GetAllVarietiesByShow(int showId)
        {
            var result = _repository.Varieties.GetAllVarietiesByShowAsync(showId);
            return result;
        }
        public  Task<List<VarietyModel>> GetAllVarieties()
        {
            var result =  _repository.Varieties.GetAllVarietiesAsync();
            return result;
        }
        public async Task<bool?> CreateVarietyAsync(VarietyModel model)
        {
            var variety = new Variety
            {
                Name = model.VarietyName,
                Origin = model.VarietyOrigin,
                Status = model.VarietyStatus
            };

            await _repository.Varieties.AddAsync(variety);

            // Return the created variety as a model
            return true;
        }

        public async Task<bool?> UpdateVarietyAsync(VarietyModel model)
        {
            var existingVariety = await _repository.Varieties.GetByIdAsync(model.VarietyId);
            if (existingVariety == null)
            {
                return false; // Variety not found
            }

            // Update the existing variety
            existingVariety.Name = model.VarietyName;
            existingVariety.Origin = model.VarietyOrigin;
            existingVariety.Status = model.VarietyStatus;

            await _repository.Varieties.UpdateAsync(existingVariety);

            // Return the updated variety as a model
            return true;
        }

        public async Task<bool> DeleteVarietyAsync(int id)
        {
            var variety = await _repository.Varieties.GetByIdAsync(id);
            if (variety == null)
            {
                return false; // Variety not found
            }

            await _repository.Varieties.DeleteAsync(id);
            return true; // Deletion successful
        }

     
    }
}
