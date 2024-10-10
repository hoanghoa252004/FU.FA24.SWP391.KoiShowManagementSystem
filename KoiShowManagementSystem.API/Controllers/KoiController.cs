using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KoiController : Controller
    {
        private readonly IKoiService _koiService;
        public KoiController(IKoiService koiService)
        {
            _koiService = koiService;
        }

        [Authorize(Roles ="Member")]
        [HttpGet("get-koi-by-user")]
        public async Task<IActionResult> GetKoiByUser()
        {
          

            var result = await _koiService.GetKoiByUserId();
            return Ok(new ApiResponse
            {
                Message = "Success",
                Payload = result
            });
        }

        [HttpGet("koi-detail ")]
        public async Task<IActionResult> KoiDetail(int koiId)
        {
            if (koiId <= 0)
            {
                return BadRequest(new ApiResponse { Message = "Invalid Koi ID." });
            }

            var result = await _koiService.GetKoiDetail(koiId);
            if (result != null)
            {
                return Ok(new ApiResponse
                {
                    Message = "Success",
                    Payload = result
                });
            }

            return NotFound(new ApiResponse { Message = "Koi not found." });
        }

    }
}
