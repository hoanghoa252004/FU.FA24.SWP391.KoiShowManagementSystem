using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using KoiShowManagementSystem.DTOs.BusinessModels;
using KoiShowManagementSystem.API.Helper;
using System.Numerics;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        public UserController(IUserService userService, IEmailService emailService)
        {
            _userService = userService;
            _emailService = emailService;
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
                await _emailService.SendEmail(new EmailModel()
                {
                    To = dto.Email,
                    Subject = "[SIGN UP] KOI SHOW MANAGEMENT SYSTEM",
                    Content = $"Dear {dto.Name}, \n\n" +
                             $"You just have sign up the Website [KOI SHOW MANAGEMENT SYSTEM] with these information:\n\n" +
                             $"\tEmail: {dto.Email}\n" +
                             $"\tFull Name: {dto.Name}\n" +
                             $"\tPhone: {dto.Phone}\n" +
                             $"\tPassword: {dto.Password}\n" +
                             $"\tDate Of Birth: {dto.DateOfBirth}\n\n" +
                             $"Wish you have a wonderful experience with our services !"

                });
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

        //// 5: CONFIRM EMAIL:-----------------------------------------------------------
        //[HttpGet("confirm-email")]
        //public async Task<IActionResult> ConfirmMail(ConfirmMailModel dto)
        //{
        //    IActionResult? response = null;
        //    try
        //    {
        //        var user = await _emailService.ConfirmEmail(dto);
        //    }
        //    catch (Exception ex)
        //    {
        //        response = BadRequest(new ApiResponse()
        //        {
        //            Message = ex.Message,
        //        });
        //        return response;
        //    }
        //}
        #endregion
    }
}
