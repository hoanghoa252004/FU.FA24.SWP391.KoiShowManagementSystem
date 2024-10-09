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
            return Ok(result);
        }

        [HttpGet("list-Koi")]
        public async Task<IActionResult> GetListKoiByGroupIdAsync(int groupId)
        {
            var result = await _refereeService.GetKoiDetailsByGroupId(groupId);
            return Ok(result);
        }

        [HttpGet("save-score")]
        public async Task<IActionResult> SaveScoreAsync(int criterionId, int koiId, int refereeDetailId, decimal scoreValue)
        {
            var result = await _refereeService.SaveScoreFromReferee(criterionId, koiId, refereeDetailId, scoreValue);
            return Ok(result);
        }
    }
}
