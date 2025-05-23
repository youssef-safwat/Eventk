using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTOs.Authentication.SignUp;
using ServiceContracts.DTOs.Authentication.Login;
using ServiceContracts.DTOs;
using Eventk.Helpers;
using ServiceContracts.ServicesContracts;
namespace Eventk.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{

        private readonly IAuthentcationService _authService;
        public AuthenticationController(IAuthentcationService authentcationService)
        {
            _authService = authentcationService;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequestDto register)
        {
             var response = await _authService.Register(register);
             return this.ToActionResult(response);            
        }

    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailRequest confirmEmailRequest)
    {
        var response = await _authService.ConfirmEmail(confirmEmailRequest);
        return this.ToActionResult(response);
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto loginRequest)
    {
        var response = await _authService.Login(loginRequest);
        if(response.IsSuccess == false)
        {
            return StatusCode(401, new Response { Status = "Failed", Message = "Invalid username or password , Please try again" });
        }
        return this.ToActionResult(response);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult>ForgotPassword([FromBody]string email)
    {

       var response = await _authService.ForgotPassword(email);
       return this.ToActionResult(response);
    }
    [HttpPost("verify-email")]
    public IActionResult ForgotPassword([FromBody]ConfirmEmailRequest emailRequest)
    {
       var response = _authService.VerifyEmail(emailRequest);
       return this.ToActionResult(response);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordRequest resetPasswordRequest) 
    {
        var response = await _authService.ResetPassword(resetPasswordRequest);
        return this.ToActionResult(response);
    }

}