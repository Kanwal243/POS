using EyeHospitalPOS.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Interfaces
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        Task<RefreshToken> GenerateRefreshTokenAsync(string userId, string jwtId);
        ClaimsPrincipal? ValidateToken(string token);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
        bool IsTokenValid(string token);
        Task<RefreshToken?> GetRefreshTokenAsync(string token);
        Task RevokeRefreshTokenAsync(string token);
        Task<bool> ValidateRefreshTokenAsync(string refreshToken, string jwtId);
    }
}
