using gestionale_scalzo.Data;
using gestionale_scalzo.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace gestionale_scalzo.Services
{
    
    public class JwtService : IJwtService
    {
        private readonly IConfiguration _cfg;
        private readonly UserManager<IdentityUser> _um;
        private readonly ApplicationDbContext _db;
        public JwtService(IConfiguration cfg, UserManager<IdentityUser> um, ApplicationDbContext db)
            => (_cfg, _um, _db) = (cfg, um, db);

        public async Task<(string access, string refresh)> GenerateTokensAsync(ApplicationUser user)
        {
            var key = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!);
            var creds = new SigningCredentials(
                new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };

            var jwt = new JwtSecurityToken(
                issuer: _cfg["Jwt:Issuer"],
                audience: _cfg["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.Parse(_cfg["Jwt:AccessTokenMinutes"]!)),
                signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);

            // ---- Refresh token ----
            var refresh = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(int.Parse(_cfg["Jwt:RefreshTokenDays"]!)),
                UserId = user.Id.ToString()
            };
            _db.RefreshTokens.Add(refresh);
            await _db.SaveChangesAsync();

            return (accessToken, refresh.Token);
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!);
            var handler = new JwtSecurityTokenHandler();
            var principal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,      // ignora scadenza
                ValidateIssuerSigningKey = true,
                ValidIssuer = _cfg["Jwt:Issuer"],
                ValidAudience = _cfg["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out _);
            return principal;
        }
    }

}
