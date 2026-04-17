namespace Availability.Domain.Entities
using Availability.Domain.Enums;
using Entities;

public class LecturerAvailability : BaseEntity
{
    public int LecturerId { get; private set; }
    public Guid ReviewSlotId { get; private set; }
    public AvailabilityStatus Status { get; private set; } = AvailabilityStatus.Available;
    public DateTime RegisteredAt { get; private set; } = DateTime.UtcNow;

    public static LecturerAvailability Create(int lecturerId, Guid reviewSlotId)
    {
        return new LecturerAvailability
        {
            LecturerId = lecturerId,
            ReviewSlotId = reviewSlotId,
            Status = AvailabilityStatus.Available,
            RegisteredAt = DateTime.UtcNow
        };
    }

    public void Cancel()
    {
        Status = AvailabilityStatus.Cancelled;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
