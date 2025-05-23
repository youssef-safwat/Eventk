using Entites;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ServiceContracts.DTOs.Dashboard;
using ServiceContracts.ServicesContracts;
using System.Security.Claims;

namespace Services
{
    public class DashboardAuthenticationService : IDashboardAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public DashboardAuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<bool> LoginAsync(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
                return false; // User not found

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                loginDTO.Password,
                loginDTO.RememberMe,
                lockoutOnFailure: true);

            return result.Succeeded;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDTO registerDTO, string role)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerDTO.Email);
            if (existingUser != null)
                return IdentityResult.Failed(new IdentityError { Description = "Email already in use." });
        
            // 1) Create the user
            var user = new ApplicationUser
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                FirstName = registerDTO.FirstName,
                LastName  = registerDTO.LastName,
                EmailConfirmed = true
            };
            var result = await _userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
                return result;

            // 2) Ensure the role exists
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));

            // 3) Add user to the role
            await _userManager.AddToRoleAsync(user, role);

            // 4) If OrgAdmin, create blank Organization and link
            if (role == "OrganizationAdmin")
            {
                var org = new Organization
                {
                    UserId = user.Id,
                    FollowersCount = 0
                };
                _context.Organization.Add(org);
                await _context.SaveChangesAsync();

                // 5) Add OrganizationId claim so User.Claims has it
                await _userManager.AddClaimAsync(user,
                    new Claim("OrganizationId", org.OrganizationId.ToString()));
            }

            return result;
        }


        public async Task<IEnumerable<UserDTO>> GetAllUsersAsync()
        {
            // Join users with organizations to get organization data for org admins
            var usersWithOrgs = await (from u in _context.Users
                                      join o in _context.Organization on u.Id equals o.UserId into orgJoin
                                      from org in orgJoin.DefaultIfEmpty()
                                      select new 
                                      {
                                          u.Id,
                                          u.Email,
                                          u.UserName,
                                          u.FirstName,
                                          u.LastName,
                                          OrganizationName = org != null ? org.Name : null,
                                          OrganizationId = org != null ? org.OrganizationId : (int?)null
                                      }).ToListAsync();

            // Fetch all user–role mappings
            var userRoles = await _context.UserRoles
                .Select(ur => new { ur.UserId, ur.RoleId })
                .ToListAsync();

            // Fetch all roles
            var roles = await _context.Roles
                .Select(r => new { r.Id, r.Name })
                .ToListAsync();

            // Build a lookup from roleId → roleName
            var roleLookup = roles.ToDictionary(r => r.Id, r => r.Name);

            // Compose the DTOs in memory
            var result = usersWithOrgs
                .Select(u => new UserDTO
                {
                    Id = u.Id,
                    Email = u.Email,
                    UserName = u.UserName,
                    FullName = $"{u.FirstName} {u.LastName}",
                    OrganizationName = u.OrganizationName,
                    Roles = userRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Select(ur => roleLookup[ur.RoleId])
                        .ToList()
                })
                .ToList();

            return result;
        }



        public async Task<bool> IsInRoleAsync(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return false;

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<IdentityResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task SeedRolesAsync()
        {
            if (!await _roleManager.RoleExistsAsync("SuperAdmin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            }

            if (!await _roleManager.RoleExistsAsync("OrganizationAdmin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("OrganizationAdmin"));
            }
        }

        public async Task SeedSuperAdminAsync(string firstName, string lastName, string email, string password)
        {
            // Check if super admin exists
            var superAdmin = await _userManager.FindByEmailAsync(email);

            if (superAdmin == null)
            {
                superAdmin = new ApplicationUser
                {
                    FirstName = firstName,
                    LastName = lastName,
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(superAdmin, password);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                }
            }
        }

        public async Task<EditUserDTO> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            
            // Get organization if exists
            var organization = await _context.Organization
                .FirstOrDefaultAsync(o => o.UserId == userId);
        
            // Get all available roles
            var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
        
            return new EditUserDTO
            {
                Id = user.Id,
                Roles = roles.ToList(),
                AvailableRoles = allRoles
            };
        }

        public async Task<IdentityResult> UpdateUserAsync(EditUserDTO model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });

            
            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return result;
            
            // Handle role management if a new role is selected
            if (!string.IsNullOrEmpty(model.SelectedRole) && !await _userManager.IsInRoleAsync(user, model.SelectedRole))
            {
                // Add the new role
                result = await _userManager.AddToRoleAsync(user, model.SelectedRole);
                if (!result.Succeeded)
                    return result;

                var org = new Organization
                {
                    UserId = user.Id,
                    FollowersCount = 0
                };
                _context.Organization.Add(org);
                await _context.SaveChangesAsync();
                if (model.SelectedRole == "OrganizationAdmin")
                {
                    await _userManager.AddClaimAsync(user,
                        new Claim("OrganizationId", org.OrganizationId.ToString()));
                }
            }
            
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
        
            // Check if this is a SuperAdmin user
            if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
                // Count how many SuperAdmins we have
                var superAdmins = await _userManager.GetUsersInRoleAsync("SuperAdmin");
                if (superAdmins.Count <= 1)
                {
                    return IdentityResult.Failed(new IdentityError 
                    { 
                        Description = "Cannot delete the last SuperAdmin user." 
                    });
                }
            }

            // Delete associated organization if exists
            var organization = await _context.Organization
                .FirstOrDefaultAsync(o => o.UserId == userId);
            if (organization != null)
            {
                _context.Organization.Remove(organization);
                await _context.SaveChangesAsync();
            }

            return await _userManager.DeleteAsync(user);
        }

       
    }
}