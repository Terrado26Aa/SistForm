using AuthLogin.Data;
using AuthLogin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AuthLogin.Controllers
{
    [Route ("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context; //Inyecta tu DbContext

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.UserName)
                || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return BadRequest("Usuario y contraseña son requeridos");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequest.UserName.ToLower());

            if (user == null)
            {
                // Usuario no encontrado
                return Unauthorized("Usuario no encontrado");
            }

            // Verifica la contraseña usando BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return Unauthorized("Contraseña incorrecta");
            }

            return Ok(new { Message = $"Bienvenido {user.UserName}!", UserId = user.Id });
        }
    }
}
