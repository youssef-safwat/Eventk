using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Entites
{
    public class ApplicationUser : IdentityUser
    {
        [Required(ErrorMessage = "First Name is required")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last Name is required")]
        public string? LastName { get; set; }
        public ICollection<Order> Orders {  get; set; } = new List<Order>();
        public ICollection<Follow> Following { get; set; } = new List<Follow>();
        public DateOnly? BirthDate { get; set; }
        public string? Bio { get; set; }
        [Url]
        public Uri? ProfilePicture { get; set; }
        public ICollection<Interests> Interests { get; set; } = new List<Interests>();
        public Organization? Organization { get; set; }  
    }
}
