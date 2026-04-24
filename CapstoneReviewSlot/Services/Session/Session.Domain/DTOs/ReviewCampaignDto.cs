using Session.Domain.Enums;

namespace Session.Domain.DTOs;

public class ReviewCampaignDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = default!;
    public int MaxGroupsPerLecturer { get; set; }
    public int RequiredReviewersPerGroup { get; set; }
    public List<ReviewSlotDto> ReviewSlots { get; set; } = new();

    public ReviewCampaignDto() { }

    public ReviewCampaignDto(Guid id, string name, DateTime startTime, DateTime endTime,
        int maxGroups, int requiredReviewers, string status)
    {
        Id = id;
        Name = name;
        StartTime = startTime;
        EndTime = endTime;
        MaxGroupsPerLecturer = maxGroups;
        RequiredReviewersPerGroup = requiredReviewers;
        Status = status;
    }
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
