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
        public DateOnly ReviewDate { get; set; }
        public int SlotNumber { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public string Room { get; set; } = default!;
        public int MaxCapacity { get; set; }

        public ReviewCampaign ReviewCampaign { get; set; } = default!;

        private ReviewSlot() { }

        public static ReviewSlot Create(Guid campaignId, DateOnly reviewDate, int slotNumber, TimeOnly startTime, TimeOnly endTime, int maxCapacity, string? room)
        {
            return new ReviewSlot
            {
                CampaignId = campaignId,
                ReviewDate = reviewDate,
                SlotNumber = slotNumber,
                StartTime = startTime,
                EndTime = endTime,
                MaxCapacity = maxCapacity,
                Room = room ?? string.Empty
            };
        }
    }
}
