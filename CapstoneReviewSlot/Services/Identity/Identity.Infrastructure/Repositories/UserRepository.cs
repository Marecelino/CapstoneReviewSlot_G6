using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _ctx;
    public UserRepository(IdentityDbContext ctx) => _ctx = ctx;

    public async Task<User?> GetByIdAsync(Guid userId, CancellationToken ct = default)
        => await _ctx.Users.Include(u => u.Lecturer)
                           .FirstOrDefaultAsync(u => u.Id == userId, ct);

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
        => await _ctx.Users.Include(u => u.Lecturer)
                           .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.Users.Include(u => u.Lecturer).ToListAsync(ct);

    public async Task AddAsync(User user, CancellationToken ct = default)
        => await _ctx.Users.AddAsync(user, ct);

    public Task UpdateAsync(User user, CancellationToken ct = default)
    {
        _ctx.Users.Update(user);
        return Task.CompletedTask;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default)
        => await _ctx.Users.AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);
}
