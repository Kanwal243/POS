using EyeHospitalPOS.Interfaces;
using EyeHospitalPOS.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Services
{
    /// <summary>
    /// HttpClient-based API client for Authentication operations
    /// </summary>
    public class ApiAuthService : ApiClientService, IAuthService
    {
        public ApiAuthService(HttpClient httpClient, IJSRuntime jsRuntime, IHttpContextAccessor httpContextAccessor, Helper.LoginManager loginManager) 
            : base(httpClient, jsRuntime, httpContextAccessor, loginManager)
        {
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            return await PostAsync<RegisterRequestDto, AuthResponseDto>("api/Auth/register", request) 
                ?? new AuthResponseDto { Success = false, Message = "Registration failed" };
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            return await PostAsync<LoginRequestDto, AuthResponseDto>("api/Auth/login", request) 
                ?? new AuthResponseDto { Success = false, Message = "Login failed" };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            _httpClient.DefaultRequestHeaders.Remove("Authorization");
            return await PostAsync<RefreshTokenRequestDto, AuthResponseDto>("api/Auth/refresh-token", request) 
                ?? new AuthResponseDto { Success = false, Message = "Token refresh failed" };
        }

        public async Task<bool> RevokeTokenAsync(string userId)
        {
            await PostAsync<object, object>("api/Auth/revoke-token", new { });
            return true;
        }

        // These methods are not used via API, but required by interface
        // They can throw NotSupportedException or be implemented if needed
        public Task<Models.User?> Login(string username, string password)
        {
            throw new System.NotSupportedException("Use LoginAsync instead");
        }

        public Task<bool> ValidateUserAsync(string username, string password)
        {
            throw new System.NotSupportedException("Use LoginAsync instead");
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto request)
        {
            return await PostAsync<ChangePasswordDto, object>("api/Auth/change-password", request) != null;
        }

        public bool VerifyPassword(string password, string hash)
        {
            throw new System.NotSupportedException("Password verification is server-side only");
        }

        public string HashPassword(string password)
        {
            throw new System.NotSupportedException("Password hashing is server-side only");
        }

        public Task UpdateLastLoginAsync(string userId)
        {
            // This is handled automatically by the API
            return Task.CompletedTask;
        }
    }
}

