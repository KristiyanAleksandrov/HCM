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

        public async Task<Guid> RegisterAsync(RegisterRequestModel req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Username))
            {
                throw new BadRequestException("First name is required.");
            }

            if (string.IsNullOrWhiteSpace(req.Password))
            {
                throw new BadRequestException("Password is required.");
            }

            if (string.IsNullOrWhiteSpace(req.Email))
            {
                throw new BadRequestException("Email is required.");
            }

            if (await usersRepository.ExistsAsync(req.Username, ct))
            {
                throw new ConflictException("Username already exists");
            }

            var user = new User(req.Username, req.Email, passwordHashService.Hash(req.Password));
            foreach (var roleName in req.Roles.Distinct())
                user.AddRole(await rolesRepository.RequireByNameAsync(roleName, ct));

            await usersRepository.AddAsync(user, ct);
            await usersRepository.SaveChangesAsync(ct);
            return user.Id;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequestModel req, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(req.Username))
            {
                throw new BadRequestException("First name is required.");
            }

            if (string.IsNullOrWhiteSpace(req.Password))
            {
                throw new BadRequestException("Password is required.");
            }

            var user = await usersRepository.GetByUserNameAsync(req.Username, ct)
                       ?? throw new UnauthorizedException();

            if (!passwordHashService.Verify(req.Password, user.PasswordHash))
                throw new UnauthorizedException();

            return jwtTokenGenerator.Generate(user);
        }
    }
}
