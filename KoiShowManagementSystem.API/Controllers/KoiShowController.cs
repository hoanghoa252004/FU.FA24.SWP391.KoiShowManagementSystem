using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KoiShowController : ControllerBase
    {
        //
        private readonly KoiShowService _koiShowService;
        public KoiShowController(KoiShowService koiShowService)
        {
            _koiShowService = koiShowService;
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchShow(int pageIndex = 1, int pageSize = 4, string keyword = "")
        {

            var result = await _koiShowService.SearchShow(pageIndex, pageSize, keyword);
            return Ok(new ApiResponse
            {
                Message = "",
                Payload = new { result.TotalItems, result.Shows }
            });
        }
        [HttpGet("show-detail")]
        public async Task<IActionResult> ShowDetail(int showID)
        {
            var result = await _koiShowService.GetShowDetails(showID);
            if (result != null)
            {
                return Ok(new ApiResponse()
                {
                    Message = "Success",
                    Payload = result
                });
            }
            return BadRequest(new ApiResponse() { Message = "Có lỗi rồi kài" });
        }

        [HttpGet]
        [Route("koi-detail")]
        public async Task<IActionResult> KoiDetail(int koiId)
        {
            var result = await _koiShowService.GetKoiDetail(koiId);
            if (result != null)
            {
                return Ok(new ApiResponse()
                {
                    Message = "Success",
                    Payload = result
                });
            }
            return BadRequest(new ApiResponse() { Message = "Có lỗi rồi kài" });
        }

        //[HttpGet]
        //[Route("koibyshow")]
        //public async Task<IActionResult> KoiByShow(int pageIndex, int pageSize, int showID)
        //{
        //    var result = await _koiShowRepository.GetKoiByShowId(pageIndex, pageSize, showID);
        //    return Ok(new APIResponse()
        //    {
        //        Message = "Success",
        //        Payload = result
        //    });
    //    }


        //// method get the closest show return showDTO 
        //[HttpGet]
        //[Route("closest-show")]
        //public async Task<IActionResult> ClosestShow()
        //{
        //    var result = await _koiShowRepository.GetClosestShow();
        //    if (result != null)
        //    {
        //        return Ok(new APIResponse()
        //        {
        //            Message = "Success",
        //            Payload = result
        //        });
        //    }
        //    return BadRequest(new APIResponse() { Message = "Có lỗi rồi kài" });
        //}

        ////function get paging show 
        //[HttpGet]
        //[Route("paging-show/page")]

        //public async Task<IActionResult> PagingShow([FromQuery] int page)
        //{
        //    var result = await _koiShowRepository.GetPagingShow(page);
        //    return Ok(new APIResponse()
        //    {
        //        Message = "Success",
        //        Payload = result
        //    });
        //}

    }
}
