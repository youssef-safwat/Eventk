using Entites;
using Microsoft.AspNetCore.Identity;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Profile;
using ServiceContracts.Helpers;
using ServiceContracts.ServicesContracts;

namespace Services;
public class UsersService : IUsersService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICloudinaryService _cloudinaryService;
    public UsersService(UserManager<ApplicationUser> userManager, ICloudinaryService cloudinaryService)
    {
        _userManager = userManager;
        _cloudinaryService = cloudinaryService;
    }
    public async Task<ServiceResponse<Response>> ChangeUserPassword(ChangePasswordRequest request, string? userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
        { 
            return new ServiceResponse<Response>
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.Unauthorized,
                Data = new Response
                {
                    Status = "Faild",
                    Message = "User Credentials not provided."
                }
            };
        }
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user is null)
        {
            return new ServiceResponse<Response>()
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.NotFound,
                Data = new Response() { Status = "Faild", Message = "User is not founded." }
            };
        }
        var isChanged = await _userManager.ChangePasswordAsync(user, request.OldPassword, request.Password);
        if (isChanged.Succeeded)
        {
            return new ServiceResponse<Response>
            {
                IsSuccess = true,
                Data = new Response { Status = "Success", Message = "Password has changed successfully" },
                StatusCode = ServiceStatus.Success,
            };
        }
        return new ServiceResponse<Response>
        {
            IsSuccess = false,
            StatusCode = ServiceStatus.Forbidden,
        };
    }
    public async Task<ServiceResponse<Response>> DeleteUserData(DeleteProfileRequest request, string? userEmail)
    {

        if (string.IsNullOrEmpty(userEmail))
        {
            return new ServiceResponse<Response>
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.Unauthorized,
                Data = new Response
                {
                    Status = "Faild",
                    Message = "User Credentials not provided. "
                }
            };
        }
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user is null)
        {
            return new ServiceResponse<Response>
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.NotFound,
                Data = new Response
                {
                    Status = "Faild",
                    Message = "User Credentials not found."
                }
            };
        }
        var IsPasswordValid = await _userManager.CheckPasswordAsync(user, request.OldPassword);
        if (!IsPasswordValid)
        {
            return new ServiceResponse<Response>
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.Forbidden,
                Data = new Response
                {
                    Status = "Failed",
                    Message = "Wrong Password, Please try again."
                }
            };
        }
        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return new ServiceResponse<Response>
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.InternalServerError,
                Data = new Response
                {
                    Status = "Faild",
                    Message = "Something went wrong, Please try again later."
                }
            };
        }
        if (user.ProfilePicture is not null)
        {
            string publicId = ImageRemoveHelper.ExtractRootPublicIdFromUrl(user.ProfilePicture.ToString());
             _ = _cloudinaryService.DeleteImageAsync(publicId);
        }

        return new ServiceResponse<Response>
        {
            IsSuccess = true,
            Data = new Response { Status = "Success", Message = "User Deleted Successfully" },
            StatusCode = ServiceStatus.Success
        };

    }
    public async Task<ServiceResponse<Response>> DeleteUserProfilePicture(string? userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            return new ServiceResponse<Response>
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.Unauthorized,
                Data = new Response { Status = "Failed", Message = "Invalid or expired token" },
            };
        }
        var userData = await _userManager.FindByEmailAsync(userEmail);

        if (userData?.ProfilePicture is null)
        {
            return new ServiceResponse<Response>
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.BadRequest,
                Data = new Response { Status = "Failed", Message = "Profile picture does not exists" }
            };

        }
        string publicId = ImageRemoveHelper.ExtractRootPublicIdFromUrl(userData.ProfilePicture.ToString());
        await _cloudinaryService.DeleteImageAsync(publicId);
        userData.ProfilePicture = null;
        await _userManager.UpdateAsync(userData);
        return new ServiceResponse<Response>
        {
            IsSuccess = true,
            StatusCode = ServiceStatus.Success,
            Data = new Response { Status = "Success", Message = "Profile picture deleted successfully" }
        };
    }
    public async Task<ServiceResponse<UserDataResponse>> GetUserData(string? userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            return new ServiceResponse<UserDataResponse>()
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.Unauthorized
            };
        }
        ApplicationUser? user = await _userManager.FindByEmailAsync(userEmail);
        if (user is null)
        {
            return new ServiceResponse<UserDataResponse>()
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.NotFound
            };
        }
        return new ServiceResponse<UserDataResponse>()
        {
            IsSuccess = true,
            StatusCode = ServiceStatus.Success,
            Data = user.ToUserDataResponse()
        };
    }
    public async Task<ServiceResponse<Response>> UpdateUserData(UpdateProfileRequest request, string? userEmail)
    {
        if (string.IsNullOrEmpty(userEmail))
        {
            return new ServiceResponse<Response>
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.Unauthorized,
                Data = new Response
                {
                    Status = "Faild",
                    Message = "User Credentials not provided."
                }
            };
        }
        var userData = await _userManager.FindByEmailAsync(userEmail);
        if (userData is null)
        {
            return new ServiceResponse<Response>
            {
                IsSuccess = false,
                StatusCode = ServiceStatus.NotFound
            };
        }
        if (userData?.UserName != request.UserName
            && await _userManager.FindByNameAsync(request.UserName) is not null)
        {
            return new ServiceResponse<Response>
            {
                StatusCode = ServiceStatus.Conflict,
                Data = new Response
                {
                    Status = "Failed",
                    Message = "userName Already Exists"
                },
                IsSuccess = false
            };
        }
        userData.UserName = request.UserName;
        userData.FirstName = request.FirstName;
        userData.LastName = request.LastName;
        userData.BirthDate = request.BirthDate;
        userData.Bio = request.Bio;

        // Need to be updated
        if (request.ProfilePicture != default)
        {
            if (request.ProfilePicture.ContentType == "image/jpg" ||
            request.ProfilePicture.ContentType == "image/jpeg" ||
            request.ProfilePicture.ContentType == "image/png")
            {
                if (userData.ProfilePicture is not null)
                {
                    string publicId = ImageRemoveHelper.ExtractRootPublicIdFromUrl(userData.ProfilePicture.ToString());
                    _ = _cloudinaryService.DeleteImageAsync(publicId);
                }
                userData.ProfilePicture = await _cloudinaryService.UploadImageAsync(request.ProfilePicture);
            }
            else
            {
                return new ServiceResponse<Response>
                {
                    StatusCode = ServiceStatus.BadRequest,
                    Data = new Response
                    {
                        Status = "Failed",
                        Message = "Invalid image extension"
                    },
                    IsSuccess = false
                };
            }
        }

        var updateUser = await _userManager.UpdateAsync(userData);
        if (updateUser.Succeeded)
        {
            return new ServiceResponse<Response>
            {
                StatusCode = ServiceStatus.Success,
                Data = new Response
                {
                    Status = "Success",
                    Message = "User Updated Successfully."
                },
                IsSuccess = true
            };
        }
        return new ServiceResponse<Response>
        {
            StatusCode = ServiceStatus.InternalServerError,
            Data = new Response
            {
                Status = "Faild",
                Message = "Enable to update user date."
            },
            IsSuccess = false
        };
    }
}
