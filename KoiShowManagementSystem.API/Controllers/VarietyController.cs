﻿using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VarietyController : ControllerBase
    {
        private readonly IVarietyService _varietyService;
        public VarietyController(IVarietyService varietyService)
        {
            _varietyService = varietyService;
        }

        [HttpGet("get-all-show-varieties")]
        public async Task<IActionResult> GetAllVarieties(int showId)
        {
            try
            {
                var result = await _varietyService.GetAllVarietiesByShow(showId);
                if (result == null) return NotFound(new ApiResponse {
                    Message = "No varieties in show",
                    Payload = result });
                return Ok(new ApiResponse
                {
                    Message = "Success",
                    Payload = result
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse()
                {
                    Message = ex.Message,
                });
            }
        }

        [HttpGet("get-all-varieties")]
        public async Task<IActionResult> GetAllVarieties()
        {
            var result = await _varietyService.GetAllVarieties();
            if (result == null) return NotFound(new ApiResponse
            {
                Message = "No varieties",
                Payload = result
            });
            return Ok(new ApiResponse
            {
                Message = "Success",
                Payload = result
            });
        }

        //[HttpGet("get-variety-by-id")]
        //public async Task<IActionResult> GetVarietyById(int varietyId)
        //{
        //    if (varietyId <= 0)
        //    {
        //        return BadRequest(new ApiResponse { Message = "Invalid Variety ID." });
        //    }

        //    var result = await _varietyService.GetVarietyById(varietyId);
        //    if (result != null)
        //    {
        //        return Ok(new ApiResponse
        //        {
        //            Message = "Success",
        //            Payload = result
        //        });
        //    }

        //    return NotFound(new ApiResponse { Message = "Variety not found." });
        //}

        [Authorize(Roles = "Manager")]
        [HttpPost("create-variety")]
        public async Task<IActionResult> CreateVariety([FromForm] VarietyModel variety)
        {
            var result = await _varietyService.CreateVarietyAsync(variety);
            if (result != null)
            {
                return Ok(new ApiResponse
                {
                    Message = "Success",
                    Payload = result
                });
            }

            return BadRequest(new ApiResponse
            {
                Message = "Failed to create variety."
            });
        }

        [Authorize(Roles = "Manager")]
        [HttpPut("update-variety")]
        public async Task<IActionResult> UpdateVariety([FromForm] VarietyModel variety)
        {
            var result = await _varietyService.UpdateVarietyAsync(variety);
            if (result != null)
            {
                return Ok(new ApiResponse
                {
                    Message = "Success",
                    Payload = result
                });
            }

            return BadRequest(new ApiResponse
            {
                Message = "Failed to update variety."
            });
        }

        [Authorize(Roles = "Manager")]
        [HttpDelete("delete-variety")]
        public async Task<IActionResult> DeleteVariety(int varietyId)
        {
            if (varietyId <= 0)
            {
                return BadRequest(new ApiResponse
                {
                    Message = "Invalid Variety ID."
                });
            }

            var result = await _varietyService.DeleteVarietyAsync(varietyId);
            if (result)
            {
                return Ok(new ApiResponse
                {
                    Message = "Success"
                });
            }

            return BadRequest(new ApiResponse
            {
                Message = "Failed to delete"

            });
        }
    }
}