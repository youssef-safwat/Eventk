using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTOs.Authentication.SignUp
{
    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string? UserName { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?~`]).{6,30}$",
    ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character max 30 characters.")]

        public string? Password { get; set; } 

        [Required(ErrorMessage = "First Name is Required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is Required")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "BirthDate is required")]
        public DateOnly BirthDate { get; set; }
    }
}

