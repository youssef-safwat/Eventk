using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceContracts.DTOs;

namespace Eventk.Helpers;

public static class ControllerResponseExtensions
{
    public static IActionResult ToActionResult<TResponse>(
        this ControllerBase controller,
        ServiceResponse<TResponse> response)
    {
        return response.StatusCode switch
        {
            ServiceStatus.Success => controller.Ok(response.Data),
            ServiceStatus.Conflict => controller.Conflict(response.Data),
            ServiceStatus.Unauthorized => controller.StatusCode(401, response.Data),
            ServiceStatus.NotFound => controller.NotFound(response.Data),
            ServiceStatus.BadRequest => controller.BadRequest(response.Data),
            ServiceStatus.Forbidden => controller.StatusCode(403, response.Data),
            _ => controller.StatusCode(500, response.Data)
        };
    }
}