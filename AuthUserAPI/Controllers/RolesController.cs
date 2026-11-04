using AuthUserAPI.Models;
using AuthUserAPI.Models.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthUserAPI.Controllers
{
    [Route("roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AuthuserContext _context;

        public RolesController(AuthuserContext context)
        {
            _context = context;
        }

        [HttpPost("newRole")]
        public async Task<IActionResult> NewRole([FromBody] NewRoleDtos newRoleDto)
        {
            if (string.IsNullOrWhiteSpace(newRoleDto.Name))
            {
                return BadRequest(new { message = "A szerepkör neve nem lehet üres." });
            }

            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = newRoleDto.Name,
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync();

            return StatusCode(201, new { message = "Felvéve.", result = role });
        }

        [HttpGet("allRoles")]
        public async Task<IActionResult> AllRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return Ok(new { message = "Lekérdezve.", result = roles });
        }

        [HttpGet("byId")]
        public async Task<IActionResult> RoleById([FromQuery] Guid id)
        {
            var role = await _context.Roles.SingleOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                return NotFound(new { message = "A szerepkör nem található." });
            }

            return Ok(new { message = "Lekérdezve.", result = role });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteRole([FromQuery] Guid id)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);

            if (role == null)
            {
                return NotFound(new { message = "A törölni kívánt szerepkör nem található." });
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Törölve.", result = role });
        }
    }
}
