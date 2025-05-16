using Auth.Application.RequestModels;

namespace Auth.Application.Services
{
    public interface IAuthService
    {
        Task<Guid> RegisterAsync(RegisterRequestModel request, CancellationToken ct);
    }
}
