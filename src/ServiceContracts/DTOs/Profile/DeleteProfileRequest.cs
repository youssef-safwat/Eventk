using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTOs.Profile
{
    public class DeleteProfileRequest
    {
        [Required]
        public string? OldPassword { get; set; }
    }
}
