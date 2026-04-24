using Registration.Domain.DTOs;

namespace Registration.Application.Interfaces;

public interface IStudentSlotPreferenceService
{
    Task<StudentSlotPreferenceDto> RegisterPreferenceAsync(CreateStudentSlotPreferenceDto dto, CancellationToken ct = default);
    Task<bool> CancelPreferenceAsync(Guid id, CancellationToken ct = default);
    Task<List<StudentSlotPreferenceDto>> GetByGroupAsync(Guid groupId, CancellationToken ct = default);
    Task<List<StudentSlotPreferenceDto>> GetBySlotAsync(Guid slotId, CancellationToken ct = default);
    Task<int> GetRegistrationCountAsync(Guid slotId, CancellationToken ct = default);
}
