using Identity.Domain.Entities;

namespace Identity.Application.Abstractions.Persistence;

public interface ILecturerRepository
{
    Task<Lecturer?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Lecturer?> GetByCodeAsync(string lecturerCode, CancellationToken cancellationToken = default);
    Task AddAsync(Lecturer lecturer, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string lecturerCode, CancellationToken cancellationToken = default);
}
