namespace Report.Domain.DTOs;

public record ReviewTimelineRowDto(
    string WeekCode,
    string DayCode,
    string SlotCode,
    string GroupCode,
    string ProjectCode,
    string ProjectNameEn,
    DateOnly Date,
    string DayOfWeek,
    string Room,
    string Reviewer1Name,
    string Reviewer2Name,
    Guid Reviewer1Id,
    Guid Reviewer2Id);

public record ReviewTimelineDto(
    Guid CampaignId,
    string CampaignName,
    int TotalGroups,
    int TotalSlots,
    List<ReviewTimelineRowDto> Rows);

public record LecturerWorkloadDto(
    Guid LecturerId,
    string LecturerName,
    string Department,
    int ReviewCount,
    int DefenseCount,
    int Total);

public record WorkloadReportDto(
    Guid CampaignId,
    string CampaignName,
    List<LecturerWorkloadDto> LecturerWorkloads,
    int TotalGroups,
    int TotalReviews,
    int TotalDefense);

public record ExportResultDto(
    string FileName,
    string ContentType,
    byte[] Data);
