using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Todos_API.Application.DTOs.Auth
{
    public class LoginResultDto
    {
        public string Token { get; set; }
        public string Username { get; set; } = string.Empty;

                public string Email { get; set; }
        public string UserId { get; set; }
    }
}
