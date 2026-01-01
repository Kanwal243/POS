using EyeHospitalPOS.Models;
using EyeHospitalPOS.Models.DTOs;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Interfaces
{
    public interface IAuthService
    {
        Task<User?> Login(string username, string password);
        Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
        Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
        Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request);
        Task<bool> RevokeTokenAsync(string userId);
        Task<bool> ValidateUserAsync(string username, string password);
        Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto request);
        bool VerifyPassword(string password, string hash);
        string HashPassword(string password);
        Task UpdateLastLoginAsync(string userId);
    }
}
