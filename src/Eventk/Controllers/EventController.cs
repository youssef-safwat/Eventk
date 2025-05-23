using Eventk.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTOs.Event;
using ServiceContracts.ServicesContracts;
using System.Security.Claims;

namespace Eventk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        public EventsController(IEventService eventService) => _eventService = eventService;

        [HttpGet("get-events")]
        public async Task<IActionResult> GetSummaries([FromQuery] EventFilters filters)
        {
            // Extract user email from JWT
            string? userId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
           //     ?? throw new UnauthorizedAccessException("Email claim missing");

            var serviceResponse = await _eventService.GetEvents(filters, userId);

            return this.ToActionResult(serviceResponse);
        }
        [HttpGet("get-event/{eventId}")]
        public async Task<IActionResult> GetEvent(int eventId)
        {
            // Extract user email from JWT
            string? userId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
           //     ?? throw new UnauthorizedAccessException("Email claim missing");

            var serviceResponse = await _eventService.GetEvent(eventId , userId);

            return this.ToActionResult(serviceResponse);
        } 
        
        [HttpGet("get-event/{eventId}/ticket-types")]
        public async Task<IActionResult> GetTicketTypes(int eventId)
        {
            var serviceResponse = await _eventService.GetTicketTypes(eventId);
            return this.ToActionResult(serviceResponse);
        }
    }
}
