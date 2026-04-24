using Report.Application.Interfaces;
using Report.Domain.DTOs;

namespace Report.Application.Services;

public class ReportService : IReportService
{
    private readonly ISessionApiClient _sessionClient;
    private readonly IAssignmentApiClient _assignmentClient;
    private readonly IIdentityApiClient _identityClient;
    private readonly IReportExporter _exportService;

    public ReportService(
        ISessionApiClient sessionClient,
        IAssignmentApiClient assignmentClient,
        IIdentityApiClient identityClient,
        IReportExporter exportService)
    {
        _sessionClient = sessionClient;
        _assignmentClient = assignmentClient;
        _identityClient = identityClient;
        _exportService = exportService;
    }

    public async Task<ReviewTimelineDto> GetReviewTimelineAsync(Guid campaignId, CancellationToken ct = default)
    {
        var campaign = await _sessionClient.GetCampaignAsync(campaignId, ct);
        var campaignName = campaign?.Name ?? $"Campaign {campaignId}";

        var slots = await _sessionClient.GetSlotsByCampaignAsync(campaignId, ct);
        var slotMap = slots.ToDictionary(s => s.Id);

        var assignments = await _assignmentClient.GetAllAssignmentsAsync(ct);
        var campaignAssignments = assignments
            .Where(a => slotMap.ContainsKey(a.ReviewSlotId))
            .ToList();

        var assignmentIds = campaignAssignments.Select(a => a.Id).ToList();
        var reviewers = await _assignmentClient.GetReviewersByAssignmentIdsAsync(assignmentIds, ct);

        var reviewerMap = reviewers
            .GroupBy(r => r.ReviewAssignmentId)
            .ToDictionary(g => g.Key, g => g.ToList());

        var rows = new List<ReviewTimelineRowDto>();

        foreach (var assignment in campaignAssignments)
        {
            if (!slotMap.TryGetValue(assignment.ReviewSlotId, out var slot))
                continue;

            var assignmentReviewers = reviewerMap.GetValueOrDefault(assignment.Id) ?? new List<ReviewerData>();

            var primary = assignmentReviewers
                .FirstOrDefault(r => r.Role.Equals("primaryreviewer", StringComparison.OrdinalIgnoreCase));
            var secondary = assignmentReviewers
                .FirstOrDefault(r => r.Role.Equals("secondaryreviewer", StringComparison.OrdinalIgnoreCase));

            var group = await _sessionClient.GetCapstoneGroupAsync(assignment.CapstoneGroupId, ct);

            var reviewer1Name = primary != null
                ? await _identityClient.GetLecturerNameAsync(primary.LecturerId, ct) ?? primary.LecturerId.ToString()
                : "";
            var reviewer2Name = secondary != null
                ? await _identityClient.GetLecturerNameAsync(secondary.LecturerId, ct) ?? secondary.LecturerId.ToString()
                : "";

            rows.Add(new ReviewTimelineRowDto(
                WeekCode: $"W{GetWeekOfYear(slot.ReviewDate)}",
                DayCode: slot.ReviewDate.ToString("yyyy-MM-dd"),
                SlotCode: $"Slot {slot.SlotNumber}",
                GroupCode: group?.GroupCode ?? assignment.CapstoneGroupId.ToString(),
                ProjectCode: group?.SubjectCode ?? "",
                ProjectNameEn: group?.ProjectNameEn ?? "",
                Date: slot.ReviewDate,
                DayOfWeek: slot.ReviewDate.DayOfWeek.ToString(),
                Room: slot.Room,
                Reviewer1Name: reviewer1Name,
                Reviewer2Name: reviewer2Name,
                Reviewer1Id: primary?.LecturerId ?? Guid.Empty,
                Reviewer2Id: secondary?.LecturerId ?? Guid.Empty
            ));
        }

        return new ReviewTimelineDto(
            campaignId,
            campaignName,
            rows.Select(r => r.GroupCode).Distinct().Count(),
            slots.Count,
            rows.OrderBy(r => r.Date).ThenBy(r => r.SlotCode).ToList());
    }

    public async Task<WorkloadReportDto> GetWorkloadReportAsync(Guid campaignId, CancellationToken ct = default)
    {
        var campaign = await _sessionClient.GetCampaignAsync(campaignId, ct);
        var campaignName = campaign?.Name ?? $"Campaign {campaignId}";

        var slots = await _sessionClient.GetSlotsByCampaignAsync(campaignId, ct);
        var slotIds = slots.Select(s => s.Id).ToHashSet();

        var assignments = await _assignmentClient.GetAllAssignmentsAsync(ct);
        var campaignAssignments = assignments
            .Where(a => slotIds.Contains(a.ReviewSlotId))
            .ToList();

        var assignmentIds = campaignAssignments.Select(a => a.Id).ToList();
        var reviewers = await _assignmentClient.GetReviewersByAssignmentIdsAsync(assignmentIds, ct);

        var lecturerWorkloads = new List<LecturerWorkloadDto>();

        foreach (var group in reviewers.GroupBy(r => r.LecturerId))
        {
            var name = await _identityClient.GetLecturerNameAsync(group.Key, ct) ?? group.Key.ToString();
            lecturerWorkloads.Add(new LecturerWorkloadDto(
                LecturerId: group.Key,
                LecturerName: name,
                Department: "",
                ReviewCount: group.Count(),
                DefenseCount: 0,
                Total: group.Count()
            ));
        }

        return new WorkloadReportDto(
            campaignId,
            campaignName,
            lecturerWorkloads.OrderByDescending(l => l.ReviewCount).ToList(),
            campaignAssignments.Select(a => a.CapstoneGroupId).Distinct().Count(),
            reviewers.Count,
            0);
    }

    public async Task<byte[]> ExportTimelineAsync(Guid campaignId, CancellationToken ct = default)
    {
        var timeline = await GetReviewTimelineAsync(campaignId, ct);
        return _exportService.ExportTimelineToExcel(timeline);
    }

    public async Task<byte[]> ExportWorkloadAsync(Guid campaignId, CancellationToken ct = default)
    {
        var workload = await GetWorkloadReportAsync(campaignId, ct);
        return _exportService.ExportWorkloadToExcel(workload);
    }

    private static int GetWeekOfYear(DateOnly date)
    {
        var jan1 = new DateOnly(date.Year, 1, 1);
        var dayOfYear = date.DayOfYear;
        var dayOfWeekJan1 = (int)jan1.DayOfWeek;
        return (int)Math.Ceiling((dayOfYear + dayOfWeekJan1 - 1) / 7.0);
    }
}
