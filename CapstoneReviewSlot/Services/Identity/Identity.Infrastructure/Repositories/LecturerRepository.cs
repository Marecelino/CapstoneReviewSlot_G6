using Identity.Domain.Entities;
using Identity.Domain.Interfaces;
using Identity.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Repositories;

public class LecturerRepository : ILecturerRepository
{
    private readonly IdentityDbContext _ctx;
    public LecturerRepository(IdentityDbContext ctx) => _ctx = ctx;

    public async Task<Lecturer?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _ctx.Lecturers.FirstOrDefaultAsync(l => l.UserId == userId, ct);

    public async Task<Lecturer?> GetByCodeAsync(string lecturerCode, CancellationToken ct = default)
        => await _ctx.Lecturers.FirstOrDefaultAsync(
            l => l.LecturerCode == lecturerCode.ToUpperInvariant(), ct);

    public async Task AddAsync(Lecturer lecturer, CancellationToken ct = default)
        => await _ctx.Lecturers.AddAsync(lecturer, ct);

    public async Task<bool> ExistsByCodeAsync(string lecturerCode, CancellationToken ct = default)
        => await _ctx.Lecturers.AnyAsync(
            l => l.LecturerCode == lecturerCode.ToUpperInvariant(), ct);
}
