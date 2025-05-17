using Auth.Application.Interfaces;
using Auth.Application.RequestModels;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService authService;

    public AuthController(IAuthService authService)
    {
        this.authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestModel dto, CancellationToken ct)
    {
        var id = await authService.RegisterAsync(dto, ct);

        return Ok(id);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Login(LoginRequestModel dto, CancellationToken ct)
    {
        return Ok();
    }
}
