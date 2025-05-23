using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceContracts.DTOs.Authentication.Login;

public class LoginResponseDto
{
    public string? Status { get; set; }
    public string? Token { get; set; }
    public DateTime? Expiration { get; set; }
}
