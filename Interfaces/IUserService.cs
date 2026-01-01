using EyeHospitalPOS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(string id);
        Task<User> CreateUserAsync(User user, string password);
        Task<User> UpdateUserAsync(User user);
        Task ActivateUserAsync(string userId);
        Task DeactivateUserAsync(string userId);
        Task AssignRoleAsync(string userId, string roleId);
    }
}
