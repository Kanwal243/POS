using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components;
using System.Security.Claims;
using EyeHospitalPOS.Helper;
using EyeHospitalPOS.Interfaces;

namespace EyeHospitalPOS.Services
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly LoginManager _loginManager;

        public CustomAuthenticationStateProvider(LoginManager loginManager)
        {
            _loginManager = loginManager;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Initialize the LoginManager to load user from session
            await _loginManager.InitializeAsync();

            if (_loginManager.CurrentUser != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, _loginManager.CurrentUser.UserName),
                    new Claim(ClaimTypes.Email, _loginManager.CurrentUser.Email ?? ""),
                    new Claim(ClaimTypes.Role, _loginManager.CurrentUser.Role?.Name ?? "User"),
                    new Claim("UserId", _loginManager.CurrentUser.Id),
                    new Claim("IsSessionBased", "true")
                };

                var identity = new ClaimsIdentity(claims, "SessionAuth");
                var user = new ClaimsPrincipal(identity);

                return new AuthenticationState(user);
            }

            return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        public async Task MarkUserAsAuthenticatedAsync(string userId, string userName, string email, string roleName)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim(ClaimTypes.Email, email ?? ""),
                new Claim(ClaimTypes.Role, roleName ?? "User"),
                new Claim("UserId", userId),
                new Claim("IsSessionBased", "true")
            };

            var identity = new ClaimsIdentity(claims, "SessionAuth");
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }

        public void MarkUserAsLoggedOut()
        {
            var identity = new ClaimsIdentity();
            var user = new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
        }
    }
}