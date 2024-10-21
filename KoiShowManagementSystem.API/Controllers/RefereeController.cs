using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RefereeController : ControllerBase
    {
        private readonly IRefereeService _refereeService;
        public RefereeController(IRefereeService refereeService)
        {
            _refereeService = refereeService;
        }

        [Authorize]
        [HttpGet("get-all-referee-by-show")]
        public async Task<IActionResult> GetAllRefereeByShow(int showId)
        {
            var result = await _refereeService.GetAllRefereeByShow(showId);
            if (result == null)
            {
                return BadRequest(new ApiResponse { Message = "Failed" });
            }
            return Ok(new ApiResponse { Message = "Success", Payload = result });
        }

        [Authorize(Roles = "Manager")]
        [HttpPost("add-referees-to-show")]
        public async Task<IActionResult> AddRefereeToShow([FromBody] List<int> referees, int showId)
        {
            var result = await _refereeService.AddRefereeToShow(referees, showId);
            if (result)
            {
                return Ok(new ApiResponse { Message = "Success" });
            }
            return BadRequest(new ApiResponse { Message = "Failed" });
        }


    }
}
