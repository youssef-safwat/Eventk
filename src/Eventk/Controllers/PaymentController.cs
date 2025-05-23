using Eventk.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceContracts.DTOs.Payment;
using ServiceContracts.ServicesContracts;
using System.Security.Claims;

namespace Eventk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly PaymobProcessedHmacVerifier _verifier;

        public PaymentController(IPaymentService paymentService, IConfiguration config)
        {
            _paymentService = paymentService;
            _verifier = new PaymobProcessedHmacVerifier(config["Paymob:HmacSecret"]);
        }
        [Authorize]
        [HttpPost("payment/intention")]
        public async Task<IActionResult> CreatePaymentIntention([FromBody] PaymentRequest request)
        {
            string? userEmail = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            var checkoutUrl = await _paymentService.PaymentIntent(request, userEmail);
            return this.ToActionResult(checkoutUrl);
        }
        [Authorize]
        [HttpPost("payment/refund")]
        public async Task<IActionResult> CreatePaymentIntention([FromBody] RefundRequest request)
        {
            string? userId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            var checkoutUrl = await _paymentService.PaymentRefund(request, userId);
            return this.ToActionResult(checkoutUrl);
        }
        [HttpPost("payment/callback")]
        [AllowAnonymous]
        public IActionResult Callback([FromBody] object input, [FromQuery] string hmac)
        {
            if (input == null) return BadRequest("Invalid JSON");

            // If the input is already a string (JSON)
            string? json = input as string ?? input.ToString();
            if (json is null)
            {
                return BadRequest("Can't Convert JSON to String");
            }
            // Deserialize to CallbackWrapper
            CallbackWrapper? wrapper;
            try
            {
                wrapper = JsonConvert.DeserializeObject<CallbackWrapper>(json);
            }
            catch (Exception e)
            {
                return BadRequest("Invalid JSON");
            }
            var callback = _verifier.VerifyHmac(wrapper, hmac);
            if (callback is not null)
            {
                _paymentService.HandleCallBack(callback);
            }
            return BadRequest("Invalid HMAC");
        }
    }
}
