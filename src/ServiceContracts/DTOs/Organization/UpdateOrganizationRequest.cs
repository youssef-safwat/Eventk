using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ServiceContracts.DTOs.Organization
{
    public class UpdateOrganizationRequest
    {
        public int OrganizationId { get; set; }
        
        [Required(ErrorMessage = "Organization Name is required")]
        public string? Name { get; set; }
        
        [Required(ErrorMessage = "Description is required")]
        public string? Description { get; set; }
        
        public IFormFile? Logo { get; set; }
    }
}