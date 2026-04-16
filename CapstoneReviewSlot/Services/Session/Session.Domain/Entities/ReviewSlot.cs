using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Session.Domain.Entities
{
    public class ReviewSlot : BaseEntity
    {
        public Guid CampaignId { get; set; }
        public DateTime ReviewDate { get; set; }
        public int SlotNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Room { get; set; } = default!;
        public int MaxCapacity { get; set; }

        public ReviewCampaign ReviewCampaign { get; set; } = default!;
    }
}
