using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Entities;
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


        // 3. REMOVE REFEREE FROM SHOW:
        [Authorize(Roles = "Manager")]
        [HttpDelete("remove-referee-from-show")]
        public async Task<IActionResult> RemoveRefereeFromShow(int refereeDetail)
        {
            try
            {
                bool result = await _refereeService.RemoveRefereeFromShow(refereeDetail);
                if (result == true)
                {
                    return Ok(new ApiResponse()
                    {
                        Message = "Removew Referee From Show Successfully."
                    });
                }
                else
                {
                    return BadRequest(new ApiResponse()
                    {
                        Message = "Failed To Remove Referee From Show !"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse()
                {
                    Message = ex.Message,
                });
            }
        }

        [Authorize]
        [HttpGet("get-all-referee")]
        public async Task<IActionResult> GetAllReferee()
        {
            var result = await _refereeService.GetAllReferee();
            if (result == null)
            {
                return BadRequest(new ApiResponse { Message = "Failed" });
            }
            return Ok(new ApiResponse { Message = "Success", Payload = result });
        }
    }
}
