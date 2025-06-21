using gestionale_scalzo.Model;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace gestionale_scalzo.Services
{
    public interface IJwtService
    {
        Task<(string access, string refresh)> GenerateTokensAsync(ApplicationUser user);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
