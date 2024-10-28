using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        public DashboardController(IDashboardService service)
        {
            _service = service;
        }
        #region API
        // 1: DASHBOARD REGISTRATION:-----------------------------------------------------------------
        [Authorize(Roles = "Manager")]
        [HttpGet("quantity-registrations-of-shows")]
        public async Task<IActionResult> GetRegistrationDashboard(int quantityOfRecentShow)
        {
            try
            {
                var result = await _service.GetRegistrationDashboard(quantityOfRecentShow);
                return Ok(new ApiResponse()
                {
                    Message = "Get Registration Dashboard Successfully .",
                    Payload = result,
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

        // 2: DASHBOARD REVENUE:-----------------------------------------------------------------
        [Authorize(Roles = "Manager")]
        [HttpGet("renevnue-of-shows")]
        public async Task<IActionResult> GetRevenueDashboard(int quantityOfRecentShow)
        {
            try
            {
                var result = await _service.GetRevenueDashboard(quantityOfRecentShow);
                return Ok(new ApiResponse()
                {
                    Message = "Get Registration Revenue Successfully .",
                    Payload = result,
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

        // 3: DASHBOARD VARIETY:-----------------------------------------------------------------
        [Authorize(Roles = "Manager")]
        [HttpGet("quantity-of-each-variety-in-shows")]
        public async Task<IActionResult> GetVarietyDashboard(int quantityOfRecentShow)
        {
            try
            {
                var result = await _service.GetVarietyDashboard(quantityOfRecentShow);
                return Ok(new ApiResponse()
                {
                    Message = "Get Registration Variety Successfully .",
                    Payload = result,
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
        #endregion
    }
}
