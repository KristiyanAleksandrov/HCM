using Auth.Application.Errors;
using Auth.Application.Interfaces;
using Auth.Application.RequestModels;
using Auth.Application.ResponseModels;
using Auth.Domain.Entities;
using Auth.Infrastructure.Repositories;

namespace Auth.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository usersRepository;
        private readonly IRoleRepository rolesRepository;
        private readonly IPasswordHashService passwordHashService;
        private readonly IJwtTokenGenerator jwtTokenGenerator;

        public AuthService(IUserRepository userRepository,
            IRoleRepository rolesRepository,
            IPasswordHashService passwordHashService,
            IJwtTokenGenerator jwtTokenGenerator)
        {
            this.usersRepository = userRepository;
            this.rolesRepository = rolesRepository;
            this.passwordHashService = passwordHashService;
            this.jwtTokenGenerator = jwtTokenGenerator;
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

        public async Task<AuthResponse> LoginAsync(LoginRequestModel dto, CancellationToken ct)
        {
            var user = await usersRepository.GetByUserNameAsync(dto.Username, ct)
                       ?? throw new UnauthorizedException();

            if (!passwordHashService.Verify(dto.Password, user.PasswordHash))
                throw new UnauthorizedException();

            return jwtTokenGenerator.Generate(user);
        }
    }
}
