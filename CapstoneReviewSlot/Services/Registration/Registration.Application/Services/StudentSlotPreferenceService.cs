using Registration.Application.Interfaces;
using Registration.Application.Ultils;
using Registration.Domain.DTOs;
using Registration.Domain.Entities;
using Registration.Domain.Interfaces;

namespace Registration.Application.Services;

public class StudentSlotPreferenceService : IStudentSlotPreferenceService
{
    private readonly IStudentSlotPreferenceRepository _repo;
    public StudentSlotPreferenceService(IStudentSlotPreferenceRepository repo) => _repo = repo;

    public async Task<StudentSlotPreferenceDto> RegisterPreferenceAsync(
        CreateStudentSlotPreferenceDto dto, CancellationToken ct = default)
    {
        if (dto.PreferenceOrder < 1 || dto.PreferenceOrder > 5)
            throw ErrorHelper.BadRequest("PreferenceOrder must be between 1 and 5.");

        var exists = await _repo.ExistsAsync(dto.CapstoneGroupId, dto.ReviewSlotId, dto.StudentMssv, ct);
        if (exists)
            throw ErrorHelper.Conflict("This preference already exists for the student in this slot.");

        var pref = StudentSlotPreference.Create(
            dto.CapstoneGroupId,
            dto.ReviewSlotId,
            dto.PreferenceOrder,
            dto.StudentMssv);

        await _repo.AddAsync(pref, ct);
        return ToDto(pref);
    }

    public async Task<bool> CancelPreferenceAsync(Guid id, CancellationToken ct = default)
    {
        var pref = await _repo.GetByIdAsync(id, ct)
            ?? throw ErrorHelper.NotFound("Preference not found.");

        await _repo.SoftRemoveAsync(pref, ct);
        return true;
    }

    public async Task<List<StudentSlotPreferenceDto>> GetByGroupAsync(Guid groupId, CancellationToken ct = default)
    {
        var prefs = await _repo.GetByGroupAsync(groupId, ct);
        return prefs.Select(ToDto).ToList();
    }

    public async Task<List<StudentSlotPreferenceDto>> GetBySlotAsync(Guid slotId, CancellationToken ct = default)
    {
        var prefs = await _repo.GetBySlotAsync(slotId, ct);
        return prefs.Select(ToDto).ToList();
    }

    public async Task<int> GetRegistrationCountAsync(Guid slotId, CancellationToken ct = default)
        => await _repo.GetCountBySlotAsync(slotId, ct);

    private static StudentSlotPreferenceDto ToDto(StudentSlotPreference p)
        => new(p.Id, p.CapstoneGroupId, p.ReviewSlotId, p.PreferenceOrder, p.StudentMssv, p.CreatedAtUtc);
}
