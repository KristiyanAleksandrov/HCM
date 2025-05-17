using Auth.Application.RequestModels;

namespace Auth.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Guid> RegisterAsync(RegisterRequestModel request, CancellationToken ct);
    }
}
