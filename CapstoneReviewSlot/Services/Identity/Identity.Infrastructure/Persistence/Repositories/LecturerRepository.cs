using Identity.Application.Abstractions.Persistence;
using Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Persistence.Repositories;

public class LecturerRepository : ILecturerRepository
{
    private readonly IdentityDbContext _context;

    public LecturerRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<Lecturer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        => await _context.Lecturers
            .FirstOrDefaultAsync(l => l.UserId == userId, cancellationToken);

    public async Task<Lecturer?> GetByCodeAsync(string lecturerCode, CancellationToken cancellationToken = default)
        => await _context.Lecturers
            .FirstOrDefaultAsync(l => l.LecturerCode == lecturerCode.ToUpperInvariant(), cancellationToken);

    public async Task AddAsync(Lecturer lecturer, CancellationToken cancellationToken = default)
        => await _context.Lecturers.AddAsync(lecturer, cancellationToken);

    public async Task<bool> ExistsByCodeAsync(string lecturerCode, CancellationToken cancellationToken = default)
        => await _context.Lecturers
            .AnyAsync(l => l.LecturerCode == lecturerCode.ToUpperInvariant(), cancellationToken);
}
