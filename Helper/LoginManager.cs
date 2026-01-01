using EyeHospitalPOS.Models;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.DependencyInjection;

namespace EyeHospitalPOS.Helper
{
    /// <summary>
    /// Manages user authentication state using HTTP Session.
    /// 
    /// This implementation uses HTTP Session instead of JavaScript sessionStorage to fix
    /// the Blazor Server prerendering issue where users were logged out on page refresh.
    /// HTTP Session is available during server-side prerendering, ensuring authentication
    /// state persists across page refreshes.
    /// </summary>
    public class LoginManager
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private User? _currentUser;
        private string? _authToken;
        private string? _refreshToken;
        private bool _initialized = false;

        public LoginManager(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public User? CurrentUser
        {
            get => _currentUser;
            set => _currentUser = value;
        }

        public string? AuthToken
        {
            get => _authToken;
            set => _authToken = value;
        }

        public string? RefreshToken
        {
            get => _refreshToken;
            set => _refreshToken = value;
        }

        public bool IsLoggedIn => _currentUser != null;
        public string GetUserName() => _currentUser?.UserName ?? "Guest";
        public string GetRoleName() => _currentUser?.Role?.Name ?? "Unknown";

        /**   
        /// Load user from HTTP Session on app start.
        /// 
        /// How it fixes the prerendering issue:
        /// In Blazor Server, a page refresh triggers a "prerendering" phase on the server.
        /// During this phase, JavaScript (and thus sessionStorage) is not available.
        /// Previously, the LoginManager would fail to find the user during this phase and
        /// report them as logged out, causing the application to redirect to the login page.
        /// By using HTTP Session, the server can now identify the user immediately during
        /// the refresh, preventing the unwanted logout.
        /// 
        /// The user will now remain logged in across refreshes and tab remains until they
        /// explicitly log out or the session expires./
        **/
        public async Task InitializeAsync()
        {
            if (_initialized) return;

            // Load from HTTP Session (works during prerendering & refresh)
            // This is critical for Blazor Server - HTTP Session is available during
            // prerendering when JavaScript/sessionStorage is not.
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                ISession? session = null;
                try { session = httpContext?.Session; } catch (InvalidOperationException) { }

                if (session != null && httpContext != null)
                {
                    try
                    {
                        // Check if response has started - if so, we cannot access session
                        if (httpContext.Response.HasStarted)
                        {
                            // Response has started, cannot access session
                            // Mark as initialized to prevent repeated attempts
                            _initialized = true;
                            return;
                        }

                        // Ensure session is loaded (this is safe before response starts)
                        await session.LoadAsync();

                        // Check if session is available after loading
                        if (session.IsAvailable)
                        {
                            var userJson = session.GetString("currentUser");
                            if (!string.IsNullOrEmpty(userJson))
                            {
                                _currentUser = JsonSerializer.Deserialize<User>(userJson, new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true,
                                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                                });
                            }
                            _authToken = session.GetString("authToken");
                            _refreshToken = session.GetString("refreshToken");
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // Session cannot be accessed (response has started or session unavailable)
                        // Silently fail and mark as initialized to prevent repeated attempts
                    }
                    catch (Exception ex)
                    {
                        // Log other exceptions for debugging
                        Console.WriteLine($"Error initializing LoginManager: {ex.Message}");
                        // Silently fail and mark as initialized to prevent repeated attempts
                    }
                }

                _initialized = true;
            }
            catch
            {
            }
        }
            


        /// Save user to HTTP Session after login.
        /// 
        /// Saves authentication state to HTTP Session, which is available during server-side
        /// prerendering. This ensures the user remains authenticated across page refreshes
        /// in Blazor Server.
       
        public async Task SaveUserAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            ISession? session = null;
            try { session = httpContext?.Session; } catch (InvalidOperationException) {}
            
            if (session != null && httpContext != null)
            {
                try
                {
                    // Check if response has started - if so, we cannot modify session
                    if (httpContext.Response.HasStarted)
                    {
                        // Response has started, cannot modify session
                        return;
                    }

                    // Ensure session is loaded before writing
                    await session.LoadAsync();

                    // Check if session is available
                    if (!session.IsAvailable)
                    {
                        return;
                    }

                    string? userJson = null;
                    
                    if (_currentUser != null)
                    {
                        userJson = JsonSerializer.Serialize(_currentUser, new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            ReferenceHandler = ReferenceHandler.IgnoreCycles
                        });
                        session.SetString("currentUser", userJson);
                    }

                    if (!string.IsNullOrEmpty(_authToken))
                    {
                        session.SetString("authToken", _authToken);
                    }

                    if (!string.IsNullOrEmpty(_refreshToken))
                    {
                        session.SetString("refreshToken", _refreshToken);
                    }

                    // Commit session changes
                    await session.CommitAsync();
                }
                catch (InvalidOperationException)
                {
                    // Session cannot be accessed (response has started or session unavailable)
                    // Silently fail - session write is not critical if response has started
                }
            }
        }

        /// <summary>
        /// Clear HTTP Session on logout.
        /// 
        /// Clears all authentication data from HTTP Session to ensure complete logout.
        /// </summary>
        public async Task LogoutAsync()
        {
            _currentUser = null;
            _authToken = null;
            _refreshToken = null;

            // Clear HTTP Session
            var httpContext = _httpContextAccessor.HttpContext;
            ISession? session = null;
            try { session = httpContext?.Session; } catch (InvalidOperationException) {}
            
            if (session != null && httpContext != null)
            {
                try
                {
                    // Check if response has started - if so, we cannot modify session
                    if (!httpContext.Response.HasStarted)
                    {
                        // Ensure session is loaded before clearing
                        await session.LoadAsync();
                        
                        if (session.IsAvailable)
                        {
                            session.Clear();
                            await session.CommitAsync();
                        }
                    }
                }
                catch (InvalidOperationException)
                {
                    // Session cannot be accessed (response has started or session unavailable)
                    // Silently fail - session clear is not critical if response has started
                }
            }
        }

        /// <summary>
        /// Attempt to refresh the JWT token if possible
        /// </summary>
        public async Task<bool> TryRefreshTokenAsync(IServiceProvider serviceProvider)
        {
            var refreshToken = _refreshToken;
            var currentToken = _authToken;
            
            if (string.IsNullOrEmpty(refreshToken) || string.IsNullOrEmpty(currentToken))
            {
                return false;
            }
            
            try
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var authService = scope.ServiceProvider.GetRequiredService<Interfaces.IAuthService>();
                    
                    var refreshRequest = new Models.DTOs.RefreshTokenRequestDto
                    {
                        AccessToken = currentToken,
                        RefreshToken = refreshToken
                    };

                    var result = await authService.RefreshTokenAsync(refreshRequest);

                    if (result.Success)
                    {
                        // Update stored tokens
                        _authToken = result.AccessToken;
                        _refreshToken = result.RefreshToken;
                        await SaveUserAsync();
                        
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Token refresh failed: {ex.Message}");
            }
            
            return false;
        }
    }
}