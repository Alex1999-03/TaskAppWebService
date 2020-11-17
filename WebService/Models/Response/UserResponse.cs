using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WebService.Models.Response
{
    public class UserResponse
    {
        public string Email { get; set; }
        public string Token { get; set; }
        [JsonIgnore] // Devolveremos el refresh token mediante http como una cookie
        public string RefreshToken { get; set; }

    }
}
