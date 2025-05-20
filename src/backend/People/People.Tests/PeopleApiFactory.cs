using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using People.Infrastructure.Data;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace People.Tests
{
    public class PeopleApiFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("IntegrationTest");

            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IVaultService>();
                services.AddSingleton<IVaultService, MockVaultService>();

                services.RemoveAll<DbContextOptions<PeopleDbContext>>();
                services.RemoveAll(typeof(IConfigureOptions<DbContextOptions<PeopleDbContext>>));
                services.AddDbContext<PeopleDbContext>(o =>
                    o.UseInMemoryDatabase("PeopleTestDb"));

                services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                    options.DefaultChallengeScheme = TestAuthHandler.Scheme;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.Scheme, _ => { });

                services.AddAuthorization();
            });
        }
    }
    public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public const string Scheme = "TestScheme";

        public TestAuthHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder)
            : base(options, logger, encoder) { }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(new[]
            {
            new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "HRAdmin")
        }, Scheme);

            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }

    public interface IVaultService
    {
        Task<Dictionary<string, object>> GetSecretAsync(string path);
    }

    public class MockVaultService : IVaultService
    {
        public Task<Dictionary<string, object>> GetSecretAsync(string path)
        {
            return Task.FromResult(new Dictionary<string, object>
        {
            { "Secret", "testsecrettestsecrettestsecrettestsecret" },
            { "Issuer", "testissuer" },
            { "Audience", "testaudience" },
            { "ExpiryMinutes", "60" }
        });
        }
    }
}
