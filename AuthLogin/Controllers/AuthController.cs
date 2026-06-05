using AuthLogin.Models;
using AuthLogin.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace AuthLogin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var result = await _authService.LoginAsync(loginRequest);
            if (!result.success)
            {
                return Unauthorized(result.message);
            }

            return Ok(new { Message = result.message, UserId = result.userId, Token = result.token, Role = result.role });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAsync(registerRequest);
            if (!result.success)
            {
                return BadRequest(result.message);
            }

            return Ok(new { Message = result.message });
        }

        [Authorize]
        [HttpGet("profile/{id}")]
        public async Task<IActionResult> GetProfile(int id)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != id.ToString())
            {
                return Forbid();
            }

            var profile = await _authService.GetProfileAsync(id);
            if (profile == null) return NotFound("Usuario no encontrado.");

            return Ok(profile);
        }

        [Authorize]
        [HttpPut("profile/{id}")]
        public async Task<IActionResult> UpdateProfile(int id, [FromBody] UserProfileDto dto)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (currentUserId != id.ToString())
            {
                return Forbid();
            }

            var result = await _authService.UpdateProfileAsync(id, dto);
            if (!result.success)
            {
                return NotFound(result.message);
            }

            return Ok(new { Message = result.message });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterRequestDto registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authService.RegisterAdminAsync(registerRequest);
            if (!result.success)
            {
                return BadRequest(result.message);
            }

            return Ok(new { Message = result.message });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _authService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("users/{id}/role")]
        public async Task<IActionResult> UpdateUserRole(int id, [FromBody] string newRole)
        {
            if (newRole != "Admin" && newRole != "User")
            {
                return BadRequest("Rol invalido. Debe ser 'Admin' o 'User'.");
            }

            var result = await _authService.UpdateUserRoleAsync(id, newRole);
            if (!result.success)
            {
                return NotFound(result.message);
            }

            return Ok(new { Message = result.message });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var result = await _authService.DeleteUserAsync(id);
            if (!result.success)
            {
                return NotFound(result.message);
            }

            return Ok(new { Message = result.message });
        }
    }
}
