using Availability.Domain.Entities;
using Availability.Domain.Interfaces;
using Availability.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Availability.Infrastructure.Repositories;

public class LecturerAvailabilityRepository : ILecturerAvailabilityRepository
{
    private readonly AvailabilityDbContext _ctx;
    public LecturerAvailabilityRepository(AvailabilityDbContext ctx) => _ctx = ctx;

    public async Task<LecturerAvailability?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.LecturerAvailabilities.FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<LecturerAvailability?> GetByLecturerAndSlotAsync(
        Guid lecturerId, Guid slotId, CancellationToken ct = default)
        => await _ctx.LecturerAvailabilities.FirstOrDefaultAsync(
            a => a.LecturerId == lecturerId && a.ReviewSlotId == slotId, ct);

    public async Task<IEnumerable<LecturerAvailability>> GetByLecturerIdAsync(
        Guid lecturerId, CancellationToken ct = default)
        => await _ctx.LecturerAvailabilities
                     .Where(a => a.LecturerId == lecturerId)
                     .OrderBy(a => a.ReviewSlotId)
                     .ToListAsync(ct);

    public async Task<IEnumerable<LecturerAvailability>> GetAllAsync(CancellationToken ct = default)
        => await _ctx.LecturerAvailabilities
                     .OrderBy(a => a.ReviewSlotId)
                     .ToListAsync(ct);

    public async Task<IEnumerable<LecturerAvailability>> GetBySlotIdAsync(
        Guid slotId, CancellationToken ct = default)
        => await _ctx.LecturerAvailabilities
                     .Where(a => a.ReviewSlotId == slotId)
                     .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid lecturerId, Guid slotId, CancellationToken ct = default)
        => await _ctx.LecturerAvailabilities.AnyAsync(
            a => a.LecturerId == lecturerId && a.ReviewSlotId == slotId, ct);

    public async Task AddAsync(LecturerAvailability entity, CancellationToken ct = default)
        => await _ctx.LecturerAvailabilities.AddAsync(entity, ct);

    public async Task AddRangeAsync(IEnumerable<LecturerAvailability> entities, CancellationToken ct = default)
        => await _ctx.LecturerAvailabilities.AddRangeAsync(entities, ct);

    public Task UpdateAsync(LecturerAvailability entity, CancellationToken ct = default)
    {
        _ctx.LecturerAvailabilities.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(LecturerAvailability entity, CancellationToken ct = default)
    {
        _ctx.LecturerAvailabilities.Remove(entity);
        return Task.CompletedTask;
    }
}

public class UnitOfWork : IUnitOfWork
{
    private readonly AvailabilityDbContext _ctx;
    private LecturerAvailabilityRepository? _availabilities;

    public UnitOfWork(AvailabilityDbContext ctx) => _ctx = ctx;

    public ILecturerAvailabilityRepository Availabilities
        => _availabilities ??= new LecturerAvailabilityRepository(_ctx);

    public async Task<int> SaveChangesAsync(CancellationToken ct = default)
        => await _ctx.SaveChangesAsync(ct);
}
