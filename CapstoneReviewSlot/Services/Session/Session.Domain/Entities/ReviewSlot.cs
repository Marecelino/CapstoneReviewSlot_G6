using Entities;

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

        public ReviewCampaign? ReviewCampaign { get; set; }
    }
}
