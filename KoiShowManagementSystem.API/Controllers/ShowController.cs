﻿using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShowController : ControllerBase
    {
        private readonly IShowService _koiShowService;

        public ShowController(IShowService koiShowService)
        {
            _koiShowService = koiShowService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchShow(int pageIndex = 1, int pageSize = 4, string keyword = "")
        {
            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest(new ApiResponse { Message = "Page index or Page size must be greater than or equal to 1." });
            }

            var result = await _koiShowService.SearchShow(pageIndex, pageSize, keyword);
            return Ok(new ApiResponse
            {
                Message = "Success",
                Payload = new { result.TotalItems, result.Shows }
            });
        }

        [HttpGet("show-detail")]
        public async Task<IActionResult> ShowDetail(int showID)
        {
            if (showID <= 0)
            {
                return BadRequest(new ApiResponse { Message = "Invalid show ID." });
            }

            var result = await _koiShowService.GetShowDetails(showID);
            if (result != null)
            {
                return Ok(new ApiResponse
                {
                    Message = "Success",
                    Payload = result
                });
            }

            return NotFound(new ApiResponse { Message = "Show not found." });
        }

        // implemt api getclosestshow

        [HttpGet("closest-show")]
        public async Task<IActionResult> ClosestShow()
        {
            //catch the exception from the service
            try
            {
                var result = await _koiShowService.GetClosestShow();
                return Ok(new ApiResponse()
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


        // implement create a show
        [Authorize(Roles = "Manager")]
        [HttpPost("create-show")]
        public async Task<IActionResult> CreateShow([FromForm] ShowDTO show)
        {

            var result = await _koiShowService.CreateAShow(show);
            if (result != 0)
            {
                return Ok(new ApiResponse { Message = "Success" , Payload = result}); 
            }

            return BadRequest(new ApiResponse { Message = "Failed to create show." });
        }

        // implement get all varieties
        [HttpGet("get-all-varieties")]
        public async Task<IActionResult> GetAllVarieties()
        {
            var result = await _koiShowService.GetAllVarieties();
            return Ok(new ApiResponse { Message = "Success", Payload = result });
        }

    }
}
