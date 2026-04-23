using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Domain.Entities
{
    public class PasswordResetOtp : BaseEntity
    {
        public Guid UserId { get; set; }

        public string Email { get; set; } = string.Empty;

        public string OtpHash { get; set; } = string.Empty;

        public DateTime ExpiresAtUtc { get; set; }

        public DateTime? VerifiedAtUtc { get; set; }

        public DateTime? UsedAtUtc { get; set; }
    }
}
