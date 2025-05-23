using Azure.Core;
using Eventk.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTOs;
using ServiceContracts.ServicesContracts;
using System.Security.Claims;

namespace Eventk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InterestController : ControllerBase
    {
        private readonly IInterestsService _interestsService;

        public InterestController(IInterestsService interestsService)
        {
            _interestsService = interestsService;
        }

        [HttpPost("add-interest")]
        public async Task<IActionResult> AddInterest(InterestAddRequest request)
        {
            string? userId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            var response = await _interestsService.AddInterest(request, userId);
            return this.ToActionResult(response);
        }
        [HttpDelete("delete-interest")]
        public async Task<IActionResult> DeleteInterest(InterestAddRequest request)
        {
            string? userId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            var response = await _interestsService.DeleteInterest(request, userId);
            return this.ToActionResult(response);
        }
        [HttpGet("get-interest")]
        public async Task<IActionResult> GetAllInterest()
        {
            string? userId = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.NameIdentifier)?.Value;
            var response = await _interestsService.GetAllInterest(userId);
            return this.ToActionResult(response);
        }
    }
}
