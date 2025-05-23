using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Authentication.Login;
using ServiceContracts.DTOs.Authentication.SignUp;

namespace ServiceContracts.ServicesContracts;

public interface IAuthentcationService 
{
    Task <ServiceResponse<LoginResponseDto>> Login(LoginRequestDto loginRequest);
    Task<ServiceResponse<Response>> Register(RegisterRequestDto registerRequest);
    Task<ServiceResponse<Response>> ConfirmEmail(ConfirmEmailRequest registerRequest);
    Task<ServiceResponse<Response>> ForgotPassword(string Email);
    ServiceResponse<Response> VerifyEmail(ConfirmEmailRequest confirmEmailRequest);
    Task<ServiceResponse<Response>> ResetPassword(ResetPasswordRequest resetPasswordRequest);
}
