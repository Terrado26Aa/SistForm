using AuthLogin.Models;

namespace AuthLogin.Services
{
    public interface IAuthService
    {
        Task<(bool success, string message, int userId, string token, string role)> LoginAsync(LoginRequestDto loginRequest);
        Task<(bool success, string message)> RegisterAsync(RegisterRequestDto registerRequest);
        Task<(bool success, string message)> RegisterAdminAsync(RegisterRequestDto registerRequest);
        Task<UserProfileDto?> GetProfileAsync(int id);
        Task<(bool success, string message)> UpdateProfileAsync(int id, UserProfileDto dto);
        Task<IEnumerable<object>> GetAllUsersAsync();
        Task<(bool success, string message)> UpdateUserRoleAsync(int userId, string newRole);
        Task<(bool success, string message)> DeleteUserAsync(int userId);
    }
}
