﻿using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace KoiShowManagementSystem.API.Controllers
{
    public class KoiController : Controller
    {
        private readonly IKoiService _koiService;
        public KoiController(IKoiService koiService)
        {
            _koiService = koiService;
        }

        [HttpGet("get-koi-by-user")]
        public async Task<IActionResult> GetKoiByUser(int userId)
        {
            if (userId <= 0)
            {
                return BadRequest(new ApiResponse { Message = "Invalid User ID." });
            }

            var result = await _koiService.GetKoiByUserId(userId);
            return Ok(new ApiResponse
            {
                Message = "Success",
                Payload = result
            });
        }

        [HttpGet("koi-detail")]
        public async Task<IActionResult> KoiDetail(int koiId)
        {
            if (koiId <= 0)
            {
                return BadRequest(new ApiResponse { Message = "Invalid Koi ID." });
            }

            var result = await _koiService.GetKoiDetail(koiId);
            if (result != null)
            {
                return Ok(new ApiResponse
                {
                    Message = "Success",
                    Payload = result
                });
            }

            return NotFound(new ApiResponse { Message = "Koi not found." });
        }
    }
}