using Session.Domain.Enums;

namespace Session.Application.DTOs;

public record ReviewCampaignDto(
    Guid CampaignId,
    string Name,
    DateTime StartTime,
    DateTime EndTime,
    int MaxGroupsPerLecturer,
    int RequiredReviewersPerGroup,
    string Status);

public record ReviewSlotDto(
    Guid ReviewSlotId,
    Guid CampaignId,
    DateOnly ReviewDate,
    int SlotNumber,
    TimeOnly StartTime,
    TimeOnly EndTime,
    string? Room,
    int MaxCapacity);
