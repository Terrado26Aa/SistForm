using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.Json.Serialization;

namespace Forms.Models
{
    public class LoginResponseDto
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
        
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        
        [JsonPropertyName("token")]
        public string Token { get; set; }
        
        [JsonPropertyName("role")]
        public string? Role { get; set; }
    }
}
