using KoiShowManagementSystem.DTOs.Request;
using KoiShowManagementSystem.Services;
using Microsoft.AspNetCore.Http;
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
    }
}
