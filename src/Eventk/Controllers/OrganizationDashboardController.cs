using Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTOs.Organization;
using ServiceContracts.ServicesContracts;
using System.Security.Claims;

namespace Eventk.Controllers
{
    [Authorize(Roles = "OrganizationAdmin")]
    public class OrganizationDashboardController : Controller
    {
        private readonly IOrganizationService _organizationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrganizationDashboardController(
            IOrganizationService organizationService,
            UserManager<ApplicationUser> userManager)
        {
            _organizationService = organizationService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var result = await _organizationService.GetOrganizationByUserId(user.Id);
            if (!result.IsSuccess || result.Data == null)
                return NotFound();

            var model = new UpdateOrganizationRequest
            {
                OrganizationId = result.Data.OrganizationId,
                Name = result.Data.Name,
                Description = result.Data.Description
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateOrganizationRequest request)
        {
            if (!ModelState.IsValid)
                return View(request);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            // Verify the organization belongs to this user
            var orgResult = await _organizationService.GetOrganizationByUserId(user.Id);
            if (!orgResult.IsSuccess || orgResult.Data == null || orgResult.Data.OrganizationId != request.OrganizationId)
                return Forbid();

            var result = await _organizationService.UpdateOrganization(request);
            if (!result.IsSuccess)
            {
                ModelState.AddModelError("", result.Data?.Message ?? "Failed to update organization");
                return View(request);
            }

            return RedirectToAction("Index", "Dashboard");
        }
    }
}