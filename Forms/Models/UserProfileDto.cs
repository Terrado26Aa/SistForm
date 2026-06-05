using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace Forms.Models
{
    public class UserProfileDto
    {
        [JsonPropertyName("firstName")]
        public string FirstName { get; set; }
        
        [JsonPropertyName("lastName")]
        public string LastName { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("role")]
        public string? Role { get; set; }
        
        [JsonPropertyName("newPassword")]
        public string? NewPassword { get; set; }
    }
}
