using Auth.Application.Errors;
using Auth.Application.Interfaces;
using Auth.Application.RequestModels;
using Auth.Domain.Entities;
using Auth.Infrastructure.Repositories;

namespace Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository usersRepository;
        private readonly IRoleRepository rolesRepository;
        private readonly IPasswordHashService passwordHashService;

        public AuthService(IUserRepository userRepository,
            IRoleRepository rolesRepository,
            IPasswordHashService passwordHashService)
        {
            this.usersRepository = userRepository;
            this.rolesRepository = rolesRepository;
            this.passwordHashService = passwordHashService;
        }

        public async Task<Guid> RegisterAsync(RegisterRequestModel input, CancellationToken ct)
        {
            if (await usersRepository.ExistsAsync(input.Username, ct))
            {
                throw new ConflictException("Username already exists");
            }

            var user = new User(input.Username, input.Email, passwordHashService.Hash(input.Password));
            foreach (var roleName in input.Roles.Distinct())
                user.AddRole(await rolesRepository.RequireByNameAsync(roleName, ct));

            await usersRepository.AddAsync(user, ct);
            await usersRepository.SaveChangesAsync(ct);
            return user.Id;
        }
    }
}
