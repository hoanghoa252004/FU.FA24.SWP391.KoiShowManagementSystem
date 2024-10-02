//using KoiShowManagementSystem.Payload;
//using KoiShowManagementSystem.Repositories.OldVersion;
//using KoiShowManagementSystem.Services;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace KoiShowManagementSystem.API.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class KoiShowController : ControllerBase
//    {
//        //
//        private readonly IKoiShowRepository _koiShowRepository;
//        public KoiShowController(IKoiShowRepository koiShowRepository)
//        {
//            _koiShowRepository = koiShowRepository;
//        }

//        [HttpGet]
//        [Route("search")]
//        public async Task<IActionResult> SearchShow(int pageIndex = 1, int pageSize = 4, string keyword = "")
//        {
//            var result = await _koiShowRepository.SearchShow(pageIndex, pageSize, keyword);

//            return Ok(new APIResponse()
//            {
//                Message = "",
//                Payload = new { result.TotalItems, data = result.Shows }
//            });
//        }

//        [HttpGet]
//        [Route("show-detail")]
//        public async Task<IActionResult> ShowDetail(int showID)
//        {
//            var result = await _koiShowRepository.GetShowDetails(showID);
//            if (result != null)
//            {
//                return Ok(new APIResponse()
//                {
//                    Message = "Success",
//                    Payload = result
//                });
//            }
//            return BadRequest(new APIResponse() { Message = "Có lỗi rồi kài" });
//        }

//        [HttpGet]
//        [Route("koi-detail")]
//        public async Task<IActionResult> KoiDetail(int koiId)
//        {
//            var result = await _koiShowRepository.GetKoiDetail(koiId);
//            if (result != null)
//            {
//                return Ok(new APIResponse()
//                {
//                    Message = "Success",
//                    Payload = result
//                });
//            }
//            return BadRequest(new APIResponse() { Message = "Có lỗi rồi kài" });
//        }

//        [HttpGet]
//        [Route("koibyshow")]
//        public async Task<IActionResult> KoiByShow(int pageIndex, int pageSize, int showID)
//        {
//            var result = await _koiShowRepository.GetKoiByShowId(pageIndex, pageSize, showID);
//            return Ok(new APIResponse()
//            {
//                Message = "Success",
//                Payload = result
//            });
//        }


//        // method get the closest show return showDTO 
//        [HttpGet]
//        [Route("closest-show")]
//        public async Task<IActionResult> ClosestShow()
//        {
//            var result = await _koiShowRepository.GetClosestShow();
//            if (result != null)
//            {
//                return Ok(new APIResponse()
//                {
//                    Message = "Success",
//                    Payload = result
//                });
//            }
//            return BadRequest(new APIResponse() { Message = "Có lỗi rồi kài" });
//        }

//        //function get paging show 
//        [HttpGet]
//        [Route("paging-show/page")]

//        public async Task<IActionResult> PagingShow([FromQuery] int page)
//        {
//            var result = await _koiShowRepository.GetPagingShow(page);
//            return Ok(new APIResponse()
//            {
//                Message = "Success",
//                Payload = result
//            });
//        }

//    }
//}
