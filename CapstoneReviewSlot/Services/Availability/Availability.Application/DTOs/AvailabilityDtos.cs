using Availability.Domain.Enums;

namespace Availability.Application.DTOs;

public record LecturerAvailabilityDto(
    Guid Id,
    Guid LecturerId,
    Guid ReviewSlotId,
    AvailabilityStatus Status,
    DateTime RegisteredAt);

public record RegisterAvailabilityRequest(IReadOnlyList<Guid> SlotIds);
