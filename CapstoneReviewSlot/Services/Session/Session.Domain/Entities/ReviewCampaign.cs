using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Session.Domain.Entities
{
    public class ReviewCampaign : BaseEntity
    {
        public string Name { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = default!;

        public ICollection<ReviewSlot> ReviewSlots { get; set; } = new List<ReviewSlot>();
    }
}
