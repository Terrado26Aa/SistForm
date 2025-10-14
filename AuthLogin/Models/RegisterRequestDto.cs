using System.ComponentModel.DataAnnotations;

namespace AuthLogin.Models
{
    public class RegisterRequestDto
    {
        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Firstname { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 5)]
        public string Lastname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
