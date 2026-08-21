using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MonyLoop.Application.ServicesAbstractions.UserAuth;
using MonyLoop.Domain.Entities.UserAuth;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Services.UserAuth
{
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateAccessToken(ApplicationUser user, IList<string> roles)
        {
            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("uid", user.Id.ToString()),
                new("fullName", $"{user.FirstName} {user.LastName}")
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var secretKey = _configuration["JwtOptions:SecretKey"]!;
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiryMinutes = int.Parse(_configuration["JwtOptions:AccessTokenExpiryMinutes"] ?? "15");

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtOptions:Issuer"],
                audience: _configuration["JwtOptions:Audience"],
                expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
                claims: claims,
                signingCredentials: cred
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var secretKey = _configuration["JwtOptions:SecretKey"]!;

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidAudience = _configuration["JwtOptions:Audience"],
                ValidateIssuer = true,
                ValidIssuer = _configuration["JwtOptions:Issuer"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                ValidateLifetime = false // مهم: عشان نقدر نقرا الـ Token حتى لو منتهي
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }
    }
}
