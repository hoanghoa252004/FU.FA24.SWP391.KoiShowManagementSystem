
using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IUserService _userService;
        public AuthenticationController(IUserService userService)
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
                //await _emailService.SendEmail(new EmailModel()
                //{
                //    To = dto.Email,
                //    Subject = "[SIGN UP] KOI SHOW MANAGEMENT SYSTEM",
                //    Content = $"Dear {dto.Name}, \n\n" +
                //             $"You just have sign up the Website [KOI SHOW MANAGEMENT SYSTEM] with these information:\n\n" +
                //             $"\tEmail: {dto.Email}\n" +
                //             $"\tFull Name: {dto.Name}\n" +
                //             $"\tPhone: {dto.Phone}\n" +
                //             $"\tPassword: {dto.Password}\n" +
                //             $"\tDate Of Birth: {dto.DateOfBirth}\n\n" +
                //             $"Wish you have a wonderful experience with our services !"

                //});
                return Ok(new ApiResponse()
                {
                    Message = "Sign up successfully",
                });
            }
            catch (Exception ex)
            {
                // TH1: Email tồn tại.
                // Default: Other exceptions.
                return BadRequest(new ApiResponse()
                {
                    Message = ex.Message,
                });
            }
        }
        #endregion
    }
}
