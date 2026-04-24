namespace Report.Application.Interfaces;

public interface ISessionApiClient
{
    Task<CampaignData?> GetCampaignAsync(Guid campaignId, CancellationToken ct = default);
    Task<List<SlotData>> GetSlotsByCampaignAsync(Guid campaignId, CancellationToken ct = default);
    Task<SlotData?> GetSlotAsync(Guid slotId, CancellationToken ct = default);
    Task<GroupData?> GetCapstoneGroupAsync(Guid groupId, CancellationToken ct = default);
}

public interface IAssignmentApiClient
{
    Task<List<AssignmentData>> GetAllAssignmentsAsync(CancellationToken ct = default);
    Task<List<ReviewerData>> GetReviewersByAssignmentIdsAsync(List<Guid> assignmentIds, CancellationToken ct = default);
}

public interface IIdentityApiClient
{
    Task<string?> GetLecturerNameAsync(Guid lecturerId, CancellationToken ct = default);
}

public record CampaignData(Guid Id, string Name);
public record SlotData(Guid Id, Guid CampaignId, DateOnly ReviewDate, int SlotNumber, string StartTime, string EndTime, string Room);
public record GroupData(Guid Id, string GroupCode, string SubjectCode, string ProjectNameEn, string ProjectNameVn);
public record AssignmentData(Guid Id, Guid CapstoneGroupId, Guid ReviewSlotId, string Status, int ReviewOrder);
public record ReviewerData(Guid Id, Guid LecturerId, Guid ReviewAssignmentId, string Role);
