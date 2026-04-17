using Entities;

namespace Availability.Domain.Entities
{
    public class LecturerAvailability : BaseEntity
    {
        public Guid LecturerId { get; set; }
        public Guid ReviewSlotId { get; set; }
    }
}
