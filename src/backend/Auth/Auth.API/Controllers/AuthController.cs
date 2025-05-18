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
    public async Task<IActionResult> Register(RegisterRequestModel req, CancellationToken ct)
    {
        var id = await authService.RegisterAsync(req, ct);

        return Ok(id);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequestModel req, CancellationToken ct)
    {
        var response = await authService.LoginAsync(req, ct);

        return Ok(response);
    }
}
