using Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Abstractions.Security
{
    public interface IPasswordHasherService
    {
        string HashPassword(User user, string password);
        bool VerifyPassword(User user, string password);
    }
}
