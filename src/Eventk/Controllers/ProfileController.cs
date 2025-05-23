using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ServiceContracts.DTOs.Profile;
using Eventk.Helpers;
using Microsoft.AspNetCore.Authorization;
using ServiceContracts.ServicesContracts;
namespace Eventk.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IUsersService _usersService;

        public ProfileController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpGet("me")]
        
        public async Task<IActionResult> Me()
        {
            string? userEmail = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;
            var userDate = await _usersService.GetUserData(userEmail);
            return this.ToActionResult(userDate);
        }
        [HttpPut("edit-profile")]
        public async Task<IActionResult> EditProfile([FromForm]UpdateProfileRequest request)
        {
            string? userEmail = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;
            var updatedUser = await _usersService.UpdateUserData(request, userEmail);
            return this.ToActionResult(updatedUser);
        }
        [HttpDelete("delete-profile-picture")]
        public async Task<IActionResult> DeleteProfilePicture()
        {
            string? userEmail = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;
            var deleteProfilePicture = await _usersService.DeleteUserProfilePicture(userEmail);
            return this.ToActionResult(deleteProfilePicture);
        }
        [HttpDelete("delete-profile")]
        public async Task<IActionResult> DeleteProfile(DeleteProfileRequest request)
        {
            string? userEmail = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;
            var deletedUser = await _usersService.DeleteUserData(request, userEmail);
            return this.ToActionResult(deletedUser);
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            string? userEmail = User.Claims.FirstOrDefault(p => p.Type == ClaimTypes.Email)?.Value;
            var userChangePassword = await _usersService.ChangeUserPassword(request, userEmail);
            return this.ToActionResult(userChangePassword);
        }
    }
}
