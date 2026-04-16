using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Domain.Entities
{
    public class ReviewAssignment : BaseEntity
    {
        public Guid CapstoneGroupId { get; set; }
        public Guid ReviewSlotId { get; set; }
        public string Status { get; set; } = default!;
        public int ReviewOrder { get; set; }
        public Guid AssignedBy { get; set; }
        public DateTime AssignedAt { get; set; }

        public ICollection<ReviewAssignmentReviewer> Reviewers { get; set; } = new List<ReviewAssignmentReviewer>();
    }
}
