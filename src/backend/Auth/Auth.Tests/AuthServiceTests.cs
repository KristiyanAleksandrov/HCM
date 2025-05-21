using Auth.Application.Errors;
using Auth.Application.Interfaces;
using Auth.Application.RequestModels;
using Auth.Application.ResponseModels;
using Auth.Application.Services;
using Auth.Domain.Entities;
using Auth.Infrastructure.Repositories;
using Moq;
using Xunit;

namespace Auth.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> usersRepoMock;
        private readonly Mock<IRoleRepository> rolesRepoMock;
        private readonly Mock<IPasswordHashService> passwordHashMock;
        private readonly Mock<IJwtTokenGenerator> tokenGenMock;
        private readonly AuthService authService;
        private readonly CancellationToken ct = CancellationToken.None;

        public AuthServiceTests()
        {
            usersRepoMock = new Mock<IUserRepository>(MockBehavior.Strict);
            rolesRepoMock = new Mock<IRoleRepository>(MockBehavior.Strict);
            passwordHashMock = new Mock<IPasswordHashService>(MockBehavior.Strict);
            tokenGenMock = new Mock<IJwtTokenGenerator>(MockBehavior.Strict);
            authService = new AuthService(usersRepoMock.Object, rolesRepoMock.Object, passwordHashMock.Object, tokenGenMock.Object);
        }

        [Theory]
        [InlineData("", "test", "kris@gmail")]
        [InlineData("kris", "", "kris@gmail")]
        [InlineData("kris", "test", "")]
        public async Task RegisterAsync_ShouldThrowBadRequest_WhenFieldMissing(string username, string password, string email)
        {
            var req = new RegisterRequestModel
            {
                Username = username,
                Password = password,
                Email = email,
                Roles = Array.Empty<string>()
            };

            await Assert.ThrowsAsync<BadRequestException>(
                () => authService.RegisterAsync(req, ct));
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowConflict_WhenUsernameExists()
        {
            var req = new RegisterRequestModel
            {
                Username = "kris",
                Password = "test",
                Email = "kris@gmail",
                Roles = Array.Empty<string>()
            };

            usersRepoMock.Setup(r => r.ExistsAsync(req.Username, ct))
                        .ReturnsAsync(true);

            await Assert.ThrowsAsync<ConflictException>(
                () => authService.RegisterAsync(req, ct));
        }

        [Fact]
        public async Task RegisterAsync_ShouldReturnId_WhenValid()
        {
            var req = new RegisterRequestModel
            {
                Username = "alice",
                Password = "p@ss",
                Email = "a@acme.com",
                Roles = new[] { "HRAdmin" }
            };

            usersRepoMock.Setup(r => r.ExistsAsync(req.Username, ct))
                        .ReturnsAsync(false);

            passwordHashMock.Setup(h => h.Hash(req.Password)).Returns("hashed-pw");

            rolesRepoMock.Setup(r => r.RequireByNameAsync(
                                     It.IsAny<string>(), ct))
                        .ReturnsAsync((string name, CancellationToken _) =>
                                      new Role() { Name = name});

            usersRepoMock.Setup(r => r.AddAsync(It.IsAny<User>(), ct))
                        .Returns(Task.CompletedTask);
            usersRepoMock.Setup(r => r.SaveChangesAsync(ct))
                        .Returns(Task.CompletedTask);

            var id = await authService.RegisterAsync(req, ct);

            usersRepoMock.Verify(r => r.AddAsync(
                It.Is<User>(u => u.Username == req.Username
                              && u.Email == req.Email
                              && u.Roles.Select(role => role.Name)
                                        .SequenceEqual(req.Roles.Distinct())),
                ct), Times.Once);

            usersRepoMock.Verify(r => r.SaveChangesAsync(ct), Times.Once);
            Assert.NotEqual(Guid.Empty, id);
        }

        [Theory]
        [InlineData("", "test")]
        [InlineData("kris", "")]
        public async Task LoginAsync_ShouldThrowBadRequest_WhenFieldMissing(string user, string password)
        {
            var req = new LoginRequestModel { Username = user, Password = password };

            await Assert.ThrowsAsync<BadRequestException>(
                () => authService.LoginAsync(req, ct));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorized_WhenUserNotFound()
        {
            var req = new LoginRequestModel { Username = "kris", Password = "test" };

            usersRepoMock.Setup(r => r.GetByUserNameAsync(req.Username, ct))
                        .ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => authService.LoginAsync(req, ct));
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorized_WhenBadPassword()
        {
            var user = new User("kris", "kris@gmail.com", "hashed-pw");

            usersRepoMock.Setup(r => r.GetByUserNameAsync(user.Username, ct))
                        .ReturnsAsync(user);
            passwordHashMock.Setup(h => h.Verify("wrong-pw", user.PasswordHash))
                    .Returns(false);

            var req = new LoginRequestModel { Username = "kris", Password = "wrong-pw" };

            await Assert.ThrowsAsync<UnauthorizedException>(
                () => authService.LoginAsync(req, ct));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenValid()
        {
            var user = new User("kris", "kris@gmail.com", "hashed-pw");
            var token = new AuthResponse { Token = "jwtToken" };

            usersRepoMock.Setup(r => r.GetByUserNameAsync(user.Username, ct))
                        .ReturnsAsync(user);
            passwordHashMock.Setup(h => h.Verify("test", user.PasswordHash))
                    .Returns(true);
            tokenGenMock.Setup(t => t.Generate(user)).Returns(token);

            var req = new LoginRequestModel { Username = "kris", Password = "test" };
            var response = await authService.LoginAsync(req, ct);

            tokenGenMock.Verify(t => t.Generate(user), Times.Once);
            Assert.Equal(token, response);
        }
    }
}
