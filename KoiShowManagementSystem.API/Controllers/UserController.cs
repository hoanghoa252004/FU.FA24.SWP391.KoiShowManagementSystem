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
                UserModel result = await _userService.GetUser();
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

        // 6: CREATE USER:-----------------------------------------------------------
        [Authorize(Roles ="Manager")]
        [HttpPost("create-user")]
        public async Task<IActionResult> CreateUser([FromForm]CreateUserRequest user)
        {
            try
            {
                await _userService.CreateUser(user);
                return Ok(new ApiResponse()
                {
                    Message = "Create User Successfully ."
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

        // 7: DELETE USER:-----------------------------------------------------------
        [Authorize(Roles = "Manager,Staff")]
        [HttpDelete("delete-user")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                await _userService.DeleteUser(userId);
                return Ok(new ApiResponse()
                {
                    Message = "Delete User Successfully ."
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


        // 8: GET ALL USER:-----------------------------------------------------------
        [Authorize(Roles = "Manager")]
        [HttpGet("get-all-user")]
        public async Task<IActionResult> GetAllUser(int pageIndex, int pageSize, string? role)
        {
            try
            {
                var result = await _userService.GetAllUser(pageIndex, pageSize, role);
                return Ok(new ApiResponse()
                {
                    Message = $"Get {role}s Successfully .",
                    Payload = new
                    {
                        TotalItems = result.TotalItems,
                        Users = result.Users,
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
        #endregion
    }
}
