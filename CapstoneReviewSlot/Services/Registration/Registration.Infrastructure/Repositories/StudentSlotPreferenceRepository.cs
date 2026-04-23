using Microsoft.EntityFrameworkCore;
using Registration.Domain.Entities;
using Registration.Domain.Interfaces;
using Registration.Infrastructure.Persistence;

namespace Registration.Infrastructure.Repositories;

public class StudentSlotPreferenceRepository : IStudentSlotPreferenceRepository
{
    private readonly RegistrationDbContext _ctx;
    public StudentSlotPreferenceRepository(RegistrationDbContext ctx) => _ctx = ctx;

    public async Task<StudentSlotPreference?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _ctx.StudentSlotPreferences.FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IEnumerable<StudentSlotPreference>> GetByGroupAsync(Guid groupId, CancellationToken ct = default)
        => await _ctx.StudentSlotPreferences
                     .Where(p => p.CapstoneGroupId == groupId)
                     .OrderBy(p => p.PreferenceOrder)
                     .ToListAsync(ct);

    public async Task<IEnumerable<StudentSlotPreference>> GetBySlotAsync(Guid slotId, CancellationToken ct = default)
        => await _ctx.StudentSlotPreferences
                     .Where(p => p.ReviewSlotId == slotId)
                     .ToListAsync(ct);

    public async Task<bool> ExistsAsync(Guid groupId, Guid slotId, string mssv, CancellationToken ct = default)
        => await _ctx.StudentSlotPreferences.AnyAsync(
            p => p.CapstoneGroupId == groupId && p.ReviewSlotId == slotId && p.StudentMssv == mssv, ct);

    public async Task<int> GetCountBySlotAsync(Guid slotId, CancellationToken ct = default)
        => await _ctx.StudentSlotPreferences.CountAsync(p => p.ReviewSlotId == slotId, ct);

    public async Task AddAsync(StudentSlotPreference pref, CancellationToken ct = default)
        => await _ctx.StudentSlotPreferences.AddAsync(pref, ct);

    public Task UpdateAsync(StudentSlotPreference pref, CancellationToken ct = default)
    {
        _ctx.StudentSlotPreferences.Update(pref);
        return Task.CompletedTask;
    }

    public Task SoftRemoveAsync(StudentSlotPreference pref, CancellationToken ct = default)
    {
        pref.IsDeleted = true;
        pref.UpdatedAtUtc = DateTime.UtcNow;
        _ctx.StudentSlotPreferences.Update(pref);
        return Task.CompletedTask;
    }
}
