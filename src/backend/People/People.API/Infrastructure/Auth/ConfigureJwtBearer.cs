using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using People.Application.Interfaces;
using System.Text;

namespace People.API.Infrastructure.Auth
{
    public class ConfigureJwtBearer
     : IConfigureNamedOptions<JwtBearerOptions>
    {
        private readonly IVaultService vault;
        public ConfigureJwtBearer(IVaultService vault) => this.vault = vault;

        public void Configure(string? name, JwtBearerOptions opts)
        {
            var secrets = vault.GetSecretAsync("hcm/jwt").GetAwaiter().GetResult();

            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = secrets["Issuer"]!.ToString(),
                ValidAudience = secrets["Audience"]!.ToString(),
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(secrets["Secret"]!.ToString())),
                ClockSkew = TimeSpan.Zero
            };
        }
        public void Configure(JwtBearerOptions options) => Configure(Options.DefaultName, options);
    }
}
