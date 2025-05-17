using Auth.Application.RequestModels;
using Auth.Application.ResponseModels;

namespace Auth.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Guid> RegisterAsync(RegisterRequestModel request, CancellationToken ct);

        Task<AuthResponse> LoginAsync(LoginRequestModel dto, CancellationToken ct);
    }
}
