using Entites;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceContracts.DTOs;
using ServiceContracts.DTOs.Authentication.Login;
using ServiceContracts.DTOs.Authentication.SignUp;
using ServiceContracts.ServicesContracts;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Services;
public class AuthenticationService : IAuthentcationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailServices _emailService;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    public AuthenticationService(UserManager<ApplicationUser> userManager,
        IEmailServices emailService,
        IMemoryCache cache,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _emailService = emailService;
        _cache = cache;
        _configuration = configuration;
    }
    public async Task<ServiceResponse<Response>> Register(RegisterRequestDto registerRequest)
    {
        //Check User Exist 

        var userEmailExist = await _userManager.FindByEmailAsync(registerRequest.Email!);

        if (userEmailExist is not null)
        {
            return new ServiceResponse<Response>
            {
                Data = new Response { Status = "Failed", Message = "Email already exists!" },
                IsSuccess = false,
                StatusCode = ServiceStatus.Conflict
            };
        }

        var userUsernNameExist = await _userManager.FindByNameAsync(registerRequest.UserName!);

        if (userUsernNameExist is not null)
        {
            return new ServiceResponse<Response>
            {
                Data = new Response { Status = "Failed", Message = "Username already exists!" },
                IsSuccess = false,
                StatusCode = ServiceStatus.Conflict
            };
        }
        _cache.Set(registerRequest.Email!, registerRequest, TimeSpan.FromMinutes(15));

         SendOTP(registerRequest.Email!);

        return new ServiceResponse<Response>
        {
            Data = new Response { Status = "Success", Message = "We've sent an otp to your email" },
            IsSuccess = true,
            StatusCode = ServiceStatus.Success
        };
    }
    public async Task<ServiceResponse<Response>> ConfirmEmail(ConfirmEmailRequest verification)
    {
       var res = VerifyOTP(verification);
        if(!res.IsSuccess) return res;

        if (!_cache.TryGetValue(verification.Email!, out RegisterRequestDto? userData))
        {
            var response = new ServiceResponse<Response>
                 {
                    Data = new Response { Status = "Failed", Message = "Your Session Has Expired , Please Try Again" },
                    IsSuccess = false,
                    StatusCode = ServiceStatus.BadRequest
                };
            return response;
        }

        _cache.Remove(verification.Email!);

        var user = userData.Adapt<ApplicationUser>();
        var result = await _userManager.CreateAsync(user,userData.Password);
        if (result.Succeeded)
        {
            return new ServiceResponse<Response>
            {
                Data = new Response { Status = "Success", Message = "User registered successfully!" },
                IsSuccess = true,
                StatusCode = ServiceStatus.Success
            };
        }

        return new ServiceResponse<Response>
        {
            Data = new Response { Status = "Failed", Message = "InternalServerError" },
            IsSuccess = false,
            StatusCode = ServiceStatus.InternalServerError
        };

    }
    private ServiceResponse<Response> VerifyOTP(ConfirmEmailRequest verification)
    {
        if (!_cache.TryGetValue($"otp-{verification.Email}", out string? otp))
        {
            return new ServiceResponse<Response>
            {
                Data = new Response { Status = "Failed", Message = "Expired OTP Please Try Again." },
                IsSuccess = false,
                StatusCode = ServiceStatus.BadRequest
            };
        }

        if(otp != verification.OTP)
        {
            return new ServiceResponse<Response>
            {
                Data = new Response { Status = "Failed", Message = "Invalid OTP" },
                IsSuccess = false,
                StatusCode = ServiceStatus.BadRequest
            };
        }
        _cache.Remove($"otp-{verification.Email}");
        
        return new ServiceResponse<Response>
        {
            Data = new Response { Status = "Success", Message = "OTP Verified Successfully" },
            IsSuccess = true,
            StatusCode = ServiceStatus.Success
        };
    } 
    private async void SendOTP(string email)
    {
        var otp = new Random().Next(100000, 1000000).ToString();
        string otpMessage = 
$@"
<body style=""font-family: Arial, sans-serif; background-color: #f9f9f9; margin: 0; padding: 0;"">
    <div style=""max-width: 600px; margin: 20px auto; background-color: #ffffff; border: 1px solid #dddddd; border-radius: 5px; padding: 20px; box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);"">
        <h1 style=""color: #4CAF50; font-size: 24px; text-align: center;"">Eventk Email Verification</h1>
        <p style=""color: #333333; font-size: 16px; line-height: 1.5;"">
            Thank you for using our service. We have received a request to verify your email address.
        </p>
        <p style=""color: #333333; font-size: 16px; line-height: 1.5;"">
            Please use the following one-time verification code to complete the process:
        </p>
        <div class=""otp-code"" style=""color: #d32f2f; font-size: 20px; font-weight: bold; text-align: center; margin: 20px 0;"">
            {otp.Substring(0, 3) + "  " + otp.Substring(3)}
        </div>
        <p style=""color: #333333; font-size: 16px; line-height: 1.5;"">
            This code is valid for the next 5 minutes. If you didn't request this, you can ignore this email.
        </p>
        <p style=""color: #333333; font-size: 16px; line-height: 1.5;"">
            If you have any questions or need further assistance, feel free to contact us.
        </p>
        <p style=""font-size: 14px; color: #777777; text-align: center; margin-top: 20px;"">
            Best regards,<br>
            <strong>Eventk</strong>
        </p>
    </div>
</body>
";
         _cache.Set($"otp-{email}", otp, TimeSpan.FromMinutes(5));
         await _emailService.SendEmail(email , "Eventk Verification" , otpMessage);
    }
    public async Task<ServiceResponse<LoginResponseDto>> Login(LoginRequestDto loginRequest)
    {
        // Checking the user...
        var user = await _userManager.FindByEmailAsync(loginRequest.EmailOrUsername!)
            ?? await _userManager.FindByNameAsync(loginRequest.EmailOrUsername!);
        if (user is not null && await _userManager.CheckPasswordAsync(user, loginRequest.Password!))
        {
            // Generate the token with the claim
            var Token = GetToken(user.Email! , user.Id);
            return new ServiceResponse<LoginResponseDto>
            {
                Data = new LoginResponseDto
                {
                    Status = "Success",
                    Token = new JwtSecurityTokenHandler().WriteToken(Token),
                    Expiration = Token.ValidTo
                },
                IsSuccess = true,
                StatusCode = ServiceStatus.Success
            };
        }
        return new ServiceResponse<LoginResponseDto>
        {
            Data = null,
            IsSuccess = false,
            StatusCode = ServiceStatus.Unauthorized
        };
    }
    private List<Claim> AuthClaims(string email , string userID)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.NameIdentifier, userID),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        return claims;
    }
    private JwtSecurityToken GetToken(string email , string userId)
    {
        var authClaims = AuthClaims(email , userId);
        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));
        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            expires: DateTime.Now.AddDays(365.5),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));
        return token;
    }
    public async Task<ServiceResponse<Response>> ForgotPassword(string Email)
    {
        var user = await _userManager.FindByEmailAsync(Email);
        if(user is not null)
        {
            SendOTP(Email);
            return new ServiceResponse<Response>
            {
                Data = new Response { Status = "Success", Message = "We've sent an otp to your email" },
                IsSuccess = true,
                StatusCode = ServiceStatus.Success
            };
        }
        return new ServiceResponse<Response>
        {

            Data = new Response { Status = "Failed", Message = "Your email is not found" },
            IsSuccess = false,
            StatusCode = ServiceStatus.NotFound
        };
    }
    public async Task<ServiceResponse<Response>> ResetPassword(ResetPasswordRequest resetPasswordRequest)
    {

            if (_cache.TryGetValue<bool>($"otp-verified-{resetPasswordRequest.Email}" , out bool x) )
            {
                _cache.Remove($"otp-verified-{resetPasswordRequest.Email}");
                var user = await _userManager.FindByEmailAsync(resetPasswordRequest.Email);


            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user!);

            var changePasswordResult = await _userManager.ResetPasswordAsync(user!, resetToken, resetPasswordRequest.Password);

            if (changePasswordResult.Succeeded)
            {
                return new ServiceResponse<Response>
                {
                    Data = new Response { Status = "Success", Message = "Password changed successfully" },
                    IsSuccess = true,
                    StatusCode = ServiceStatus.Success
                };
            }

            return new ServiceResponse<Response>
            {
                Data = new Response { Status = "Failed", Message = "User Change Password Failed" },
                IsSuccess = false,
                StatusCode = ServiceStatus.InternalServerError
            };
        }

        return new ServiceResponse<Response>
        {
            Data = new Response { Status = "Failed", Message = "Your Session Has Expired , Please Verify your email Again" },
            IsSuccess = false,
            StatusCode = ServiceStatus.Forbidden
        };

    }
    public ServiceResponse<Response> VerifyEmail(ConfirmEmailRequest confirmEmailRequest)
    {
        var res = VerifyOTP(confirmEmailRequest);
        if (res.IsSuccess)
        {
            _cache.Set($"otp-verified-{confirmEmailRequest.Email}", true, TimeSpan.FromMinutes(5));
            return new ServiceResponse<Response>
            {
                Data = new Response {Status = "Success" , Message = "OTP Verified Successfully" },
                IsSuccess = true,
                StatusCode = ServiceStatus.Success
            };
        }

        return new ServiceResponse<Response>
        {
            Data = new Response { Status = "Failed", Message = "Invalid or Expired OTP" },
            IsSuccess = false,
            StatusCode = ServiceStatus.BadRequest
        };
    }
}
