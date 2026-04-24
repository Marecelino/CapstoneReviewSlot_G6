using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public class PasswordResetOtpRepository : IPasswordResetOtpRepository
{
    private readonly IdentityDbContext _context;

    public PasswordResetOtpRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(PasswordResetOtp entity, CancellationToken cancellationToken = default)
    {
        await _context.PasswordResetOtps.AddAsync(entity, cancellationToken);
    }

    public async Task<PasswordResetOtp?> GetLatestByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.PasswordResetOtps
            .Where(x => x.Email == email && !x.IsDeleted)
            .OrderByDescending(x => x.CreatedAtUtc)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public void Update(PasswordResetOtp entity)
    {
        _context.PasswordResetOtps.Update(entity);
    }
}