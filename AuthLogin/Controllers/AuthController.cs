using AuthLogin.Data;
using AuthLogin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace AuthLogin.Controllers
{
    [Route("api/[controller]")]
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            //validación del modelo automatica (gracias a [ApiController]).
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //verificar si el usuario o email ya existe en la base de datos.
            if (await _context.Users.AnyAsync(u => u.UserName == registerRequest.Username))
            {
                return BadRequest("El nombre de usuario ya esta en uso.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == registerRequest.Email))
            {
                return BadRequest("El correo electronico ya esta registrado");
            }

            //Crear la nueva entidad de Usuario.
            var newUser = new User
            {
                UserName = registerRequest.Username,
                FirstName = registerRequest.Firstname,
                LastName = registerRequest.Lastname,
                Email = registerRequest.Email,
                //Hashear la contraseña antes de guardar
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password)
            };

            //Guardar el nuevo usuario en la base de datos.
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            //Devolver una respuesta exitosa.
            return Ok(new { Message = "Usuario registrado exitosamente." });
        }

        [HttpGet("hash/{password}")]
        public IActionResult HashPassword(string password)
        {
            if (string.IsNullOrEmpty(password))
            {
                return BadRequest("Password no puede ser vacía.");
            }

            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            return Ok(new { OriginalPassword = password, HashedPassword = hashedPassword });
        }
    }
}
