using Registration.Domain.Entities;

namespace Registration.Domain.Interfaces;

public interface IStudentSlotPreferenceRepository
{
    Task<StudentSlotPreference?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<StudentSlotPreference>> GetByGroupAsync(Guid groupId, CancellationToken ct = default);
    Task<IEnumerable<StudentSlotPreference>> GetBySlotAsync(Guid slotId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid groupId, Guid slotId, string mssv, CancellationToken ct = default);
    Task<int> GetCountBySlotAsync(Guid slotId, CancellationToken ct = default);
    Task AddAsync(StudentSlotPreference pref, CancellationToken ct = default);
    Task UpdateAsync(StudentSlotPreference pref, CancellationToken ct = default);
    Task SoftRemoveAsync(StudentSlotPreference pref, CancellationToken ct = default);
}
