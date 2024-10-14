using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        #region API
        // 1. GET MY REGISTRATION:-----------------------------------------------
        [Authorize]
        [HttpGet("registrations-by-member")]
        public async Task<IActionResult> GetKoiRegistrationByUser(string status)
        {
            IActionResult? response = null;
            try
            {
                var result = await _registrationService.GetMyRegistration(status);
                if (result != null)
                    response = Ok(new ApiResponse()
                    {
                        Message = "Get Koi Registration Sucessfully",
                        Payload = result
                    });
                else
                    response = BadRequest(new ApiResponse()
                    {
                        Message = "Get Koi Registration Failed"
                    });
                return response;
            }
            catch (Exception ex)
            {
                response = BadRequest(new ApiResponse()
                {
                    Message = ex.Message,
                });
                return response;
            }
        }

        // 2. CREATE REGISTRATION:-----------------------------------------------
        [Authorize]
        [HttpPost("create-registration")]
        public async Task<IActionResult> CreateRegistration([FromForm]CreateRegistrationModel dto)
        {
            try
            {
                await _registrationService.CreateRegistration(dto);
                return Ok(new ApiResponse()
                {
                    Message = "Create Registration Successfully",
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

        // 3. GET REGISTRATION BY SHOW: 
        [HttpGet("registrations-by-show")]
        public async Task<IActionResult> GetRegistrationByShow(int pageIndex, int pageSize, int showID)
        {
            if (showID <= 0)
            {
                return BadRequest(new ApiResponse { Message = "Invalid show ID." });
            }

            if (pageIndex < 1 || pageSize < 1)
            {
                return BadRequest(new ApiResponse { Message = "Page index or Page size must be greater than or equal to 1." });
            }

            var result = await _registrationService.GetRegistrationByShow(pageIndex, pageSize, showID);

            return Ok(new
            {
                Message = "Success",
                Payload = new
                {
                    TotalItems = result.TotalItems,
                    Registrations = result.Registrations
                }
            });
        }

        // 4. GET REGISTRATION BY ID:
        [HttpGet("registration-by-id")]
        public async Task<IActionResult> GetRegistration(int registrationId)
        {
            if (registrationId <= 0)
            {
                return BadRequest(new ApiResponse { Message = "Invalid Registration ID." });
            }

            var result = await _registrationService.GetRegistration(registrationId);
            if (result != null)
            {
                return Ok(new ApiResponse
                {
                    Message = "Success",
                    Payload = result
                });
            }

            return NotFound(new ApiResponse { Message = "Registration not found." });
        }

        // 5. GET PENDING REGISTRATION:
        [Authorize(Roles ="Staff,Manager")]
        [HttpGet("pending-registration")]
        public async Task<IActionResult> GetPendingRegistration(int pageIndex, int pageSize)
        {
            try
            {
                var result = await _registrationService.GetPendingRegistration(pageIndex, pageSize);
                return Ok(new ApiResponse()
                {
                    Message = "Get Pending Registration Successfully",
                    Payload = new
                    {
                        TotalItems = result.TotalItems,
                        Registrations = result.Registrations
                    }
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

        // 6. UPDATE PENDING REGISTRATION:
        [Authorize(Roles = "Staff,Member")]
        [HttpPut("update-registration")]
        public async Task<IActionResult> UpdateRegistration([FromForm] UpdateRegistrationModel dto)
        {
            try
            {
                await _registrationService.UpdateRegistration(dto);
                return Ok(new ApiResponse()
                {
                    Message = "Update Registration Successfully",
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
