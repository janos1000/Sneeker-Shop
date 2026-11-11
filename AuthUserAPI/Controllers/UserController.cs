using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AuthUserAPI.Models;
using AuthUserAPI.Models.DTOs;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using AuthUserAPI.Services;

namespace AuthUserAPI.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AuthuserContext _context;

        private readonly PasswordHasher _password;


        public UserController(AuthuserContext context, PasswordHasher password)
        {
            _context = context;
            _password = password;
        }

        [HttpPost("newUser")]
        public async Task<IActionResult> NewUser([FromBody] NewUserDTOs newUserDto)
        {
            var (pass, salt) = _password.HashPassword(newUserDto.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Name = newUserDto.Name,
                Email = newUserDto.Email,
                Password = pass,
                Salt = salt,
            };

            if (user is not null)
            {
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return StatusCode(201, new { message = "Felvéve.", result = user });
            }

            return StatusCode(400, new { message = "Hiba.", results = user });
        }

        [HttpGet("allUsers")]
        public async Task<IActionResult> AllUsers()
        {
            var users = await _context.Users.ToListAsync();

            if (users is not null)
            {
                return StatusCode(200, new { message = "Lekérdezve.", result = users });
            }

            return StatusCode(400, new { message = "Hiba.", result = users });
        }

        [HttpGet("byId")]
        public async Task<IActionResult> UserById([FromQuery] Guid id)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);

            if (user is not null)
            {
                return StatusCode(200, new { message = "Lekérdezve.", result = user });
            }

            return StatusCode(400, new { message = "Hiba.", result = user });
        }

        [HttpGet("update")]
        public async Task<IActionResult> UpdateUser([FromQuery] Guid id, UpdateUserDTOs updatedUserDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is not null)
            {
                user.Name = updatedUserDto.Name;
                user.Email = updatedUserDto.Email;
                user.Password = updatedUserDto.Password;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return StatusCode(200, new { message = "Frissítve.", result = user });
            }
            return StatusCode(400, new { message = "Hiba.", result = user });
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteUser([FromQuery] Guid id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user is not null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return StatusCode(200, new { message = "Törölve.", result = user });
            }
            return StatusCode(400, new { message = "Hiba.", result = user });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTOs userLoginDtos)
        {
            var login = await _context.Users.SingleOrDefaultAsync(u => u.Name == userLoginDtos.Name);

            if (login is null)
            {
                return StatusCode(404, new { message = "A felhasználó nem található." });
            }

            var isValid = _password.Verify(userLoginDtos.Password, login.Password, login.Salt);

            if (isValid)
            {
                return StatusCode(200, new { message = "Sikeres bejelentkezés.", result = login });
            }
            return StatusCode(401, new { message = "Hibás jelszó vagy név.", result = login });
        }
    } 
}