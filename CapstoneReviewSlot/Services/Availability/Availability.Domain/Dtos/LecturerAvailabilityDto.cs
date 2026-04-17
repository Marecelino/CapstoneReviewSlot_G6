using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Domain.Dtos
{
    public class LecturerAvailabilityDto
    {
        public Guid Id { get; set; }
        public Guid LecturerId { get; set; }
        public Guid ReviewSlotId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

    }

    public class CreateLecturerAvailabilityDto
    {
        public Guid LecturerId { get; set; }
        public Guid ReviewSlotId { get; set; }
    }

    public class UpdateLecturerAvailabilityDto
    {
        public Guid LecturerId { get; set; }
        public Guid ReviewSlotId { get; set; }
    }
}
