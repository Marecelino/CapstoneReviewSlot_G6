using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.DTOs.Auth
{
    public sealed class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
