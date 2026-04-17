namespace Session.Domain.DTOs
{
    public class ReviewSlotDto
    {
        public Guid Id { get; set; }
        public Guid CampaignId { get; set; }
        public DateTime ReviewDate { get; set; }
        public int SlotNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Room { get; set; } = default!;
        public int MaxCapacity { get; set; }
    }
}
