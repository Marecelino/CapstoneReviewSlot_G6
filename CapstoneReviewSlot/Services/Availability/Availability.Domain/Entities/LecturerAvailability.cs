using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Availability.Domain.Entities
{
    public class LecturerAvailability : BaseEntity
    {
        public Guid LecturerId { get; set; }
        public Guid ReviewSlotId { get; set; }
    }
}
