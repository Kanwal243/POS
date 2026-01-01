using EyeHospitalPOS.Interfaces;
using EyeHospitalPOS.Services;
using EyeHospitalPOS.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Controllers
{
    public class UserController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userService.GetAllUsersAsync();
        }

        public async Task<User?> GetUserByIdAsync(string id)
        {
            return await _userService.GetUserByIdAsync(id);
        }

        public async Task<User> CreateUserAsync(User user, string password)
        {
            return await _userService.CreateUserAsync(user, password);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            return await _userService.UpdateUserAsync(user);
        }
    }
}
