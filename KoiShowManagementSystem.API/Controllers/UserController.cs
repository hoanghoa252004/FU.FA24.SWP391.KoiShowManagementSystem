using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using KoiShowManagementSystem.DTOs.BusinessModels;
using System.Numerics;
using KoiShowManagementSystem.Services.Helper;

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
        public async Task<IActionResult> EditPersonalInfor([FromForm]EditProfileModel dto)
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
        public async Task<IActionResult> ChangePassword([FromForm]ChangePasswordModel dto)
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

        //// 6: CONFIRM EMAIL:-----------------------------------------------------------
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

        //// TEST:-----------------------------------------------------------
        //[HttpPut("test")]
        //public async Task<IActionResult> Test([FromForm]TestModel dto)
        //{
        //    string a = await _s3UploadService.UploadShowBannerImage(dto.file!);
        //    return Ok(new ApiResponse()
        //    {
        //        Message = a
        //    });
        //}
    }
}
