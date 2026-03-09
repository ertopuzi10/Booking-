using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Booking.Application.Common.Interfaces;

namespace Booking.Infrastructure.Contracts.AuthService
{
    public class AuthManager : IAuthManager
    {
        private readonly string _jwtSecret;
        private readonly string _jwtIssuer;
        private readonly string _jwtAudience;
        private readonly int _jwtExpiryMinutes;

        public AuthManager(string jwtSecret, string jwtIssuer, string jwtAudience, int jwtExpiryMinutes = 60)
        {
            _jwtSecret = jwtSecret;
            _jwtIssuer = jwtIssuer;
            _jwtAudience = jwtAudience;
            _jwtExpiryMinutes = jwtExpiryMinutes;
        }

        public bool VerifyPassword(string password, string hash)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var computedHash = sha.ComputeHash(bytes);
            var computedHashString = Convert.ToHexString(computedHash);
            return computedHashString == hash;
        }

        public string GenerateJwtToken(int userId, string email, string firstName, string lastName, IEnumerable<string> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecret);

            var claims = new System.Collections.Generic.List<Claim>
            {
                new Claim("id", userId.ToString()),
                new Claim("email", email),
                new Claim("firstName", firstName),
                new Claim("lastName", lastName)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtExpiryMinutes),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
