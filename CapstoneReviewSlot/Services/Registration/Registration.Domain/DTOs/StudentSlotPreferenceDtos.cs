namespace Registration.Domain.DTOs;

public record StudentSlotPreferenceDto(
    Guid Id,
    Guid CapstoneGroupId,
    Guid ReviewSlotId,
    int PreferenceOrder,
    string StudentMssv,
    DateTime CreatedAtUtc);

public record CreateStudentSlotPreferenceDto(
    Guid CapstoneGroupId,
    Guid ReviewSlotId,
    int PreferenceOrder,
    string StudentMssv);

public record SlotAvailabilityDto(
    Guid SlotId,
    Guid CampaignId,
    DateOnly ReviewDate,
    int SlotNumber,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string Room,
    int CurrentRegistrations,
    int MaxCapacity);

public record SlotPreferenceResultDto(
    int TotalSlots,
    int AvailableSlots,
    List<SlotAvailabilityDto> Slots);
