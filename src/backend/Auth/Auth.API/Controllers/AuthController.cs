using Auth.Application.Interfaces;
using Auth.Application.RequestModels;
using Auth.Application.ResponseModels;
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

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequestModel dto, CancellationToken ct)
    {
        var response = await authService.LoginAsync(dto, ct);

        return Ok(response);
    }
}
