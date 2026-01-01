using EyeHospitalPOS.Data;
using EyeHospitalPOS.Models;
using EyeHospitalPOS.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EyeHospitalPOS.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // All endpoints require authentication
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all roles (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetAllRoles()
        {
            var roles = await _context.Roles
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsAdministrative = r.IsAdministrative
                })
                .ToListAsync();

            return Ok(roles);
        }

        /// <summary>
        /// Get role by ID (Admin only)
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> GetRoleById(string id)
        {
            var role = await _context.Roles
                .Where(r => r.Id == id)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    IsAdministrative = r.IsAdministrative
                })
                .FirstOrDefaultAsync();

            if (role == null)
            {
                return NotFound(new { message = "Role not found" });
            }

            return Ok(role);
        }

        /// <summary>
        /// Create a new role (Admin only)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if role name already exists
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == request.Name);

            if (existingRole != null)
            {
                return BadRequest(new { message = "Role name already exists" });
            }

            var role = new Role
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Name,
                Description = request.Description,
                IsAdministrative = request.IsAdministrative,
                CanEditModel = request.CanEditModel
            };

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsAdministrative = role.IsAdministrative
            };

            return CreatedAtAction(nameof(GetRoleById), new { id = role.Id }, roleDto);
        }

        /// <summary>
        /// Update an existing role (Admin only)
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] CreateRoleDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound(new { message = "Role not found" });
            }

            // Check if new name conflicts with another role
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == request.Name && r.Id != id);

            if (existingRole != null)
            {
                return BadRequest(new { message = "Role name already exists" });
            }

            role.Name = request.Name;
            role.Description = request.Description;
            role.IsAdministrative = request.IsAdministrative;
            role.CanEditModel = request.CanEditModel;

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsAdministrative = role.IsAdministrative
            };

            return Ok(roleDto);
        }

        /// <summary>
        /// Delete a role (Admin only)
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
            {
                return NotFound(new { message = "Role not found" });
            }

            // Check if role is assigned to any users
            var hasUsers = await _context.Users.AnyAsync(u => u.RoleId == id);

            if (hasUsers)
            {
                return BadRequest(new { message = "Cannot delete role that is assigned to users" });
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Role deleted successfully" });
        }

        /// <summary>
        /// Assign role to user (Admin only)
        /// </summary>
        [HttpPost("assign")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _context.Users.FindAsync(request.UserId);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }

            var role = await _context.Roles.FindAsync(request.RoleId);
            if (role == null)
            {
                return NotFound(new { message = "Role not found" });
            }

            user.RoleId = request.RoleId;
            user.ModifiedDate = DateTime.UtcNow;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Role assigned successfully",
                userId = user.Id,
                userName = user.UserName,
                roleId = role.Id,
                roleName = role.Name
            });
        }
    }
}
