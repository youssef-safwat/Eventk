using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
namespace ServiceContracts.DTOs.Profile
{

    public class UpdateProfileRequest
    {
        [Required(ErrorMessage = "First Name is required")]
        public string? FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Username is required")]
        public string? UserName { get; set; }

        public string? Bio { get; set; }
        public DateOnly? BirthDate { get; set; }
        public IFormFile? ProfilePicture { get; set; }
    }

}