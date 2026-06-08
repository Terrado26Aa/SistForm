namespace SistFormAPI.Models
{
    public class RegisterRequestDto
    {
        public string Username { get; set; } = "";
        public string Firstname { get; set; } = "";
        public string Lastname { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = "User"; // Default role, can be set by admin
    }
}
