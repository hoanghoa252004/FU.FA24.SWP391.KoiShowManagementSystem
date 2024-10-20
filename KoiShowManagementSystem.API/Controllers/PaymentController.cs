using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.DTOs.Response;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace KoiShowManagementSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService)
        {
           _paymentService = paymentService;
        }

        [HttpPost("payment")]
        public async Task<IActionResult> PaymentBySePayAsync([FromBody] PaymentWebhookDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _paymentService.ProcessPaymentWebhookAsync(dto);
            if (result)
            {
                return Ok(new { Message = "sucess" });
            }
            return BadRequest(new { Message = "Failed to payment" });
        }

        [HttpGet("IsAllMemberRegistrationsPaid")]
        public async Task<IActionResult> IsAllMemberRegistrationsPaid()
        {
            bool areAllPaid = await _paymentService.AreAllMemberRegistrationsPaidAsync();
            //return Ok(areAllPaid);
            if (areAllPaid == false)
            {
                return BadRequest(new ApiResponse { Message = "Not Payment" });
            }
            return Ok(new ApiResponse { Message = "Success", Payload = areAllPaid });
        }
    }
}
