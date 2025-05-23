using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ServiceContracts.DTOs.Dashboard
{
    public class EditUserDTO
    {
        public string Id { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
        
        // For role selection in the edit form
        public string? SelectedRole { get; set; }
        
        // Available roles for dropdown
        public List<string> AvailableRoles { get; set; } = new List<string>();
    }
}