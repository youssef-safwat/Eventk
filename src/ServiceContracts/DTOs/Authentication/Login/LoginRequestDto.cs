using System.ComponentModel.DataAnnotations;
namespace ServiceContracts.DTOs.Authentication.Login
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "EmailOrUsername is required!")]

        // Username or Email 
        public string? EmailOrUsername { get; set; }

        [Required(ErrorMessage = "Password is required!")]
        public string? Password { get; set; }

    }
}
