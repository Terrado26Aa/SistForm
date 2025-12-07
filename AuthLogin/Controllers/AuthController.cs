using AuthLogin.Data;
using AuthLogin.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context; //Inyecta tu DbContext
        private readonly IConfiguration _configuration; //Inyecta la Configuracion

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            // validar que no sea nulo o vacio
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
                return Unauthorized("Usuario o contraseña incorrecto");
            }

            // Verifica la contraseña usando BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return Unauthorized("Usuario o contraseña incorrecta");
            }

            var tokenString = GenerateTokenJWT(user);
            return Ok(new { Message = $"Bienvenido {user.UserName}!", UserId = user.Id, Token = tokenString });
        }

        // Generar el token JWT
        private string GenerateTokenJWT(User user)
        {
            // Obtener la clave secreta desde la configuración appsettings.json
            var secretKey = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("La clave secreta JWT no está configurada en appsettings.json");
            }
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);

            // Crea los claims(informacion que ira dentro del token)
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Crea las credenciales de firma
            var singingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
                );

            // Crea el descriptor del token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = singingCredentials,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            // Crea el token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Devuelve el token en formato string
            return tokenHandler.WriteToken(token);
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
