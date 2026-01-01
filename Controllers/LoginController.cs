using EyeHospitalPOS.Models;
using EyeHospitalPOS.Interfaces;
using EyeHospitalPOS.Services;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Controllers
{
    public class LoginController
    {
        private readonly IAuthService _authService;
        private readonly IJwtService _jwtService;

        public LoginController(IAuthService authService, IJwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;

        }

        public async Task<(bool Success, User? User, string Token, string RefreshToken, string ErrorMessage)> LoginAsync(string username, string password)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                return (false, null, string.Empty, string.Empty, "Please enter both username and password");
            }

            // Attempt login via AuthService to get full response including refresh token
            var loginRequest = new Models.DTOs.LoginRequestDto 
            { 
                UserName = username, 
                Password = password 
            };

            var result = await _authService.LoginAsync(loginRequest);

            if (!result.Success)
            {
                return (false, null, string.Empty, string.Empty, result.Message);
            }

            // Return success with tokens
            // Note: The User object in AuthResponseDto might be a DTO, we might need the full User entity if the rest of the app expects it.
            // However, AuthService.LoginAsync returns a UserDto, not the entity.
            // Start by getting the entity if needed, or mapping the DTO back to a User object for the frontend state.
            // Let's look at AuthService.LoginAsync implementation again. It returns AuthResponseDto with a UserDto.
            
            // To maintain compatibility with existing code that expects a 'User' model:
            var userEntity = await _authService.Login(username, password);
            
            return (true, userEntity, result.AccessToken ?? string.Empty, result.RefreshToken ?? string.Empty, string.Empty);
        }

        public string GetRedirectPath(User user)
        {
            // Role-based redirection logic
            return user.Role?.Name switch
            {
                "Administrator" => "/dashboard",
                "Manager" => "/dashboard",
                "Cashier" => "/pos",
                _ => "/dashboard"
            };
        }
    }
}
