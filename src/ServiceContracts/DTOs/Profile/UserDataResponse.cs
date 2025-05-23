
using Entites;

namespace ServiceContracts.DTOs.Profile
{
    public class UserDataResponse
    {  
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? Bio { get; set; }
        public Uri? ProfilePicture { get; set; }
    }
    public static class ApplicationUserExtension
    {
        public static UserDataResponse ToUserDataResponse(this ApplicationUser applicationUser)
        {
            return new UserDataResponse
            {
                UserName = applicationUser.UserName,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                BirthDate = applicationUser.BirthDate,
                Bio = applicationUser.Bio,
                ProfilePicture = applicationUser.ProfilePicture,
            };
        }

    }
}
