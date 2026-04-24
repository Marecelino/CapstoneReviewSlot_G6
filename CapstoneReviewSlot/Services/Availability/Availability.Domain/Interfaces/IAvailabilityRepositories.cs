using Availability.Domain.Entities;

namespace Availability.Domain.Interfaces;

public interface ILecturerAvailabilityRepository
{
    Task<LecturerAvailability?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<LecturerAvailability?> GetByLecturerAndSlotAsync(Guid lecturerId, Guid slotId, CancellationToken ct = default);
    Task<IEnumerable<LecturerAvailability>> GetByLecturerIdAsync(Guid lecturerId, CancellationToken ct = default);
    Task<IEnumerable<LecturerAvailability>> GetBySlotIdAsync(Guid slotId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid lecturerId, Guid slotId, CancellationToken ct = default);
    Task AddAsync(LecturerAvailability entity, CancellationToken ct = default);
    Task AddRangeAsync(IEnumerable<LecturerAvailability> entities, CancellationToken ct = default);
    Task UpdateAsync(LecturerAvailability entity, CancellationToken ct = default);

    Task DeleteAsync(LecturerAvailability entity, CancellationToken ct = default);
    Task<IEnumerable<LecturerAvailability>> GetAllAsync(CancellationToken ct = default);
}

public interface IUnitOfWork
{
    ILecturerAvailabilityRepository Availabilities { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}
