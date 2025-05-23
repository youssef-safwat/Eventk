using Eventk.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Organization;
using ServiceContracts.ServicesContracts;
using System.Security.Claims;

namespace Eventk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _organizationService;
        public OrganizationController(IOrganizationService organizationService)
        {
            _organizationService = organizationService;
        }
        [HttpGet("get-organizations")]
        public async Task<IActionResult> GetOrganizations([FromQuery]GetOrganizationsRequest organizationsRequest)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var result =  await _organizationService.GetOrganizations(organizationsRequest , userId);
            return this.ToActionResult(result);
        } 
        [HttpGet("get-organization/{organizationId}")]
        public async Task<IActionResult> GetOrganization(int organizationId , [FromQuery]int pageNumber=1 ,[FromQuery] int pageSize=20)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var result =  await _organizationService.GetOrganization(organizationId , userId , pageNumber , pageSize);
            return this.ToActionResult(result);
        }
        
        [HttpPost("follow-organization")]
        [Authorize]
        public async Task<IActionResult> FollowOrganization(int organizationId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return BadRequest(new Response { Message = "User not found" , Status = "Failed" });
            }
            var result =  await _organizationService.FollowOrganization(organizationId , userId);
            return this.ToActionResult(result);
        }
        [HttpDelete("unfollow-organization")]
        [Authorize]
        public async Task<IActionResult> UnFollowOrganization(int organizationId)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userId is null)
            {
                return BadRequest(new Response { Message = "User not found" , Status = "Failed" });
            }
            var result =  await _organizationService.UnFollowOrganization(organizationId , userId);
            return this.ToActionResult(result);
        }
    }
}
