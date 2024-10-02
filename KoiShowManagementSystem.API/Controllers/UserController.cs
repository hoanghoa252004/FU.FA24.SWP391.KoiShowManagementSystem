using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userRepository;
        public UserController(IUserService userRepository)
        {
            _userRepository = userRepository;
        }

        #region API

        // 1: LOGIN:-----------------------------------------------------------------
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel dto)
        {
            ActionResult response;
            try
            {
                var result = await _userRepository.Login(dto);
                if (result != null)
                    response = Ok(new ApiResponse()
                    {
                        Message = "Login Successfully",
                        Payload = result,
                    });
                else
                    response = Unauthorized(new ApiResponse()
                    {
                        Message = "Incorrect Email or Password",
                    });
                return response;
            }
            catch (Exception ex)
            {
                // TH1: User đã bị banned.
                // Default: Other exceptions.
                response = BadRequest(new ApiResponse()
                {
                    Message = ex.Message,
                });
                return response;
            }
        }

        
        // 2: SIGN UP:---------------------------------------------------------------
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(SignUpModel dto)
        {
            IActionResult? response = null;
            try
            {
                var result = await _userRepository.SignUp(dto);
                if (result == true)
                {
                    response = Ok(new ApiResponse()
                    {
                        Message = "Sign up successfully",
                    });
                }
                return response!;
            }
            catch(Exception ex)
            {
                // TH1: Email tồn tại.
                // Default: Other exceptions.
                response = BadRequest(new ApiResponse()
                {
                    Message = ex.Message,
                });
                return response;
            }
        }

        
        // 3: PERSONAL INFORMATION:---------------------------------------------------
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            IActionResult? response = null;
            try
            {
                object result = await _userRepository.GetProfile();
                if(result != null)
                {
                    response = Ok(new ApiResponse
                    {
                        Message = "Get Profile Successfully",
                        Payload = result
                    });
                }
                else
                {
                    response = BadRequest(new ApiResponse
                    {
                        Message = "Get Profile Failed",
                        Payload = result
                    });
                }
                return response;
            }
            catch(Exception ex)
            {
                response = BadRequest(new ApiResponse()
                {
                    Message = ex.Message,
                });
                return response;
            }
        }

        
        // 4: EDIT PERSONAL INFORMATION:-----------------------------------------------
        [Authorize]
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditPersonalInfor(EditProfileModel dto)
        {
            IActionResult? response = null;
            try
            {
                var result = await _userRepository.EditProfile(dto);
                if (result == true)
                    response =  Ok(new ApiResponse()
                    {
                        Message = "Update Profile Sucessfully",
                    });
                else 
                    response =  BadRequest(new ApiResponse()
                    {
                        Message = "Update Profile Failed"
                    });
                return response;
            }
            catch(Exception ex)
            {
                response =  BadRequest(new ApiResponse()
                {
                    Message = ex.Message,
                });
                return response;
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
                var result = await _userRepository.ChangePassword(dto);
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
