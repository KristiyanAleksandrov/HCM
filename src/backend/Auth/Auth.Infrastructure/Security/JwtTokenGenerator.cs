using Auth.Application.Interfaces;
using Auth.Application.ResponseModels;
using Auth.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Auth.Infrastructure.Security
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        private readonly JwtSettings cfg;
        private readonly SigningCredentials creds;

        public JwtTokenGenerator(IOptions<JwtSettings> opt)
        {
            cfg = opt.Value;
            creds = new SigningCredentials(
                         new SymmetricSecurityKey(
                             System.Text.Encoding.UTF8.GetBytes(cfg.Secret)),
                         SecurityAlgorithms.HmacSha256);
        }

        public AuthResponse Generate(User user)
        {
            var claims = new List<Claim>{
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username)
        };
            claims.AddRange(user.Roles.Select(ur =>
                new Claim(ClaimTypes.Role, ur.Name)));

            var jwt = new JwtSecurityToken(
                issuer: cfg.Issuer,
                audience: cfg.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(cfg.ExpiryMinutes),
                signingCredentials: creds);

            return new AuthResponse { Token = new JwtSecurityTokenHandler().WriteToken(jwt), ExpiresAt = jwt.ValidTo };
        }
    }
}
