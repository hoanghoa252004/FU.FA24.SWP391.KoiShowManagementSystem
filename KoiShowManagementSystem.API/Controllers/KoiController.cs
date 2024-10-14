using KoiShowManagementSystem.DTOs.Request;
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

        [Authorize(Roles = "Member")]
        [HttpGet("koi-detail")]
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

        [Authorize(Roles = "Member")]
        [HttpPost("create-koi")]
        public async Task<IActionResult> CreateKoi([FromForm] KoiDTO koi)

        {
            var result = await _koiService.CreateKoi(koi);
            try
            {
                Console.WriteLine(koi.VarietyId);
                Console.WriteLine(koi.Image);
                if (result)
                {
                    return Ok(new ApiResponse { Message = "Koi created successfully." });
                }

                return BadRequest(new ApiResponse { Message = "Failed to create Koi." });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse { Message = ex.Message, });
            }
            
        }

        [Authorize(Roles = "Member")]
        [HttpPut("update-koi")]
        public async Task<IActionResult> UpdateKoi([FromForm] KoiDTO koi)
        {
            var result = await _koiService.UpdateKoi(koi);
            if (result)
            {
                return Ok(new ApiResponse { Message = "Koi updated successfully." });
            }

            return BadRequest(new ApiResponse { Message = "Failed to update Koi." });
        }

        //[Authorize(Roles = "Member")]
        [HttpDelete("delete-koi")]
        public async Task<IActionResult> DeleteKoi(int koiId)
        {
            if (koiId <= 0)
            {
                return BadRequest(new ApiResponse { Message = "Invalid Koi ID." });
            }

            var result = await _koiService.DeleteKoi(koiId);
            if (result)
            {
                return Ok(new ApiResponse { Message = "Koi deleted successfully." });
            }

            return BadRequest(new ApiResponse { Message = "Failed to delete Koi." });
        }
    }
}
