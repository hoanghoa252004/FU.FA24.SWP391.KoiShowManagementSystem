using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using KoiShowManagementSystem.DTOs.BusinessModels;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        #region API

        // 1: LOGIN:-----------------------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel dto)
        {
            try
            {
                var result = await _userService.Login(dto);
                return Ok(new ApiResponse()
                    {
                        Message = "Login Successfully",
                        Payload = result,
                    });
            }
            catch (Exception ex)
            {
                // TH1: User đã bị banned.
                // TH2: Email / password sai.
                // Default: Other exceptions.
                return BadRequest(new ApiResponse()
                {
                    Message = ex.Message,
                });
            }
        }

        
        // 2: SIGN UP:---------------------------------------------------------------
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpModel dto)
        {
            try
            {
                await _userService.SignUp(dto);
                return Ok(new ApiResponse()
                {
                    Message = "Sign up successfully",
                });
            }
            catch(Exception ex)
            {
                // TH1: Email tồn tại.
                // Default: Other exceptions.
                return BadRequest(new ApiResponse()
                {
                    Message = ex.Message,
                });
            }
        }

        
        // 3: PERSONAL INFORMATION:---------------------------------------------------
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                ProfileModel result = await _userService.GetProfile();
                return Ok(new ApiResponse
                {
                    Message = "Get Profile Successfully",
                    Payload = result
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new ApiResponse()
                {
                    Message = ex.Message,
                });
            }
        }

        
        // 4: EDIT PERSONAL INFORMATION:-----------------------------------------------
        [Authorize]
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditPersonalInfor(EditProfileModel dto)
        {
            try
            {
                ProfileModel result = await _userService.EditProfile(dto);
                return Ok(new ApiResponse()
                {
                    Message = "Update Profile Sucessfully",
                    Payload = result
                });
            }
            catch(Exception ex)
            {
                return BadRequest(new ApiResponse()
                {
                    Message = ex.Message,
                });
            }
        }

        
        // 5: CHANGE PASSWORD:-----------------------------------------------------------
        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordModel dto)
        {
            IActionResult? response = null;
            try
            {
                bool result = await _userService.ChangePassword(dto);
                if (result == true)
                    response =  Ok(new ApiResponse()
                    {
                        Message = "Update Password Sucessfully",
                    });
                else
                    response =  BadRequest(new ApiResponse()
                    {
                        Message = "Your current password is incorrect"
                    });
                return response;
            }
            catch(Exception ex) 
            {
                response =  BadRequest(new ApiResponse()
                {
                    Message= ex.Message,
                });
                return response;
            }
        }

        /*
        // 6: GET KOI REGISTRATIONS:-------------------------------------------------------
        [Authorize]
        [HttpGet("koi-registration{status}")]
        public async Task<IActionResult> GetKoiRegistration(string status)
        {
            IActionResult response = null!;
            try
            {
                var result = await _userRepository.GetInProcessKoiRegistration(status);
                if (result != null)
                    response =  Ok(new APIResponse()
                    {
                        Message = "Get Pending Koi Registration Successfully",
                        Payload = result
                    });
                return response;
            }
            catch (Exception ex)
            {
                response = BadRequest(new APIResponse()
                {
                    Message = ex.Message,
                });
                return response;
            }
        }
        */
        #endregion
    }
}
