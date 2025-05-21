using Auth.Application.RequestModels;
using Auth.Application.ResponseModels;
using Auth.Domain.Entities;
using Auth.Infrastructure.Data;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;

namespace Auth.Tests
{
    public class AuthApiTests : IClassFixture<AuthApiFactory>
    {
        private readonly HttpClient client;

        public AuthApiTests(AuthApiFactory factory)
        {
            client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
            db.Database.EnsureDeleted();
            db.Roles.AddRange(new Role() { Name = "Manager" }, new Role() { Name = "Employee" }, new Role() { Name = "HRAdmin" });
            db.SaveChanges();
        }

        private static StringContent ToJson(object obj) =>
        new(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");

        [Fact]
        public async Task Register_ReturnsOk_WithValidData()
        {
            var req = new RegisterRequestModel
            {
                Username = "kris",
                Password = "test",
                Email = "kris@gmail.com",
                Roles = new[] { "Manager" }
            };

            var response = await client.PostAsync("/auth/register", ToJson(req));

            response.EnsureSuccessStatusCode();
            var body = await response.Content.ReadAsStringAsync();
            var id = JsonConvert.DeserializeObject<Guid>(body);
            Assert.NotEqual(Guid.Empty, id);
        }

        [Fact]
        public async Task Register_ReturnsBadRequest_WhenMissingFields()
        {
            var req = new RegisterRequestModel
            {
                Username = "",
                Password = "",
                Email = "",
                Roles = Array.Empty<string>()
            };

            var response = await client.PostAsync("/auth/register", ToJson(req));
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_ReturnsNotFound_WhenRoleDontExist()
        {
            var req = new RegisterRequestModel
            {
                Username = "kris",
                Password = "password",
                Email = "kris@gmail.com",
                Roles = new[] { "RandomRole" }
            };

            var response = await client.PostAsync("/auth/register", ToJson(req));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Login_ReturnsJwt_WhenValidCredentials()
        {
            var req = new RegisterRequestModel
            {
                Username = "kris",
                Password = "password",
                Email = "kris@gmail.com",
                Roles = new[] { "Employee" }
            };

            var registerResp = await client.PostAsync("/auth/register", ToJson(req));
            registerResp.EnsureSuccessStatusCode();

            var loginReq = new LoginRequestModel
            {
                Username = "kris",
                Password = "password"
            };

            var loginResp = await client.PostAsync("/auth/login", ToJson(loginReq));
            loginResp.EnsureSuccessStatusCode();

            var body = await loginResp.Content.ReadAsStringAsync();
            var auth = JsonConvert.DeserializeObject<AuthResponse>(body);

            Assert.NotNull(auth);
            Assert.False(string.IsNullOrWhiteSpace(auth.Token));
            Assert.True(auth.ExpiresAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenBadCredentials()
        {
            var loginReq = new LoginRequestModel
            {
                Username = "kris",
                Password = "password"
            };

            var response = await client.PostAsync("/auth/login", ToJson(loginReq));
            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}
