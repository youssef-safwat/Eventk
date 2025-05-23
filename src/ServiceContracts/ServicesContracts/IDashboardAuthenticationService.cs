using Microsoft.AspNetCore.Identity;
using ServiceContracts.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.ServicesContracts
{
    public interface IDashboardAuthenticationService
    {
        Task<bool> LoginAsync(LoginDTO loginDTO);
        Task LogoutAsync();
        Task<IEnumerable<UserDTO>> GetAllUsersAsync();
        Task<IdentityResult> RegisterUserAsync(RegisterDTO model, string role);
        Task<bool> IsInRoleAsync(string userId, string role);
        Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task SeedRolesAsync();
        Task SeedSuperAdminAsync(string firstName, string lastName, string email, string password);
        
        // New methods
        Task<EditUserDTO> GetUserByIdAsync(string userId);
        Task<IdentityResult> UpdateUserAsync(EditUserDTO model);
        Task<IdentityResult> DeleteUserAsync(string userId);
    }
}
