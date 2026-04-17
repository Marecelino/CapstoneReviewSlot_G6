using Assignment.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Domain.Dtos
{
    public class ReviewAssignmentDto
    {
        public Guid Id { get; set; }
        public Guid CapstoneGroupId { get; set; }
        public Guid ReviewSlotId { get; set; }
        public string Status { get; set; } = default!;
        public int ReviewOrder { get; set; }
        public Guid AssignedBy { get; set; }
        public DateTime AssignedAt { get; set; }

        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get;set; }
    }

    public class ReviewAssignmentRequest
    {
        public Guid CapstoneGroupId { get; set; }
        public Guid ReviewSlotId { get; set; }
        public Guid AssignedBy { get; set; }
    }
}
