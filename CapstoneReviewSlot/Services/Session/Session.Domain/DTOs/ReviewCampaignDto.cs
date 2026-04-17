namespace Session.Domain.DTOs
{
    public class ReviewCampaignDto
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }

        public List<ReviewSlotDto> ReviewSlots { get; set; } = new List<ReviewSlotDto>();
    }

    public class CreateReviewCampaignDto
    {
        public string Name { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class UpdateReviewCampaignDto
    {
        public string Name { get; set; } = default!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
