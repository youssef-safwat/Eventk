using Entites;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Profile;

namespace ServiceContracts.ServicesContracts;

public interface IUsersService
{
    Task<ServiceResponse<UserDataResponse>> GetUserData(string? userEmail);
    Task<ServiceResponse<Response>> UpdateUserData(UpdateProfileRequest request, string? userEmail);
    Task<ServiceResponse<Response>> ChangeUserPassword(ChangePasswordRequest request, string? userEmail);
    Task<ServiceResponse<Response>> DeleteUserData(DeleteProfileRequest request, string? userEmail);
    Task<ServiceResponse<Response>> DeleteUserProfilePicture(string? userEmail);
}
