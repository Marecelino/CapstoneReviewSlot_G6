namespace Session.Domain.DTOs;

public class ReviewSlotDto
{
    public Guid Id { get; set; }
    public Guid CampaignId { get; set; }
    public DateOnly ReviewDate { get; set; }
    public int SlotNumber { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string Room { get; set; } = default!;
    public int MaxCapacity { get; set; }

    public ReviewSlotDto() { }

    public ReviewSlotDto(Guid id, Guid campaignId, DateOnly reviewDate, int slotNumber,
        TimeOnly startTime, TimeOnly endTime, string room, int maxCapacity)
    {
        Id = id;
        CampaignId = campaignId;
        ReviewDate = reviewDate;
        SlotNumber = slotNumber;
        StartTime = startTime;
        EndTime = endTime;
        Room = room;
        MaxCapacity = maxCapacity;
    }
}
