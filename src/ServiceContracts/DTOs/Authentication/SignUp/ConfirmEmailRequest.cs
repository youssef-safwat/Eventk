using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTOs.Authentication.SignUp
{
    public class ConfirmEmailRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public string? OTP { get; set; }
    }
}
