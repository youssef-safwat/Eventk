using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.ServicesContracts;
using ServiceContracts.DTOs.Dashboard;
using System.Threading.Tasks;

namespace Eventk.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class UsersController : Controller
    {
        private readonly IDashboardAuthenticationService _authService;

        public UsersController(IDashboardAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _authService.GetAllUsersAsync();
            return View(users);
        }

        // GET: Users/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var user = await _authService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserDTO
            {
                Id = user.Id,
                Roles = user.Roles,
                AvailableRoles = user.AvailableRoles,
                SelectedRole = user.SelectedRole,
               
            };

            return View(model);
        }

        // POST: Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserDTO model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _authService.UpdateUserAsync(model);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return NotFound();
            }

            var result = await _authService.DeleteUserAsync(id);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Failed to delete user. " + string.Join(", ", result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Index));
        }
    }
}