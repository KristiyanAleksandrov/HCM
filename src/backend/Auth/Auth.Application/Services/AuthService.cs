using Auth.Application.RequestModels;
using Auth.Domain.Entities;
using Auth.Infrastructure.Repositories;

namespace Auth.Application.Services
{
    public class AuthService
    {
        private readonly IUserRepository userRepository;

        public AuthService(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        public async Task<Guid> RegisterAsync(RegisterRequestModel input, CancellationToken ct)
        {
            //TODO, CHECK, ADD ROLES, hash PASSWORD, ETC.

            var user = new User(input.Username, input.Email, input.Password, null);

            await userRepository.AddAsync(user, ct);
            await userRepository.SaveChangesAsync(ct);
            return user.Id;
        }
    }
}
