using SistFormAPI.Data;
using SistFormAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SistFormAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<(bool success, string message, int userId, string token, string role)> LoginAsync(LoginRequestDto loginRequest)
        {
            if (loginRequest == null || string.IsNullOrWhiteSpace(loginRequest.UserName) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                return (false, "Usuario y contraseña son requeridos", 0, "", "");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequest.UserName.ToLower());
            if (user == null)
            {
                return (false, "Usuario o contraseña incorrecto", 0, "", "");
            }

            if (!BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash))
            {
                return (false, "Usuario o contraseña incorrecta", 0, "", "");
            }

            var token = GenerateTokenJWT(user);
            return (true, $"Bienvenido {user.UserName}!", user.Id, token, user.Role);
        }

        public async Task<(bool success, string message)> RegisterAsync(RegisterRequestDto registerRequest)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == registerRequest.Username))
            {
                return (false, "El nombre de usuario ya esta en uso.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == registerRequest.Email))
            {
                return (false, "El correo electronico ya esta registrado");
            }

            var newUser = new User
            {
                UserName = registerRequest.Username,
                FirstName = registerRequest.Firstname,
                LastName = registerRequest.Lastname,
                Email = registerRequest.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
                Role = "User" // Default role for self-registration
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return (true, "Usuario registrado exitosamente.");
        }

        public async Task<(bool success, string message)> RegisterAdminAsync(RegisterRequestDto registerRequest)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == registerRequest.Username))
            {
                return (false, "El nombre de usuario ya esta en uso.");
            }

            if (await _context.Users.AnyAsync(u => u.Email == registerRequest.Email))
            {
                return (false, "El correo electronico ya esta registrado");
            }

            var newUser = new User
            {
                UserName = registerRequest.Username,
                FirstName = registerRequest.Firstname,
                LastName = registerRequest.Lastname,
                Email = registerRequest.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
                Role = registerRequest.Role ?? "User"
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return (true, "Usuario creado exitosamente por el administrador.");
        }

        public async Task<IEnumerable<object>> GetAllUsersAsync()
        {
            return await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.FirstName,
                    u.LastName,
                    u.Email,
                    u.Role
                })
                .ToListAsync();
        }

        public async Task<(bool success, string message)> UpdateUserRoleAsync(int userId, string newRole)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return (false, "Usuario no encontrado.");

            user.Role = newRole;
            await _context.SaveChangesAsync();
            return (true, "Rol de usuario actualizado exitosamente.");
        }

        public async Task<(bool success, string message)> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return (false, "Usuario no encontrado.");

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return (true, "Usuario eliminado exitosamente.");
        }

        public async Task<UserProfileDto?> GetProfileAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            return new UserProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role
            };
        }

        public async Task<(bool success, string message)> UpdateProfileAsync(int id, UserProfileDto dto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return (false, "Usuario no encontrado.");

            user.FirstName = dto.FirstName;
            user.LastName = dto.LastName;
            user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.NewPassword))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            }

            await _context.SaveChangesAsync();
            return (true, "Perfil actualizado exitosamente.");
        }

        private string GenerateTokenJWT(User user)
        {
            var secretKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found");
            var keyBytes = Encoding.ASCII.GetBytes(secretKey);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role), // Include role
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(keyBytes),
                SecurityAlgorithms.HmacSha256Signature
            );

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(8),
                SigningCredentials = signingCredentials,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
