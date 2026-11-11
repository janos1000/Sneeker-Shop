using Microsoft.AspNetCore.Http;
using AuthUserAPI.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using AuthUserAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace AuthUserAPI.Controllers
{
    [Route("userrole")]
    [ApiController]
    public class UserRoleController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public UserRoleController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRole([FromBody] NewUserRoleDTOs newUserRoleDto)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Roles)
                    .SingleOrDefaultAsync(u => u.Id == newUserRoleDto.UserId);

                var role = await _context.Roles.FindAsync(newUserRoleDto.RoleId);

                if (user is null || role is null)
                {
                    return NotFound(new { message = "A user vagy a role nem található." });
                }

                bool isCreated = user.Roles.Any(r => r.Id == newUserRoleDto.RoleId);

                if (isCreated)
                {
                    return Conflict(new { message = "A user már rendelkezik ezzel a role-lal." });
                }


                user.Roles.Add(role);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { message = "Sikeres hozzárendelés.", result = user });
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpDelete("removeRole")]

        public async Task<IActionResult> RemoveRole([FromBody] NewUserRoleDTOs newUserRoleDto)
        {
            try
            {
                var user = await _context.Users.Include(u => u.Roles)
                    .SingleOrDefaultAsync(u => u.Id == newUserRoleDto.UserId);
                var role = await _context.Roles.FindAsync(newUserRoleDto.RoleId);


                if (user is null || role is null)
                {
                    return NotFound(new { message = "A user vagy a role nem található." });
                }
                bool isAssigned = user.Roles.Any(r => r.Id == newUserRoleDto.RoleId);

                if (!isAssigned)
                {
                    return Conflict(new { message = "A user nem rendelkezik ezzel a role-lal." });
                }


                user.Roles.Remove(role);
                await _context.SaveChangesAsync();

                return StatusCode(200, new { message = "Sikeres eltávolítás.", result = user });
            }
            catch (Exception exception)
            {
                return BadRequest(exception.Message);
            }
        }

        [HttpGet("allUsersAllRoles")]
        public async Task<IActionResult> AllUsersAllRoles()
        {
            var users = await _context.Users
                .Include(u => u.Roles)
                .Select(u => new
                {
                    Users = u,
                    UserRoles = u.Roles

                }).ToListAsync();
            return Ok(users);

        }

        [HttpGet("userByIdAndRoles")]
        public async Task<IActionResult> UserRolesById([FromQuery] Guid userId)
        {
            
        }
    }
}