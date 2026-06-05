namespace AuthLogin.Models
{
    public class UserProfileDto
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
        public string? Role { get; set; }
        public string? NewPassword { get; set; }
    }
}
