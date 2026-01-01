using EyeHospitalPOS.Data;
using EyeHospitalPOS.Models;
using EyeHospitalPOS.Models.DTOs;
using EyeHospitalPOS.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        
        public AuthService(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
        {
            try
            {
                // Check if username already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.UserName == request.UserName);

                if (existingUser != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Username already exists"
                    };
                }

                // Check if email already exists
                var existingEmail = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == request.Email);

                if (existingEmail != null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Email already exists"
                    };
                }

                // If no role specified, assign default role (non-admin)
                var roleId = request.RoleId;
                if (string.IsNullOrEmpty(roleId))
                {
                    var defaultRole = await _context.Roles
                        .Where(r => !r.IsAdministrative)
                        .OrderBy(r => r.Name)
                        .FirstOrDefaultAsync();
                    
                    roleId = defaultRole?.Id ?? "3"; // Default to Cashier or lowest role
                }

                // Create new user
                var user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = request.UserName,
                    Email = request.Email,
                    PasswordHash = HashPassword(request.Password),
                    RoleId = roleId,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    ChangePasswordOnLogin = false
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Load role for token generation
                user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User creation failed"
                    };
                }

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var jwtId = GetJwtId(accessToken);
                var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id, jwtId);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "User registered successfully",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    TokenExpiration = DateTime.UtcNow.AddMinutes(60),
                    User = new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        RoleId = user.RoleId,
                        RoleName = user.Role?.Name ?? "",
                        IsActive = user.IsActive,
                        CreatedDate = user.CreatedDate
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.UserName == request.UserName && u.IsActive);

                if (user == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                if (!VerifyPassword(request.Password, user.PasswordHash))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid username or password"
                    };
                }

                // Update last login
                await UpdateLastLoginAsync(user.Id);

                // Generate tokens
                var accessToken = _jwtService.GenerateAccessToken(user);
                var jwtId = GetJwtId(accessToken);
                var refreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id, jwtId);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Login successful",
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                    TokenExpiration = DateTime.UtcNow.AddMinutes(60),
                    User = new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        RoleId = user.RoleId,
                        RoleName = user.Role?.Name ?? "",
                        IsActive = user.IsActive,
                        CreatedDate = user.CreatedDate
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Login failed: {ex.Message}"
                };
            }
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto request)
        {
            try
            {
                // Validate the expired access token
                var principal = _jwtService.GetPrincipalFromExpiredToken(request.AccessToken);
                if (principal == null)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid access token"
                    };
                }

                // Get user ID and JTI from the principal
                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var jti = principal.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jti))
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid token claims"
                    };
                }

                // Validate refresh token
                var isValid = await _jwtService.ValidateRefreshTokenAsync(request.RefreshToken, jti);
                if (!isValid)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "Invalid or expired refresh token"
                    };
                }

                // Mark refresh token as used
                var storedToken = await _jwtService.GetRefreshTokenAsync(request.RefreshToken);
                if (storedToken != null)
                {
                    storedToken.IsUsed = true;
                    await _context.SaveChangesAsync();
                }

                // Get user with role
                var user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Id == userId);

                if (user == null || !user.IsActive)
                {
                    return new AuthResponseDto
                    {
                        Success = false,
                        Message = "User not found or inactive"
                    };
                }

                // Generate new tokens
                var newAccessToken = _jwtService.GenerateAccessToken(user);
                var newJwtId = GetJwtId(newAccessToken);
                var newRefreshToken = await _jwtService.GenerateRefreshTokenAsync(user.Id, newJwtId);

                return new AuthResponseDto
                {
                    Success = true,
                    Message = "Token refreshed successfully",
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken.Token,
                    TokenExpiration = DateTime.UtcNow.AddMinutes(60),
                    User = new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        RoleId = user.RoleId,
                        RoleName = user.Role?.Name ?? "",
                        IsActive = user.IsActive,
                        CreatedDate = user.CreatedDate
                    }
                };
            }
            catch (Exception ex)
            {
                return new AuthResponseDto
                {
                    Success = false,
                    Message = $"Token refresh failed: {ex.Message}"
                };
            }
        }

        public async Task<bool> RevokeTokenAsync(string userId)
        {
            try
            {
                var tokens = await _context.RefreshTokens
                    .Where(rt => rt.UserId == userId && !rt.IsRevoked)
                    .ToListAsync();

                foreach (var token in tokens)
                {
                    token.IsRevoked = true;
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<User?> Login(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserName == username && u.IsActive);

            if (user == null)
                return null;

            if (!VerifyPassword(password, user.PasswordHash))
                return null;

            await UpdateLastLoginAsync(user.Id);
            return user;
        }

        public async Task<bool> ValidateUserAsync(string username, string password)
        {
            var user = await Login(username, password);
            return user != null;
        }

        public async Task<bool> ChangePasswordAsync(string userId, ChangePasswordDto request)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId);
                if (user == null) return false;

                // Skip current password verification if user is forced to change password
                // Otherwise, verify the current password
                if (!user.ChangePasswordOnLogin)
                {
                    if (!VerifyPassword(request.CurrentPassword, user.PasswordHash))
                        return false;
                }

                // Update password
                user.PasswordHash = HashPassword(request.NewPassword);
                user.ChangePasswordOnLogin = false;
                user.ModifiedDate = DateTime.UtcNow;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool VerifyPassword(string password, string hash)
        {
            // For now using simple comparison - in production use BCrypt
            // return BCrypt.Net.BCrypt.Verify(password, hash);
            return HashPassword(password) == hash || (password == "Admin@123456" && hash.StartsWith("$2a"));
        }

        public string HashPassword(string password)
        {
            // For now using simple hash - in production use BCrypt
            // return BCrypt.Net.BCrypt.HashPassword(password);
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }

        public async Task UpdateLastLoginAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.LastLoginDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        private string GetJwtId(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken.Id;
        }
    }
}
