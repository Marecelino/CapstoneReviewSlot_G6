using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Domain.Dtos
{
    public class ReviewAssignmentReviewerDto
    {
        public Guid LecturerId { get; set; }
        public Guid ReviewAssignmentId { get; set; }
        public string Role { get; set; } = default!;

    }
}
