using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GroupController : ControllerBase
    {
        private readonly IGroupService _groupService;
        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpPost("add-group")]
        public async Task<IActionResult> AddGroup([FromForm] GroupDTO group)
        {
            var result = await _groupService.AddGroupToShow(group);
            if (result)
            {
                return Ok(new ApiResponse { Message = "Success" });
            }
            return BadRequest(new ApiResponse { Message = "Failed" });
        }

        [HttpPut("update-group")]
        public async Task<IActionResult> UpdateGroup([FromForm] GroupDTO group)
        {
            var result = await _groupService.UpdateGroup(group);
            if (result)
            {
                return Ok(new ApiResponse { Message = "Success" });
            }
            return BadRequest(new ApiResponse { Message = "Failed" });
        }

        [HttpDelete("delete-group")]
        public async Task<IActionResult> DeleteGroup(int groupId)
        {
            var result = await _groupService.DeleteGroup(groupId);
            if (result)
            {
                return Ok(new ApiResponse { Message = "Success" });
            }
            return BadRequest(new ApiResponse { Message = "Failed" });
        }

        [HttpGet("get-all-groups-by-show")]
        public async Task<IActionResult> GetAllGroupByShow(int showId)
        {
            var result = await _groupService.GetAllGroupByShow(showId);
            if (result == null)
            {
                return NotFound(new ApiResponse { Message = "No groups in show", Payload = result });
            }
            return Ok(new ApiResponse { Message = "Success", Payload = result });
        }
    }
}
