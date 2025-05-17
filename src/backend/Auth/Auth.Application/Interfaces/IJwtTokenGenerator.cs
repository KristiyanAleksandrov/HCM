using Auth.Application.ResponseModels;
using Auth.Domain.Entities;

namespace Auth.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        AuthResponse Generate(User user);
    }
}
