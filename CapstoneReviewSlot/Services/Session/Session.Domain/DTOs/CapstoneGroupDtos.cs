namespace Session.Domain.DTOs;

public record CapstoneGroupDto(
    Guid Id,
    Guid CampaignId,
    string CampaignName,
    string GroupCode,
    string ProjectCode,
    string ProjectNameEn,
    string ProjectNameVn,
    Guid MentorLecturerId,
    string MentorLecturerName,
    List<Guid> AdditionalSupervisorIds,
    List<string> AdditionalSupervisorNames,
    List<CapstoneGroupMemberDto> Members);

public record CapstoneGroupMemberDto(
    Guid Id,
    string StudentMssv,
    string StudentName,
    string Department);

public record CreateCapstoneGroupDto(
    Guid CampaignId,
    string GroupCode,
    string ProjectCode,
    string ProjectNameEn,
    string ProjectNameVn,
    string MentorLecturerName,
    List<string>? AdditionalSupervisorNames,
    List<StudentMemberDto>? Members);

public record StudentMemberDto(
    string StudentMssv,
    string StudentName,
    string Department);

public record ImportResultDto(
    int TotalRows,
    int SuccessCount,
    int FailureCount,
    List<string> Errors);

public record ImportGroupRowDto(
    string GroupCode,
    string ProjectCode,
    string ProjectNameEn,
    string ProjectNameVn,
    string MentorLecturerName,
    string AdditionalSupervisorNames,
    string StudentMssvList);

public record ImportStudentRowDto(
    string StudentMssv,
    string StudentName,
    string GroupCode,
    string Department);

public record ImportAvailabilityRowDto(
    string FullName,
    string? MonSlots,
    string? TueSlots,
    string? WedSlots,
    string? ThuSlots,
    string? FriSlots);
