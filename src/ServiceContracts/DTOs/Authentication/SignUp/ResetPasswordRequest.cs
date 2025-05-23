using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTOs.Authentication.SignUp
{
    public class ResetPasswordRequest
    {
        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?~`]).{6,30}$",
    ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character max 30 characters.")]

        public string Password { get; set; } = null!;

        [Compare("Password", ErrorMessage = "The Password and confirmation do not match .")]

        public string ConfirmPassword { get; set; } = null!;

        public string Email { get; set; } = null!;
    }
}
