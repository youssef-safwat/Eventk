using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTOs.Profile
{
    public class ChangePasswordRequest
    {
        [Required]
        public string? OldPassword { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?~`]).{6,30}$",
    ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character max 30 characters.")]
        public string? Password { get; set; }

        [Compare("Password", ErrorMessage = "The password and confirmation don't match .")]
        [Required]
        public string? ConfirmPassword { get; set; }
    }
}