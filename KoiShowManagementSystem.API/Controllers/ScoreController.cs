using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ScoreController : ControllerBase
    {
        private readonly IRefereeService _refereeService;
        public ScoreController(IRefereeService refereeService)
        {
            _refereeService = refereeService;
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
