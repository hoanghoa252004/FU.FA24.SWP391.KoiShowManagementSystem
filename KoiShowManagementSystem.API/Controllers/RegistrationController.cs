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
        // 1. GET KOI REGISTRATION:-----------------------------------------------
        [Authorize]
        [HttpGet("get-registration")]
        public async Task<IActionResult> GetKoiRegistrationByUser(string status)
        {
            IActionResult? response = null;
            try
            {
                var result = await _registrationService.GetMyKoiRegistration(status);
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

        [HttpGet("get-registration-form")]
        public async Task<IActionResult> GetRegistrationForm(int showId)
        {
            try
            {
                var result = await _registrationService.GetRegistrationForm(showId);
                return Ok(new ApiResponse()
                {
                    Message = "Get Registration Form Successfully",
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

        [HttpPost("create-registration")]
        public async Task<IActionResult> CreateRegistration([FromForm]RegistrationFormModel dto)
        {
            try
            {
                await _registrationService.CreateRegistration(dto);
                return Ok(new ApiResponse()
                {
                    Message = "Get Registration Form Successfully",
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
