using Identity.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Application.Abstractions.Persistence
{
    public interface IPasswordResetOtpRepository
    {
        Task AddAsync(PasswordResetOtp entity, CancellationToken cancellationToken = default);

        Task<PasswordResetOtp?> GetLatestByEmailAsync(string email, CancellationToken cancellationToken = default);

        void Update(PasswordResetOtp entity);
    }
}
