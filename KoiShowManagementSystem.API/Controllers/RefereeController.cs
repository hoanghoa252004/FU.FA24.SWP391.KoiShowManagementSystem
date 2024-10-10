using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
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

        [HttpGet("list-show")]
        public async Task<IActionResult> GetListShowAsync() 
        {
            var result = await _refereeService.GetListShow();
            if (result != null)
            {
                return Ok(new ApiResponse
                {
                    Message = "Success",
                    Payload = result
                });
            }
            return NotFound(new ApiResponse{ Message = "No shows found",});
        }

        [HttpGet("list-Koi")]
        public async Task<IActionResult> GetListKoiByGroupIdAsync(int groupId)
        {
            var result = await _refereeService.GetKoiDetailsByGroupId(groupId);
            if (result != null)
            {
                return Ok(new ApiResponse
                {
                    Message = "Success",
                    Payload = result
                });
            }
            return NotFound(new ApiResponse { Message = "Not found", });
        }

        [HttpPost("save-scores")]
        public async Task<IActionResult> SaveScoresAsync([FromBody] List<ScoreDTO> scores)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _refereeService.SaveScoreFromReferee(scores);
            if (result)
            {
                return Ok(new { Message = "Scores saved successfully" });
            }
            return BadRequest(new { Message = "Failed to save scores" });
        }

    }
}
