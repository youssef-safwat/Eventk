
using Eventk.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.ServicesContracts;
using System.Security.Claims;

namespace Eventk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingHistoryController : ControllerBase
    {
        private readonly IBookingHistoryService _bookingHistoryService;
        public BookingHistoryController(IBookingHistoryService bookingHistoryService)
        {
            _bookingHistoryService = bookingHistoryService;
        }
        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var result = await _bookingHistoryService.GetAllOrders(userId!);
            return this.ToActionResult(result);
        } 
        [HttpGet("order/{orderId:int:min(0)}")]
        public async Task<IActionResult> GetOrder(int orderId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var result = await _bookingHistoryService.GetOrder(orderId,userId!);
           return this.ToActionResult(result);
        }
    }
}
